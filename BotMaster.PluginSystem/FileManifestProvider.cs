using BotMaster.PluginSystem.Connection;

using NLog;

using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;

namespace BotMaster.PluginSystem
{
    public class FileManifestProvider : IDisposable
    {
        private readonly IDisposable manifestProviderDisposable;

        public FileManifestProvider(DirectoryInfo pluginInfo, ManifestProvider manifestProvider)
        {
            var method = GetPluginManifests(pluginInfo, LogManager.GetLogger($"{nameof(BotMaster)}.{nameof(FileManifestProvider)}"));
            manifestProviderDisposable = manifestProvider.RegisterSubProvider(method);
        }

        private static IObservable<(PluginManifest, Guid)> GetPluginManifests(DirectoryInfo pluginInfo, ILogger logger)
            => Observable
                .Merge(
                    pluginInfo
                        .GetFiles("plugin.manifest.json", SearchOption.AllDirectories)
                        .ToObservable(),
                    Observable.Using
                        (() =>
                            {
                                var watcher = new FileSystemWatcher
                                {
                                    IncludeSubdirectories = true,
                                    Path = pluginInfo.FullName,
                                };
                                watcher.Filters.Add("plugin.manifest.json");
                                watcher.EnableRaisingEvents = true;
                                return watcher;
                            },
                            watcher => Observable.Merge(CreateChangedEvents(watcher), CreateDeletedEvents(watcher), CreateRenameEvents(watcher))
                        )
                        .Select(e => new FileInfo(e.EventArgs.FullPath))
                )
                .Select(file =>
                {
                    logger.Trace("rcv new manifest files");

                    var manifest = JsonSerializer.Deserialize<PluginManifest>(
                        File.ReadAllText(file.FullName),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    manifest.CurrentFileInfo = file;
                    manifest.ManifestInfo = ManifestInfo.File;
                    return manifest;
                })
                .DistinctUntilChanged()
                .Select(x => (x, Guid.NewGuid()));

        private static IObservable<EventPattern<FileSystemEventArgs>> CreateChangedEvents(FileSystemWatcher watcher)
            => Observable
                    .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                            handler => watcher.Changed += handler,
                            handler => watcher.Changed -= handler);

        private static IObservable<EventPattern<FileSystemEventArgs>> CreateDeletedEvents(FileSystemWatcher watcher)
            => Observable
                    .FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                            handler => watcher.Deleted += handler,
                            handler => watcher.Deleted -= handler);

        private static IObservable<EventPattern<FileSystemEventArgs>> CreateRenameEvents(FileSystemWatcher watcher)
            => Observable
                    .FromEventPattern<RenamedEventHandler, FileSystemEventArgs>(
                            handler => watcher.Renamed += handler,
                            handler => watcher.Renamed -= handler);

        public void Dispose()
        {
            manifestProviderDisposable.Dispose();
        }
    }

}

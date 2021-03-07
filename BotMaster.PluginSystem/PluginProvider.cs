using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BotMaster.PluginSystem
{
    public static class PluginProvider
    {
        public static IObservable<PluginServiceInstance> Watch(DirectoryInfo directory, FileInfo pluginHost)
            => GetPluginManifests(directory)
                   .Select(manifest =>
                    {
                        Process process = new()
                        {
                            StartInfo = new(pluginHost.FullName, $"-l \"{manifest.CurrentFileInfo.FullName}\"")
                            {
                                WorkingDirectory = manifest.CurrentFileInfo.Directory.FullName

                            },

                        };

                        var instance = new PluginServiceInstance(
                            manifest,
                            process,
                            packages => PluginServer.Create(manifest.Id, packages));

                        return instance;
                    });

        private static IObservable<PluginManifest> GetPluginManifests(DirectoryInfo directory)
            => Observable
                .Merge(
                    directory
                        .GetFiles("manifest.json", SearchOption.AllDirectories)
                        .ToObservable(),
                    Observable.Using
                        (() =>
                            {
                                var watcher = new FileSystemWatcher
                                {
                                    IncludeSubdirectories = true,
                                    Path = directory.FullName,
                                };
                                watcher.Filters.Add("manifest.json");
                                watcher.EnableRaisingEvents = true;
                                return watcher;
                            },
                            watcher => Observable.Merge(CreateChangedEvents(watcher), CreateDeletedEvents(watcher), CreateRenameEvents(watcher))
                        )
                        .Select(e => new FileInfo(e.EventArgs.FullPath))
                )
                .Select(file =>
                {
                    var manifest = JsonSerializer.Deserialize<PluginManifest>(
                        File.ReadAllText(file.FullName),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    manifest.CurrentFileInfo = file;
                    return manifest;
                })
                .DistinctUntilChanged();

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
    }

}

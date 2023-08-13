using System.Reactive.Disposables;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BotMaster.PluginSystem
{
    public class RunnerInstance : IDisposable
    {
        private readonly Process process;
        private readonly CompositeDisposable compositeDisposable;
        private int retries = 0;

        public RunnerInstance(
            DirectoryInfo runnersPath,
            PluginManifest manifest,
            Guid id)
        {
            var runnerManifestPath = new FileInfo(Path.Combine(runnersPath.FullName, manifest.ProcessRunner, "runner.manifest.json"));

            if (!runnerManifestPath.Exists || manifest.CurrentFileInfo is null)
                return; //Runner not running without runner info, obviously

            using var str = runnerManifestPath.OpenRead();
            var runnerManifest = System.Text.Json.JsonSerializer.Deserialize<RunnerManifest>(str);

            var processPath = runnerManifest.Filename["default"];
            foreach (var item in runnerManifest.Filename)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Create(item.Key)))
                    processPath = item.Value;
            }

            var args = runnerManifest.Args;
            foreach (var item in runnerManifest.EnviromentVariable)
            {
                args = args.Replace($"{{{item.Key}}}", item.Value
                    .Replace("{manifestpath}", manifest.CurrentFileInfo.FullName)
                    .Replace("{runnerpath}", runnerManifestPath.Directory.FullName)
                    .Replace("{instanceid}", id.ToString())
                    );
            }

            process = new()
            {
                StartInfo = new ProcessStartInfo(processPath, args)
                {
                    WorkingDirectory = manifest.CurrentFileInfo.Directory.FullName,
                    UseShellExecute = true
                },
                EnableRaisingEvents = true
            };
            compositeDisposable = new CompositeDisposable();
        }

        internal void Kill()
        {
            process.Exited -= ProcessExited;
            process.Kill(true);
            process.WaitForExit(1000);
        }

        internal bool TryStop()
        {
            return process.WaitForExit(10000);
        }

        public void Start()
        {
            if (process is not null)
            {

                process.Exited += ProcessExited;

                process.Start();
                process.Refresh();
            }
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            process.Exited -= ProcessExited;
            if (process.ExitCode == 0 || process.ExitCode == -1073741510)
                return;
            retries++;
            Thread.Sleep((retries + 1) * 10); //TODO Has this any side consequences, that we didn't consider in the heat of the moment?
            //TriggerOnError(new ProcessExitedException(process.ExitCode));
        }

        public void Dispose()
        {
            compositeDisposable.Dispose();
            process.Dispose();
        }
    }
}
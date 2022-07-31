using BotMaster.PluginSystem;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoPluginServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using var manualReset = new ManualResetEvent(false);
            var pluginFolder = new DirectoryInfo(Path.Combine(".", "Plugins"));
            var pluginHost = new FileInfo(@"D:\Projekte\Visual 2019\WarFabrik.Clone\BotMaster.PluginHost\bin\Debug\net5.0\BotMaster.PluginHost.exe");
            var packages = PluginProvider
                                        .Watch(pluginFolder, pluginHost)
                                        .Do(p => p.Start())
                                        .SelectMany(p => p.ReceivedPackages)
                                        .ToEnumerable();

            //.Subscribe(p => { Task.Run( () => Console.WriteLine(Encoding.UTF8.GetString(p.Content.ToArray()))); });
            foreach (var package in packages)
            {
                Console.WriteLine(Encoding.UTF8.GetString(package.Content.ToArray()));
            }

            Console.WriteLine("Finish");
            manualReset.WaitOne();
        }
    }
}

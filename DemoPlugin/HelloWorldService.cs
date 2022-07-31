using BotMaster.PluginSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoPlugin
{
    public class HelloWorldService : Plugin
    {
        public override IObservable<Package> Start(IObservable<Package> receivedPackages) 
            => Observable.Merge(
                Observable
                    .Interval(TimeSpan.FromSeconds(5))
                    .Select(i => new Package(Encoding.UTF8.GetBytes("Hello"))),
                    receivedPackages.Where(f => false)
                );
    }
}

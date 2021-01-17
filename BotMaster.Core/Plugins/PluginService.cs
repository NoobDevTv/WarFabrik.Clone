using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Plugins
{
    public sealed class PluginService : IDisposable
    {
        private readonly Dictionary<string, PluginServiceInstance> plugins;
        private readonly IObservable<PluginServiceInstance> instances;

        private IDisposable subscription;

        public PluginService(IObservable<PluginServiceInstance> instances)
        {
            plugins = new Dictionary<string, PluginServiceInstance>();
            this.instances = instances;
        }

        public void Start()
        {
            subscription = instances
                              .Do(instance =>
                              {
                                  if(plugins.TryGetValue(instance.Id, out var value))
                                  {
                                      if (!value.TryStop())
                                      {
                                          value.Kill();
                                      }

                                      value.Dispose();

                                      plugins[instance.Id] = instance;
                                  }
                                  else
                                  {
                                      plugins.Add(instance.Id, instance);
                                  }
                              })
                              .Subscribe();
        }

        public void Stop()
        {
            subscription?.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}

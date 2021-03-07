using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.PluginSystem
{
    public sealed class PluginService : IDisposable
    {
        private readonly Dictionary<string, PluginServiceInstance> plugins;
        private readonly IObservable<PluginServiceInstance> instances;
        private readonly MessageHub messageHub;
        private IDisposable subscription;

        public PluginService(MessageHub messageHub, IObservable<PluginServiceInstance> instances)
        {
            plugins = new Dictionary<string, PluginServiceInstance>();
            this.instances = instances;
            this.messageHub = messageHub;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Wrong Usage", "DF0021:Marks undisposed objects assinged to a field, originated from method invocation.", Justification = "<Ausstehend>")]
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

                                      using (var oldInstance = plugins[instance.Id])
                                        plugins[instance.Id] = instance;
                                  }
                                  else
                                  {
                                      plugins.Add(instance.Id, instance);
                                  }

                                  instance.Start();
                                  instance.ReceiveMessages(messageHub.SubscribeAsReceiver);
                                  instance.SendMessages(messageHub.SubscribeAsSender);
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

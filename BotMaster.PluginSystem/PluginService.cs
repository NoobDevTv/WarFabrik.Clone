using System.Reactive.Linq;

namespace BotMaster.PluginSystem
{
    public sealed class PluginService : IDisposable
    {
        private readonly Dictionary<string, PluginInstance> plugins;
        private readonly IObservable<PluginInstance> instances;
        private readonly MessageHub messageHub;
        private IDisposable subscription;

        public PluginService(MessageHub messageHub, IObservable<PluginInstance> instances)
        {
            plugins = new Dictionary<string, PluginInstance>();
            this.instances = instances;
            this.messageHub = messageHub;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Wrong Usage", "DF0021:Marks undisposed objects assinged to a field, originated from method invocation.", Justification = "<Ausstehend>")]
        public void Start()
        {
            subscription = instances
                              .Do(instance =>
                              {
                                  if (plugins.TryGetValue(instance.Id, out var value))
                                  {
                                      if (value is PluginProcessServiceInstance instanceProcess)
                                          if (!instanceProcess.TryStop())
                                          {
                                              instanceProcess.Kill();
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

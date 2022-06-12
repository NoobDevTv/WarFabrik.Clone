using BotMaster.PluginSystem.PluginCreator;
using NLog;
using System.Reactive.Linq;

namespace BotMaster.PluginSystem
{
    public sealed class PluginService : IDisposable
    {
        private readonly Logger logger;
        private readonly Dictionary<string, PluginInstance> plugins;
        private readonly IObservable<PluginInstance> instances;
        private readonly MessageHub messageHub;
        private IDisposable subscription;

        public PluginService(MessageHub messageHub, IObservable<PluginInstance> instances)
        {
            logger = LogManager.GetLogger(nameof(PluginService));
            plugins = new Dictionary<string, PluginInstance>();
            this.instances = instances;
            this.messageHub = messageHub;
        }

        public void Start()
        {
            logger.Info("Start Service");
            subscription = instances
                              .Do(instance =>
                              {
                                  logger.Debug("rcv new plugin instance");

                                  if (plugins.TryGetValue(instance.Id, out var oldInstance))
                                  {
                                      if (oldInstance is IPCPluginInstance instanceProcess)
                                      {
                                          logger.Debug("try to stop plugin instance");
                                          if (!instanceProcess.TryStop())
                                          {
                                              logger.Debug("stopping instance failed, kill the process");
                                              instanceProcess.Kill();
                                          }
                                      }

                                      logger.Debug("Update instance");

                                      if (oldInstance is IDisposable oldDisposableInstance)
                                          oldDisposableInstance.Dispose();

                                      plugins[instance.Id] = instance;
                                  }
                                  else
                                  {
                                      logger.Debug("Add instance");
                                      plugins.Add(instance.Id, instance);
                                  }

                                  logger.Debug($"Start Plugin instance {instance.Id}");
                                  instance.Start();
                                  instance.ReceiveMessages(messageHub.SubscribeAsReceiver);
                                  instance.SendMessages(messageHub.SubscribeAsSender);
                                  logger.Debug($"{instance.Id} started");
                              })
                              .Subscribe();
        }

        public void Stop()
        {
            logger.Info("Stop Service");
            subscription?.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}

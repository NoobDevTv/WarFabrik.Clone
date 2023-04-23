using BotMaster.PluginSystem.Messages;
using BotMaster.PluginSystem.PluginCreator;

using NLog;

using System.Reactive.Disposables;
using System.Reactive.Linq;
using System;
using System.Collections.Concurrent;

namespace BotMaster.PluginSystem
{
    public sealed class PluginService : IDisposable
    {
        public IReadOnlyDictionary<string, PluginInstance> Plugins => plugins;

        private readonly Logger logger;
        private readonly ConcurrentDictionary<string, PluginInstance> plugins;
        private readonly IObservable<PluginInstance> instances;
        private readonly MessageHub messageHub;
        private IDisposable subscription;

        public PluginService(MessageHub messageHub, IObservable<PluginInstance> instances)
        {
            logger = LogManager.GetLogger(nameof(PluginService));
            plugins = new ConcurrentDictionary<string, PluginInstance>();
            this.instances = instances;
            this.messageHub = messageHub;
        }

        public void Start()
        {
            logger.Info("Start Service");
            subscription = instances.Subscribe(StartPluginInstance);
        }

        private void StartPluginInstance(PluginInstance instance)
        {
            logger.Debug("rcv new plugin instance");

            if (plugins.TryGetValue(instance.Id, out var oldInstance))
            {
                oldInstance.OnError -= InstanceOnError;

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
                plugins.TryAdd(instance.Id, instance);
            }

            logger.Debug($"Start Plugin instance {instance.Id}");
            instance.OnError += InstanceOnError;


            instance.Start();
            instance.ReceiveMessages(messageHub.GetFiltered);
            instance.SendMessages(messageHub.SubscribeAsSender);
            logger.Debug($"{instance.Id} started");
        }

        private void InstanceOnError(object sender, Exception ex)
        {
            if (sender is PluginInstance pi)
            {
                StartPluginInstance(pi.Copy());
            }
        }

        public void Stop()
        {
            logger.Info("Stop Service");
            subscription?.Dispose();
        }

        public void Dispose()
        {
            Stop();
            subscription?.Dispose();
        }
    }
}

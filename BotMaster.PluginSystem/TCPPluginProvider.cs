using BotMaster.PluginSystem.PluginCreator;

using NLog;
using NonSucking.Framework.Extension.IoC;

using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;

namespace BotMaster.PluginSystem
{
    public class TCPPluginProvider : IPluginProvider
    {
        private readonly Subject<PluginInstance> instanceSubject = new();
        private TcpListener listener;
        private TCPPluginCreator pluginCreator;

        public IObservable<PluginInstance> GetPluginInstances(ILogger logger, ITypeContainer typeContainer)
        {
            var botmasterConfig = typeContainer.Get<BotmasterConfig>();
            pluginCreator = typeContainer.Get<TCPPluginCreator>();


            var port = botmasterConfig.PortForPluginCreation;
            listener = new TcpListener(IPAddress.IPv6Any, port);
            listener.Server.DualMode = true;
            listener.Start();

            listener.BeginAcceptTcpClient(BeginAcceptClient, null);

            return instanceSubject;

            //Create listener
            //Get Clients
            //Handshake: Manifest
            //Create instance
            //Profit
        }

        private void BeginAcceptClient(IAsyncResult ar)
        {
            var client = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(BeginAcceptClient, null);

            var ns = client.GetStream();
            var manifest = System.Text.Json.JsonSerializer.Deserialize<PluginManifest>(ns);
            var instance = pluginCreator.CreateServer(manifest, null);
            instanceSubject.OnNext(instance);
        }
    }
}

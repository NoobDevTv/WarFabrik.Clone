using System.IO.Pipes;

namespace BotMaster.PluginSystem
{
    public static class PluginClient
    {
        public static IObservable<Package> Create(string id, IObservable<Package> sendPipe)
            => PluginConnection<NamedPipeClientStream>.Create(id, sendPipe, CreateClient);

        private static NamedPipeClientStream CreateClient(string id)
        {
            var client = new NamedPipeClientStream(".", id, PipeDirection.InOut, PipeOptions.Asynchronous);

            client.Connect();

            return client;
        }       
    }
}

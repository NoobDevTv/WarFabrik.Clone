using System.IO.Pipes;

namespace BotMaster.PluginSystem
{
    public static class PluginClient
    {
        public static NamedPipeClientStream CreateClient(string id)
        {
            var client = new NamedPipeClientStream(".", id, PipeDirection.InOut, PipeOptions.Asynchronous);

            client.Connect();

            return client;
        }       
    }
}

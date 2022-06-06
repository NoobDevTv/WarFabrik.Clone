using System.IO.Pipes;

namespace BotMaster.PluginSystem
{
    public class PluginServer
    {
        public static IObservable<Package> Create(string id, IObservable<Package> sendPipe)
          => PluginConnection<NamedPipeServerStream>.Create(id, sendPipe, CreateServer);

        private static NamedPipeServerStream CreateServer(string id) 
            => new(id, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

    }
}

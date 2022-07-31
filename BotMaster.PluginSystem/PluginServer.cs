using System.IO.Pipes;

namespace BotMaster.PluginSystem
{
    public class PluginServer
    {
        public static NamedPipeServerStream CreateServer(string id) 
            => new(id, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

    }
}

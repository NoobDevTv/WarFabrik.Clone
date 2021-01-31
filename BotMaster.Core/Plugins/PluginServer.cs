using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Plugins
{
    public class PluginServer
    {
        public static IObservable<Package> Create(string id, IObservable<Package> sendPipe)
          => PluginConnection<NamedPipeServerStream>.Create(id, sendPipe, CreateServer);

        private static NamedPipeServerStream CreateServer(string id) 
            => new(id, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

    }
}

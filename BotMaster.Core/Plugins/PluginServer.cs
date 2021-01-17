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
        {
            var client = new NamedPipeServerStream(id, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

            client.WaitForConnection();

            return client;
        }
     
    }
}

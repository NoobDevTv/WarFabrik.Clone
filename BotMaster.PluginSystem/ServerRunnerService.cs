using System.Net.Sockets;
using BotMaster.Core;
using System.Net;
using System.Text.Json;
using System.Runtime.CompilerServices;
using NLog;
using System.Runtime.InteropServices.JavaScript;

namespace BotMaster.PluginSystem
{
    public class ServerRunnerService
    {
        public event Action<Guid, JsonElement> OnNewMessage;

        TcpListener listener;
        Dictionary<Guid, (TcpClient client, NetworkStream str)> clients = new(); //Plugin Instances

        public ServerRunnerService()
        {
            listener = new TcpListener(IPAddress.IPv6Any, 44545);
            listener.Server.DualMode = true;
            listener.Start();
            listener.BeginAcceptTcpClient(NewClientConnection, null);
        }

        private void NewClientConnection(IAsyncResult ar)
        {
            var client = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(NewClientConnection, null);

            var ns = client.GetStream();

            Span<byte> guidBytes = stackalloc byte[16];
            ns.ReadExactly(guidBytes);
            var guid = new Guid(guidBytes);

            ns.WriteByte(1);
            Task.Run(() =>
            {
                Read(client, ns, guid);
            });

            clients[guid] = (client, ns);
        }

        public void Execute(Command command, Guid instanceId)
        {
            if (clients.TryGetValue(instanceId, out var client))
            {
                if (client.client.Connected)
                {
                    var commandStr = JsonSerializer.SerializeToUtf8Bytes(new { Command = command.ToString() });
                    client.str.Write(BitConverter.GetBytes(commandStr.Length));
                    client.str.Write(commandStr);
                }
                else
                    clients.Remove(instanceId);
            }
        }

        private void Read(TcpClient client, NetworkStream ns, Guid guid)
        {
            Span<byte> size = stackalloc byte[sizeof(int)];
            while (client.Connected)
            {
                ns.ReadExactly(size);
                var toRead = BitConverter.ToInt32(size);

                if (toRead > 1000000) //1Mb
                {
                    var ex = new InvalidDataException("The detected message size was bigger than 1mb, which should not happen. Please write the size of the message first, and then the message itself.");
                    throw ex;
                }
                var message = ReadMessage(ns, toRead);
                OnNewMessage?.Invoke(guid, message);
            }
        }
        private JsonElement ReadMessage(NetworkStream ns, int toRead)
        {
            Span<byte> buffer = stackalloc byte[toRead];
            ns.ReadExactly(buffer);
            return JsonSerializer.Deserialize<JsonElement>(buffer);
        }

    }
}
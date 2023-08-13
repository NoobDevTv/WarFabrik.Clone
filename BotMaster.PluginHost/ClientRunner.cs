using BotMaster.Core;

using NLog;

using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text.Json;

namespace BotMaster.PluginHost
{
    public class ClientRunner<T> : IPluginControlMessage
    {
        private readonly string hostname;
        private readonly ushort port;
        private readonly Guid instanceId;
        private readonly Logger logger;
        private TcpClient client;
        NetworkStream ns;
        protected event Func<T, Task> OnNewMessage;

        public ClientRunner(string hostname, ushort port, Guid instanceId)
        {
            client = new TcpClient();
            this.hostname = hostname;
            this.port = port;
            this.instanceId = instanceId;
            logger = LogManager.GetCurrentClassLogger();
        }

        public void Start()
        {
            logger.Info("Starting the connection");
            client.Connect(hostname, port);
            ns = client.GetStream();

            logger.Info("Writing Instance Id");
            var guidBytes = instanceId.ToByteArray();
            ns.Write(guidBytes);

            if (ns.ReadByte() == 0)
                return; //Failed

            logger.Info("Handshake successful");
            Task.Run(() =>
            {
                Span<byte> size = stackalloc byte[sizeof(int)];
                while (client.Connected)
                {
                    ns.ReadExactly(size);
                    var toRead = BitConverter.ToInt32(size);
                    logger.Info($"Reading {toRead} bytes incomming");

                    if (toRead > 1000000) //1Mb
                    {
                        var ex = new InvalidDataException("The detected message size was bigger than 1mb, which should not happen. Please write the size of the message first, and then the message itself.");
                        logger.Error(ex);
                        throw ex;
                    }
                    var message = ReadMessage(ns, toRead);
                    logger.Info($"Got message {JsonSerializer.Serialize(message)}, invoking event {nameof(OnNewMessage)}");
                    _ = OnNewMessage?.Invoke(message);
                }
            });
        }


        public void Execute(object data)
        {
            Execute(JsonSerializer.Serialize(data));
        }
        public void Execute([StringSyntax(StringSyntaxAttribute.Json)] string data)
        {
            if (client.Connected)
            {
                logger.Debug($"Sending message \"{data}\" to server");
                var commandStr = System.Text.Encoding.UTF8.GetBytes(data);
                ns.Write(BitConverter.GetBytes(commandStr.Length));
                ns.Write(commandStr);
            }
            else
                logger.Warn("Client not connected, couldn't send message");
        }

        private T ReadMessage(NetworkStream ns, int toRead)
        {
            Span<byte> buffer = stackalloc byte[toRead];
            ns.ReadExactly(buffer);
            return JsonSerializer.Deserialize<T>(buffer);
        }

        public void Stop()
        {
            client.Close();
        }
    }
}

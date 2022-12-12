using NLog;

using System.Buffers;
using System.IO.Pipes;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace BotMaster.PluginSystem
{
    public static class PluginConnection
    {
        private static Logger logger;

        static PluginConnection()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        public static IObservable<Package> CreateSendPipe<T>(Func<T> getClientStream, IObservable<Package> sendPipe,
            Func<T, bool> checkConnectionStatus) where T : Stream
            => sendPipe
                .Do(x =>
                {
                    if (!checkConnectionStatus(getClientStream()))
                        logger.Warn("Client is not connected anymore");
                })
                .Where(_ => checkConnectionStatus(getClientStream()))
                .Select(p =>
                {
                    using var buffer = MemoryPool<byte>.Shared.Rent(p.Length);
                    var span = buffer.Memory.Span;
                    var size = p.ToBytes(buffer.Memory.Span);
                    logger.Debug("Send package message");
                    var clientStream = getClientStream();
                    return clientStream
                        .WriteAsync(buffer.Memory[..size])
                        .AsTask()
                        .ToObservable()
                        .Do(i => clientStream.Flush())
                        .Select(i => p);
                })
                .Concat()
                .IgnoreElements();

        public static IObservable<Package> CreateReceiverPipe<T>(Func<T> getClientStream, Func<T, bool> checkConnectionStatus) where T : Stream
            => Observable
                .Create<Package>((observer, token) => Task.Run(async () =>
                {
                    var clientStream = getClientStream();
                    if (clientStream is NamedPipeServerStream serverStream)
                        await serverStream.WaitForConnectionAsync(token);

                    var headerBuffer = new byte[Package.HeaderSize];
                    var headerMemory = new Memory<byte>(headerBuffer);

                    while (true)
                    {
                        try
                        {
                            token.ThrowIfCancellationRequested();
                            if (!checkConnectionStatus(clientStream))
                            {
                                observer.OnError(new Exception("Client is not connected"));
                                return;
                            }

                            logger.Debug("Waiting for new incomming message");

                            await ReadHeader(clientStream, headerMemory, token);
                            var contractId = new Guid(headerBuffer[..16]);
                            var packageSize = BitConverter.ToInt32(headerBuffer, sizeof(int) * 4);
                            using var buffer = MemoryPool<byte>.Shared.Rent(packageSize);
                            var size = await ReadContent(clientStream, packageSize, buffer, token);

                            logger.Debug("Received new incomming message");
                            observer.OnNext(new(contractId, buffer.Memory.Span[..size]));
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            logger.Error(ex);
                            throw;
                        }
                    }
                }, token));

        private static async Task<int> ReadContent(Stream clientStream, int packageSize, IMemoryOwner<byte> buffer, CancellationToken token)
        {
            int size = 0;

            do
            {
                size += await clientStream.ReadAsync(buffer.Memory[size..packageSize], token);
            } while (size < packageSize);

            return size;
        }

        private static async Task ReadHeader(Stream clientStream, Memory<byte> headerMemory, CancellationToken token)
        {
            int headerSize = 0;

            do
            {
                headerSize += await clientStream.ReadAsync(headerMemory[headerSize..Package.HeaderSize], token);
            } while (headerSize < headerMemory.Length);
        }
    }
}

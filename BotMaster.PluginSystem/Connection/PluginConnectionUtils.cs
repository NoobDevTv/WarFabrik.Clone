using NLog;

using System.Buffers;
using System.IO.Pipes;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace BotMaster.PluginSystem.Connection
{

    public static class PluginConnectionUtils
    {
        private static Logger logger;
        private static IScheduler scheduler;

        static PluginConnectionUtils()
        {
            logger = LogManager.GetCurrentClassLogger();
            scheduler = new EventLoopScheduler();
        }

        public static IObservable<Package> CreateSendPipe<T>(T clientStream, IObservable<Package> sendPipe,
            Func<T, bool> checkConnectionStatus) where T : Stream
            => sendPipe
                .ObserveOn(scheduler)
                .Where(_ => checkConnectionStatus(clientStream))
                .Select(p =>
                    {
#pragma warning disable DF0010 // Marks undisposed local variables.
                        var buffer = MemoryPool<byte>.Shared.Rent(p.Length);
#pragma warning restore DF0010 // Marks undisposed local variables.
                        var span = buffer.Memory.Span;
                        var size = p.ToBytes(buffer.Memory.Span);
                        logger.Debug($"Send package message with size {size} on thread {Environment.CurrentManagedThreadId}");

                        return Observable
                            .Using(
                                () => buffer,
                                buffer => Observable.FromAsync(async () => await clientStream.WriteAsync(buffer.Memory[..size]), scheduler)
                            )
                            .Do(i => clientStream.Flush())
                            .Select(i => p);
                    })
                .Concat();

        public static IObservable<Package> CreateReceiverPipe<T>(T clientStream, Func<T, bool> checkConnectionStatus) where T : Stream
            => Observable
                .Create<Package>((observer, token) => Task.Run(async () =>
                {
                    logger.Info(nameof(CreateReceiverPipe) + " started creation process");
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
                                observer.OnError(new PluginConnectionException("Client is not connected"));
                                throw new PluginConnectionException("Client is not connected");
                            }

                            logger.Debug($"Waiting for new incomming message on thread {Environment.CurrentManagedThreadId}");

                            await ReadHeader(clientStream, headerMemory, token);
                            var contractId = new Guid(headerBuffer[..16]);
                            var packageSize = BitConverter.ToInt32(headerBuffer, sizeof(int) * 4);
                            if (packageSize < 1)
                            {
                                logger.Error($"Received a package with an invalid content size {packageSize} on thread {Environment.CurrentManagedThreadId}");
                                continue;
                            }
                            logger.Debug($"Received package with size {packageSize + Package.HeaderSize}");
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

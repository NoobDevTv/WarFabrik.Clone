﻿using System.Buffers;
using System.IO.Pipes;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace BotMaster.PluginSystem
{
    internal static class PluginConnection<T> where T : PipeStream
    {
        public static IObservable<Package> Create(string id, IObservable<Package> sendPipe, Func<string, T> createPipe)
            => Observable.Using(
                () => createPipe(id),
                pipeClient => Observable.Merge(CreateReceiverPipe(pipeClient), CreateSendPipe(pipeClient, sendPipe)));

        private static IObservable<Package> CreateSendPipe(T clientStream, IObservable<Package> sendPipe)
            => sendPipe
                .Where(p => clientStream.IsConnected)
                .Select(p =>
                {
                    using var buffer = MemoryPool<byte>.Shared.Rent(p.Length);
                    var span = buffer.Memory.Span;
                    var size = p.ToBytes(buffer.Memory.Span);
                    return clientStream
                                .WriteAsync(buffer.Memory[..size])
                                .AsTask()
                                .ToObservable()
                                .Do(i => clientStream.Flush())
                                .Select(i => p);
                })
                .Concat()
                .Where(p => false);

        private static IObservable<Package> CreateReceiverPipe(T clientStream)
            => Observable
                .Create<Package>((observer, token) => Task.Run(async () =>
                {
                    if (clientStream is NamedPipeServerStream serverStream)
                        await serverStream.WaitForConnectionAsync(token);

                    var headerBuffer = new byte[Package.HeaderSize];
                    var headerMemory = new Memory<byte>(headerBuffer);

                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        try
                        {
                            if (!clientStream.IsConnected)
                            {
                                observer.OnCompleted();
                                return;
                            }

                            await ReadHeader(clientStream, headerMemory, token);
                            var contractId = new Guid(headerBuffer[..16]);
                            var packageSize = BitConverter.ToInt32(headerBuffer, sizeof(int)*4);
                            using var buffer = MemoryPool<byte>.Shared.Rent(packageSize);
                            var size = await ReadContent(clientStream, packageSize, buffer, token);

                            observer.OnNext(new(contractId, buffer.Memory.Span[..size]));
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            throw;
                        }
                    }
                }, token));

        private static async Task<int> ReadContent(T clientStream, int packageSize, IMemoryOwner<byte> buffer, CancellationToken token)
        {
            int size = 0;

            do
            {
                size += await clientStream.ReadAsync(buffer.Memory[size..packageSize], token);
            } while (size < packageSize);

            return size;
        }

        private static async Task ReadHeader(T clientStream, Memory<byte> headerMemory, CancellationToken token)
        {
            int headerSize = 0;

            do
            {
                headerSize += await clientStream.ReadAsync(headerMemory[headerSize..Package.HeaderSize], token);
            } while (headerSize < headerMemory.Length);
        }
    }
}

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Plugins
{
    internal static class PluginConnection<T> where T : PipeStream
    {
        public static IObservable<Package> Create(string id, IObservable<Package> sendPipe, Func<string, T> createPipe) 
            => Observable.Using(
                () => createPipe(id),
                pipeClient => Observable.Merge(CreateReceivedPipe(pipeClient), CreateSendPipe(pipeClient, sendPipe)));

        private static IObservable<Package> CreateSendPipe(T clientStream, IObservable<Package> sendPipe)
            => sendPipe
                .Where(p => clientStream.IsConnected)
                .SelectMany(p =>
                {
                    using var buffer = MemoryPool<byte>.Shared.Rent(1024);
                    var span = buffer.Memory.Span;
                    var size = p.ToBytes(buffer.Memory.Span);
                    byte[]? array = span[..size].ToArray();
                    var mem = new ReadOnlyMemory<byte>(array);
                    return clientStream
                                .WriteAsync(mem)
                                .AsTask()
                                .ToObservable()
                                .Do(i => clientStream.Flush())
                                .Select(i => p);
                })
                .Where(p => false);

        private static IObservable<Package> CreateReceivedPipe(T clientStream)
            => Observable
                .Create<Package>((observer, token) => Task.Run(async () =>
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        using var buffer = MemoryPool<byte>.Shared.Rent(1024);
                        int size = 0;
                        try
                        {
                            if (!clientStream.IsConnected)
                            {
                                observer.OnCompleted();
                                return;
                            }

                            size = await clientStream.ReadAsync(buffer.Memory, token);
                        }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                            throw;
                        }

                        observer.OnNext(Package.FromMemory(buffer.Memory.Span[..size]));
                    }
                }, token));
    }
}

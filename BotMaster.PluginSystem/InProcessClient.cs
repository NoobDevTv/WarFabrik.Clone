using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.PluginSystem
{
    public class InProcessClient : IDisposable
    {
        private readonly Subject<Package> internalSubject;
        private readonly IScheduler internalScheduler;
        private readonly IDisposable disposable;

        private static Dictionary<string, InProcessClient> _clients = new Dictionary<string, InProcessClient>();

        public InProcessClient()
        {
            internalSubject = new Subject<Package>();
            internalScheduler = new TaskPoolScheduler(Task.Factory);

            disposable = StableCompositeDisposable.Create(internalSubject);
        }

        public void Dispose()
        {
            disposable.Dispose();
        }

        public IObservable<Package> InjectPackages(IObservable<Package> packages)
            => packages.ObserveOn(internalScheduler).Do(internalSubject.OnNext);
        //=> packages.Do(package => Task.Run(() => internalSubject.OnNext(package)));

        public IObservable<Package> ReceivedPackages()
            => internalSubject;

        public static InProcessClient Create(string id)
        {
            lock (_clients)
            {
                if (!_clients.TryGetValue(id, out var client))
                {
                    client = new InProcessClient();
                    _clients.Add(id, client);
                }

                return client;
            }
        }

        public static IObservable<Package> CreateSendStream(InProcessClient clientStream, IObservable<Package> sendPipe)
        {
            return clientStream.InjectPackages(sendPipe);
        }

        public static IObservable<Package> CreateReceiverStream(InProcessClient clientStream)
        {
            return clientStream.ReceivedPackages();
        }
    }
}
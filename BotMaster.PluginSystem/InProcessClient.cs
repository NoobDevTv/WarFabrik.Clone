using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace BotMaster.PluginSystem
{
    public class InProcessClient : IDisposable
    {
        private readonly IScheduler internalScheduler;
        private readonly IDisposable disposable;

        //Debugging only
        private readonly string id;
        private readonly Subject<Package> remote;
        private readonly Subject<Package> own;

        public InProcessClient(string id, Subject<Package> remote, Subject<Package> own)
        {
            internalScheduler = TaskPoolScheduler.Default;


            disposable = StableCompositeDisposable.Create(remote, own);
            this.id = id;
            this.remote = remote;
            this.own = own;
        }


        public void Dispose()
        {
            disposable.Dispose();
        }

        public IObservable<Package> InjectPackages(IObservable<Package> packages)
            => packages.ObserveOn(internalScheduler).Do(x => remote.OnNext(x));

        public IObservable<Package> ReceivedPackages()
            => own;

        public static IObservable<Package> CreateSendStream(InProcessClient clientStream, IObservable<Package> sendPipe)
        {
            return clientStream.InjectPackages(sendPipe);
        }

        public static IObservable<Package> CreateReceiverStream(InProcessClient clientStream)
        {
            return clientStream.ReceivedPackages();
        }

        public override string ToString()
            => $"{nameof(InProcessClient)} for {id}";
    }
}
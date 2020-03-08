using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core
{
    public static class IObservableExtension
    {
        public static IObservable<TSource> ResubscribeTimer<TSource>(this IObservable<TSource> observable, TimeSpan timeSpan)
            => Observable.Create<TSource>((observer, token) => Task.Run(async () =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    using (observable.Subscribe(observer))
                    {
                        await Task.Delay(timeSpan, token);
                    }
                }
            }, token));
    }
}

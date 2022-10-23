using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.Extensibility;
public static class IObservableExtension
{

    public static IObservable<TSource> Retry<TException, TSource>(this IObservable<TSource> source, Func<TException, bool> predicate)
        where TException : Exception
        => source
            .Materialize()
            .Select(n => n.Kind == NotificationKind.OnError && n.Exception is TException ex2 && predicate(ex2) ? throw ex2 : n)
            .Retry()
            .Dematerialize();

    public static IObservable<TSource> RetryDelayed< TSource>(this IObservable<TSource> source, TimeSpan delay)
        => RetryDelayed(source, (e) => true, delay, TaskPoolScheduler.Default);

    public static IObservable<TSource> RetryDelayed<TException, TSource>(this IObservable<TSource> source, Func<TException, bool> predicate, TimeSpan delay)
        where TException : Exception
        => RetryDelayed(source, predicate, delay, TaskPoolScheduler.Default);


    public static IObservable<TSource> RetryDelayed<TSource>(this IObservable<TSource> source, Func<Exception, bool> predicate, TimeSpan delay)
        => RetryDelayed(source, predicate, delay, TaskPoolScheduler.Default);

    public static IObservable<TSource> RetryDelayed<TException, TSource>(this IObservable<TSource> source, Func<TException, bool> predicate, TimeSpan delay, IScheduler scheduler)
        where TException : Exception
        => source
            .Materialize()
            .SelectMany(n
                => n.Kind == NotificationKind.OnError && n.Exception is TException ex2 && predicate(ex2)
                    ? Observable.Timer(delay, scheduler).SelectMany(Observable.Throw<Notification<TSource>>(ex2))
                    : Observable.Return(n))
            .Retry()
            .Dematerialize();


    public static IObservable<TSource> RetryDelayed<TSource>(this IObservable<TSource> source, Func<Exception, bool> predicate, TimeSpan delay, IScheduler scheduler)
        => source
            .Materialize()
            .SelectMany(n
                => n.Kind == NotificationKind.OnError && predicate(n.Exception)
                    ? Observable.Timer(delay, scheduler).SelectMany(Observable.Throw<Notification<TSource>>(n.Exception))
                    : Observable.Return(n))
            .Retry()
            .Dematerialize();
}

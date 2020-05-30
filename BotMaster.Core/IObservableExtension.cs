using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core
{
    public static class IObservableExtension
    {        

        /// <summary>
        ///     Repeats the source observable sequence until it completes successfully unless the predicate returns false.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Observable sequence to repeat until it successfully terminates.</param>
        /// <param name="predicate">Predicate evaluated for each failed termination to determine whether to repeat the sequence.</param>
        /// <returns>An observable sequence producing the elements of the given sequence repeatedly until it terminates successfully or the predicate fails.</returns>
        public static IObservable<TSource> Retry<TException, TSource>(this IObservable<TSource> source, Func<TException, bool> predicate)
            where TException : Exception
            => source
                .Materialize()
                .Select(n => n.Kind == NotificationKind.OnError && n.Exception is TException ex2 && predicate(ex2) ? throw ex2 : n)
                .Retry()
                .Dematerialize();

        /// <summary>
        ///     Repeats the source observable sequence until it completes successfully unless the predicate returns false, delaying each repetition by the given amount of time.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Observable sequence to repeat until it successfully terminates.</param>
        /// <param name="predicate">Predicate evaluated for each failed termination to determine whether to repeat the sequence.</param>
        /// <param name="delay">The amount of time to delay each repetation.</param>
        /// <returns>An observable sequence producing the elements of the given sequence repeatedly until it terminates successfully or the predicate fails.</returns>
        public static IObservable<TSource> Retry<TException, TSource>(this IObservable<TSource> source, Func<TException, bool> predicate, TimeSpan delay)
            where TException : Exception
            => Retry(source, predicate, delay, TaskPoolScheduler.Default);

        /// <summary>
        ///     Repeats the source observable sequence until it completes successfully unless the predicate returns false, delaying each repetition by the given amount of time.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Observable sequence to repeat until it successfully terminates.</param>
        /// <param name="predicate">Predicate evaluated for each failed termination to determine whether to repeat the sequence.</param>
        /// <param name="delay">The amount of time to delay each repetation.</param>
        /// <returns>An observable sequence producing the elements of the given sequence repeatedly until it terminates successfully or the predicate fails.</returns>
        public static IObservable<TSource> Retry<TSource>(this IObservable<TSource> source, Func<Exception, bool> predicate, TimeSpan delay)
            => Retry(source, predicate, delay, TaskPoolScheduler.Default);

        /// <summary>
        ///     Repeats the source observable sequence until it completes successfully unless the predicate returns false, delaying each repetition by the given amount of time
        ///     using the provided scheduler for its internal timer.
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Observable sequence to repeat until it successfully terminates.</param>
        /// <param name="predicate">Predicate evaluated for each failed termination to determine whether to repeat the sequence.</param>
        /// <param name="delay">The amount of time to delay each repetation.</param>
        /// <param name="scheduler">The scheduler to run timers on.</param>
        /// <returns>An observable sequence producing the elements of the given sequence repeatedly until it terminates successfully or the predicate fails.</returns>
        public static IObservable<TSource> Retry<TException, TSource>(this IObservable<TSource> source, Func<TException, bool> predicate, TimeSpan delay, IScheduler scheduler)
            where TException : Exception
            => source
                .Materialize()
                .SelectMany(n
                    => n.Kind == NotificationKind.OnError && n.Exception is TException ex2 && predicate(ex2)
                        ? Observable.Timer(delay, scheduler).SelectMany(Observable.Throw<Notification<TSource>>(ex2))
                        : Observable.Return(n))
                .Retry()
                .Dematerialize();

        /// <summary>
        ///     Repeats the source observable sequence until it completes successfully unless the predicate returns false, delaying each repetition by the given amount of time
        ///     using the provided scheduler for its internal timer.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">Observable sequence to repeat until it successfully terminates.</param>
        /// <param name="predicate">Predicate evaluated for each failed termination to determine whether to repeat the sequence.</param>
        /// <param name="delay">The amount of time to delay each repetation.</param>
        /// <param name="scheduler">The scheduler to run timers on.</param>
        /// <returns>An observable sequence producing the elements of the given sequence repeatedly until it terminates successfully or the predicate fails.</returns>
        public static IObservable<TSource> Retry<TSource>(this IObservable<TSource> source, Func<Exception, bool> predicate, TimeSpan delay, IScheduler scheduler)
            => source
                .Materialize()
                .SelectMany(n
                    => n.Kind == NotificationKind.OnError && predicate(n.Exception)
                        ? Observable.Timer(delay, scheduler).SelectMany(Observable.Throw<Notification<TSource>>(n.Exception))
                        : Observable.Return(n))
                .Retry()
                .Dematerialize();
    }
}

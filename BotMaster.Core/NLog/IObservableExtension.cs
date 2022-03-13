using NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Core.NLog
{
    public static class IObservableExtension
    {

        public static IObservable<T> Trace<T>(this IObservable<T> observable, ILogger logger)
            => observable.Do(e => logger.Trace(e));

        public static IObservable<T> Debug<T>(this IObservable<T> observable, ILogger logger)
            => observable.Do(e => logger.Debug(e));

        public static IObservable<T> Info<T>(this IObservable<T> observable, ILogger logger)
            => observable.Do(e => logger.Info(e));

        public static IObservable<T> Warn<T>(this IObservable<T> observable, ILogger logger)
            => observable.Do(e => logger.Warn(e));

        public static IObservable<T> Error<T>(this IObservable<T> observable, ILogger logger)
            => observable.Do(e => logger.Error(e));

        public static IObservable<T> Fatal<T>(this IObservable<T> observable, ILogger logger)
            => observable.Do(e => logger.Fatal(e));

        public static IObservable<T> Trace<T>(this IObservable<T> observable, ILogger logger, Func<T, string> logFunc)
            => observable.Do(e => logger.Trace(logFunc(e)));

        public static IObservable<T> Debug<T>(this IObservable<T> observable, ILogger logger, Func<T, string> logFunc)
            => observable.Do(e => logger.Debug(logFunc(e)));

        public static IObservable<T> Info<T>(this IObservable<T> observable, ILogger logger, Func<T, string> logFunc)
            => observable.Do(e => logger.Info(logFunc(e)));

        public static IObservable<T> Warn<T>(this IObservable<T> observable, ILogger logger, Func<T, string> logFunc)
            => observable.Do(e => logger.Warn(logFunc(e)));

        public static IObservable<T> Error<T>(this IObservable<T> observable, ILogger logger, Func<T, string> logFunc)
            => observable.Do(e => logger.Error(logFunc(e)));

        public static IObservable<T> Fatal<T>(this IObservable<T> observable, ILogger logger, Func<T, string> logFunc)
            => observable.Do(e => logger.Fatal(logFunc(e)));

        public static IObservable<T> Trace<T, TException>(this IObservable<T> observable, ILogger logger,
            Func<T, (string, TException)> logFunc) where TException : Exception
            => observable.Do(e =>
            {
                var logLine = logFunc(e);
                logger.Trace(logLine.Item2, logLine.Item1);
            });

        public static IObservable<T> Debug<T, TException>(this IObservable<T> observable, ILogger logger,
            Func<T, (string, TException)> logFunc) where TException : Exception
            => observable.Do(e =>
            {
                var logLine = logFunc(e);
                logger.Debug(logLine.Item2, logLine.Item1);
            });

        public static IObservable<T> Info<T, TException>(this IObservable<T> observable, ILogger logger,
            Func<T, (string, TException)> logFunc) where TException : Exception
            => observable.Do(e =>
            {
                var logLine = logFunc(e);
                logger.Info(logLine.Item2, logLine.Item1);
            });

        public static IObservable<T> Warn<T, TException>(this IObservable<T> observable, ILogger logger,
            Func<T, (string, TException)> logFunc) where TException : Exception
            => observable.Do(e =>
            {
                var logLine = logFunc(e);
                logger.Warn(logLine.Item2, logLine.Item1);
            });

        public static IObservable<T> Error<T, TException>(this IObservable<T> observable, ILogger logger,
            Func<T, (string, TException)> logFunc) where TException : Exception
            => observable.Do(e =>
            {
                var logLine = logFunc(e);
                logger.Error(logLine.Item2, logLine.Item1);
            });

        public static IObservable<T> Fatal<T, TException>(this IObservable<T> observable, ILogger logger,
            Func<T, (string, TException)> logFunc) where TException : Exception
            => observable.Do(e =>
            {
                var logLine = logFunc(e);
                logger.Fatal(logLine.Item2, logLine.Item1);
            });

        public static IObservable<T> OnError<T>(this IObservable<T> observable, ILogger logger, string name)
            => Log(observable, logger, name, onError: LogLevel.Error);

        public static IObservable<T> OnError<T>(this IObservable<T> observable, ILogger logger, string name, Func<Exception, string> logFunc)
            => Log(observable, logger, name, onError: LogLevel.Error, onErrorMessage: logFunc);

        public static IObservable<T> Log<T>(
           this IObservable<T> source,
           ILogger log,
           string name,
           LogLevel subscription = default,
           Func<string> subscriptionMessage = null,
           LogLevel onNext = default,
           Func<T, string> onNextMessage = null,
           LogLevel onError = default,
           Func<Exception, string> onErrorMessage = null,
           LogLevel onCompleted = default,
           Func<string> onCompletedMessage = null)
        {
            var logged =
                source
                .Do(x => log.Log(onNext ?? LogLevel.Off, $"{name}: {(onNextMessage == null ? x.ToString() : onNextMessage(x))}"),
                    ex => log.Log(onError ?? LogLevel.Off, ex, $"{name}: {(onErrorMessage == null ? $"OnError: {ex.Message}" : onErrorMessage(ex))}"),
                    () => log.Log(onCompleted ?? LogLevel.Off, $"{name}: {(onCompletedMessage == null ? "OnCompleted" : onCompletedMessage())}"));

            // Avoid creating a new observable if subscription events aren't supposed to be logged.
            if (subscription == LogLevel.Off || subscription == default)
            {
                return logged;
            }
            else
            {
                return Observable.Create<T>(o =>
                {
                    log.Log(subscription, $"{name}: {(subscriptionMessage == null ? "Subscribe" : subscriptionMessage())}");
                    return new CompositeDisposable
                    {
                        logged.Subscribe(o),
                        Disposable.Create(() => log.Log(subscription, $"{name}: Dispose")),
                    };
                });
            }
        }
    }
}

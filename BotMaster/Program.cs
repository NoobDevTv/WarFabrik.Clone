using NLog;
using NLog.Config;
using NLog.Targets;
using NoobDevBot.Telegram;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WarFabrik.Clone;
using static WarFabrik.Clone.FollowerServiceNew;

namespace BotMaster
{
    internal class Program
    {
        private static Logger logger;
        private static CancellationTokenSource tokenSource;
        private static Bot twitchBot;
        private static TelegramBot telegramBot;

        private static ManualResetEvent resetEvent;

        internal static async Task Main(string[] args)
        {
            var config = new LoggingConfiguration();
            resetEvent = new ManualResetEvent(false);
            tokenSource = new CancellationTokenSource();

            var info = new FileInfo(Path.Combine(".", "additionalfiles", "botmaster.log"));

            if (!info.Directory.Exists)
                info.Directory.Create();

#if DEBUG
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("botmaster.logconsole"));
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("botmaster.logfile") { FileName = info.FullName });
#else
            config.AddRule(LogLevel.Info, LogLevel.Fatal, new FileTarget("botmaster.logfile") { FileName = info.FullName });
#endif
            LogManager.Configuration = config;
            logger = LogManager.GetCurrentClassLogger();
            Console.CancelKeyPress += ConsoleCancelKeyPress;

            telegramBot = new TelegramBot();
            
            twitchBot = new Bot();
            twitchBot.FollowerService.OnNewFollowersDetected += TwitchNewFollower;
            twitchBot.OnRaid += TwitchBotOnHosted;

            await twitchBot.Run(tokenSource.Token);
            
            logger.Info("Der Bot ist Online");
            telegramBot.SendMessageToGroup("NoobDev", "Der Bot ist Online");
            resetEvent.WaitOne();
        }

        private static void TwitchBotOnHosted(object sender, string message) 
            => telegramBot.SendMessageToGroup("NoobDev", message);

        private static void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Info("Quit Bot master");
            telegramBot.Exit();
            twitchBot.Disconnect();
            tokenSource.Cancel();
            resetEvent.Set();
        }

        private static void TwitchNewFollower(object sender, NewFollowerDetectedArgs e)
            => telegramBot.SendMessageToGroup("NoobDev", "Following people just followed: " + string.Join(", ", e.NewFollowers.Select(x=>x.UserName)));
    }
}

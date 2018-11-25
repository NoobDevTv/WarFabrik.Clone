using NLog;
using NLog.Config;
using NLog.Targets;
using NoobDevBot.Telegram;
using System;
using System.IO;
using System.Threading;
using WarFabrik.Clone;
using static WarFabrik.Clone.FollowerServiceNew;

namespace BotMaster
{
    class Program
    {
        private static Logger logger;
        private static ManualResetEvent manualReset;
        private static Bot twitchBot;
        private static TelegramBot telegramBot;

        static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            var info = new FileInfo(Path.Combine(".", "additionalFiles", "botmaster.log"));

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


            manualReset = new ManualResetEvent(false);

            telegramBot = new TelegramBot();

            twitchBot = new Bot();
            twitchBot.Connect();
            
            twitchBot.FollowerService.OnNewFollowersDetected += TwitchNewFollower;
            twitchBot.OnHosted += TwitchBotOnHosted;
            Console.CancelKeyPress += ConsoleCancelKeyPress;
            logger.Info("Der Bot ist Online");
            telegramBot.SendMessageToGroup("NoobDev", "Der Bot ist Online");
            manualReset.WaitOne();

        }

        private static void TwitchBotOnHosted(object sender, (string Name, int Count) e) 
            => telegramBot.SendMessageToGroup("NoobDev", $"We are hosted by {e.Name} with {e.Count} viewers");

        private static void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.Info("Quit Bot master");
            telegramBot.Exit();
            twitchBot.Disconnect();
            manualReset.Set();
        }

        private static void TwitchNewFollower(object sender, NewFollowerDetectedArgs e)
        {
            string followerNames = "";
            e.NewFollowers.ForEach(x => followerNames += x.User.DisplayName + ", ");
            telegramBot.SendMessageToGroup("NoobDev", "Following people just followed: " + followerNames.Trim(',').Trim());
        }
    }
}

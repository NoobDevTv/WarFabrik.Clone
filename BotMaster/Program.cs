using NoobDevBot.Telegram;
using System;
using System.Threading;
using WarFabrik.Clone;

namespace BotMaster
{
    class Program
    {
        private static ManualResetEvent manualReset;
        private static Bot twitchBot;
        private static TelegramBot telegramBot;

        static void Main(string[] args)
        {
            manualReset = new ManualResetEvent(false);

            telegramBot = new TelegramBot();

            twitchBot = new Bot();
            twitchBot.Connect();
            
            twitchBot.FollowerService.OnNewFollowersDetected += TwitchNewFollower;
            Console.CancelKeyPress += ConsoleCancelKeyPress;
            telegramBot.SendMessageToGroup("NoobDev", "New Follower Alert Test");
            //Console.ReadKey();
            manualReset.WaitOne();

        }

        private static void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Quit Bot master");
            telegramBot.Exit();
            twitchBot.Disconnect();
            manualReset.Set();
        }

        private static void TwitchNewFollower(object sender, NewFollowerDetectedArgs e)
        {
            string followerNames = "";
            e.NewFollowers.ForEach(x => followerNames += x.User.DisplayName + ", ");
            telegramBot.SendMessageToGroup("NoobDev", "Following people just followed: " + followerNames);
        }
    }
}

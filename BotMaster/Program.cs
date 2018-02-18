using NoobDevBot.Telegram;
using System;
using WarFabrik.Clone;

namespace BotMaster
{
    class Program
    {
        private static Bot twitchBot;
        private static TelegramBot telegramBot;

        static void Main(string[] args)
        {
            telegramBot = new TelegramBot();

            //twitchBot = new Bot();
            //twitchBot.Connect();


            //twitchBot.FollowerService.OnNewFollowersDetected += TwitchNewFollower;

            //telegramBot.SendMessageToGroup("NoobDev", "New Follower Alert Test");-
            Console.ReadKey();
        }

        private static void TwitchNewFollower(object sender, NewFollowerDetectedArgs e)
        {
            string followerNames = "";
            e.NewFollowers.ForEach(x => followerNames += x.User.DisplayName + ", ");
            telegramBot.SendMessageToGroup("NoobDev", "Following people just followed: " + followerNames);
        }
    }
}

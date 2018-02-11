using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace WarFabrik.Clone.Commands
{
    public static class SimpleCommands
    {
        static Random random;

        static SimpleCommands()
        {
            random = new Random();
        }

        [Command("hype")]
        public static bool Hype(BotCommandArgs args)
        {
            var smilies = new string[] { ":)", ":D", "O_o", "B)", ":O", "<3", ";)", ":P", ";P", "R)" };
            string msg = "";

            for (int i = 0; i < 15; i++)
            {
                msg += smilies[random.Next(0, smilies.Length)] + " ";
            }

            args.Bot.SendMessage(msg);

            return true;
        }

        [Command("uptime")]
        public static bool UptTime(BotCommandArgs args)
        {
            var uptime = args.TwitchAPI.Streams.v5.GetUptimeAsync("noobdevtv").Result;

            if (uptime == null)
                return false;

            args.Bot.SendMessage(
                $"Der Stream läuft schon {uptime.Value.Hours}:{uptime.Value.Minutes}:{uptime.Value.Seconds}");
            return true;
        }
    }
}

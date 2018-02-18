using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        public static bool Uptime(BotCommandArgs args)
        {
            return UptimeAsync(args).Result;
        }

        private static async Task<bool> UptimeAsync(BotCommandArgs args)
        {
            var uptime = await args.TwitchAPI.Streams.v5.GetUptimeAsync(args.Message.RoomId);

            if (uptime == null)
            {
                args.Bot.SendMessage("Irgendwas ist mit dem !uptime Befehl kaputt gegangen :(");

                return false;
            }

            args.Bot.SendMessage(
                $"Der Stream läuft schon {uptime.Value.Hours.ToString("d2")}:{uptime.Value.Minutes.ToString("d2")}:{uptime.Value.Seconds.ToString("d2")}");
            return true;
        }

        [Command("?", "help")]
        public static bool Help(BotCommandArgs args)
        {

            args.Bot.SendMessage("Folgende Befehle sind verfügbar: !uptime, !hype, !telegram, !flipacoin, !donate, !teamspeak, !twitter, !youtube, !github, !time, !streamer, !projects, !whoami");
            return true;
        }

        [Command("telegram")]
        public static bool TelegramGroup(BotCommandArgs args)
        {
            args.Bot.SendMessage("Telegramgruppe: https://t.me/NoobDevStream | @gallimathias | @susch19");
            return true;
        }

        [Command("flipacoin")]
        public static bool FlipACoin(BotCommandArgs args)
        {
            Random r = new Random();
            if (r.Next(0, 2) == 0)
                args.Bot.SendMessage("Kopf");
            else
                args.Bot.SendMessage("Zahl");
            return true;
        }

        [Command("donate")]
        public static bool Donate(BotCommandArgs args)
        {
            args.Bot.SendMessage("Betterplace: https://goo.gl/QK5FF3");
            return true;
        }

        [Command("github")]
        public static bool Github(BotCommandArgs args)
        {
            //TODO Check current project
            args.Bot.SendMessage("Marcus: https://github.com/Gallimathias \r\n  susch19: https://github.com/susch19");
            return true;
        }

        [Command("teamspeak", "ts")]
        public static bool TeamSpeak(BotCommandArgs args)
        {
            args.Bot.SendMessage("Teamspeak (Klickbarer Link in der Beschreibung): ts3server://drachenfeste.eu/");
            return true;
        }

        [Command("twitter")]
        public static bool Twitter(BotCommandArgs args)
        {
            args.Bot.SendMessage("Twitter: https://twitter.com/Noob_Dev_Tv");
            return true;
        }

        [Command("youtube", "yt")]
        public static bool Youtube(BotCommandArgs args)
        {
            args.Bot.SendMessage("Youtube: https://www.youtube.com/channel/UCIWEvJ9SHMQoouIe86z6buQ");
            return true;
        }

        [Command("time")]
        public static bool Time(BotCommandArgs args)
        {
            args.Bot.SendMessage("Aktuelle Uhrzeit: " + DateTime.Now.ToLongTimeString());
            return true;
        }

        [Command("streamer")]
        public static bool Streamer(BotCommandArgs args)
        {
            args.Bot.SendMessage("Namen der Streamer: Marcus Aurelius, susch19");
            return true;
        }

        [Command("projects")]
        public static bool Project(BotCommandArgs args)
        {
            args.Bot.SendMessage("Für die aktuellen Projekte wirf bitte ein Blick in unsere Beschreibung. Alle Projekte sind auch auf !Github zu finden.");
            return true;
        }

        [Command("whoami")]
        public static bool WhoAmI(BotCommandArgs args)
        {
            return AsyncStuff(args).Result;
        }

        private static async Task<bool> AsyncStuff(BotCommandArgs args)
        {
            string result = "";
            var user = await args.TwitchAPI.Users.v5.GetUserByIDAsync(args.Message.UserId);

            if (user == null)
            {
                args.Bot.SendMessage($"Sorry {args.Message.Username}, aber WhoAmI scheint für dich nicht zu funktionieren :(");
                return false;
            }

            result += $"Hallo {user.DisplayName}. Du bist registriert seit {user.CreatedAt} und folgst uns ";

            var follow = args.Bot.FollowerService.CurrentFollowers.FirstOrDefault(x => x.User.Id == user.Id);
            if (follow == null)
            {
                result += "leider nicht :(.";
            }
            else
            {
                result += "seit dem " + follow.CreatedAt + ". Danke dafür :). ";

                if (follow.Notifications)
                    result += "Außerdem möchtest du benachrichtigt werden, wenn wir streamen :).";
                else
                    result += "Dich scheinen Benachrichtigungen in Form von Emails zu stören.";
            }

            args.Bot.SendMessage(result);
            return true;
        }
    }
}

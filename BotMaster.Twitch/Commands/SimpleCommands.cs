using BotMaster.MessageContract;
using BotMaster.Twitch;

using CommandManagementSystem.Attributes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotMaster.Twitch.Commands
{
    public static class SimpleCommands
    {
        static Random random;
        private static readonly string[] smilies;

        static SimpleCommands()
        {
            random = new Random();
            smilies = new string[] { ":)", ":D", "O_o", "B)", ":O", "<3", ";)", ":P", ";P", "R)" };
        }

        [Command("hype")]
        internal static void Hype(TwitchContext context, CommandMessage message)
        {
            string msg = "";

            for (int i = 0; i < 15; i++)
            {
                msg += smilies[random.Next(0, smilies.Length)] + " ";
            }

            context.Client.SendMessage(context.Channel, msg);
        }

        [Command("uptime")]
        internal static void Uptime(TwitchContext context, CommandMessage message) => UptimeAsync(context, message);

        [Command("?", "help")]
        internal static void Help(TwitchContext context, CommandMessage message)
        {
            var commandsList = context.CommandoCentral.Commands;
            string toPrintMessage = "Folgende Befehle sind verfügbar:";

            //context.Client.SendMessage(context.Channel, "Folgende Befehle sind verfügbar: !uptime, !hype, !telegram, !flipacoin, !donate, !teamspeak, !twitter, !youtube, !github, !time, !streamer, !projects, !whoami");
            foreach (var commandTags in commandsList)
                toPrintMessage += $" !{commandTags},";

            context.Client.SendMessage(context.Channel, toPrintMessage.Trim(',', ' '));

        }

        [Command("telegram")]
        internal static void TelegramGroup(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Telegramgruppe: https://t.me/NoobDevStream | @gallimathias | @susch19");

        }

        [Command("flipacoin")]
        internal static void FlipACoin(TwitchContext context, CommandMessage message)
        {
            if (random.Next(0, 2) == 0)
                context.Client.SendMessage(context.Channel, "Kopf");
            else
                context.Client.SendMessage(context.Channel, "Zahl");


        }

        [Command("donate")]
        internal static void Donate(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Betterplace: https://goo.gl/QK5FF3");

        }

        [Command("github")]
        internal static void Github(TwitchContext context, CommandMessage message)
        {
            //TODO Check current project
            context.Client.SendMessage(context.Channel, "Github: https://github.com/NoobDevTv");

        }

        internal static void Add(TwitchContext context, CommandMessage c)
        {
            var command = c.Parameter.First().ToLower();
            var message = " " + string.Join(" ", c.Parameter.Skip(1));
            context.AddCommand((command) => context.Client.SendMessage(context.Channel, message), command);
            context.Client.SendMessage(context.Channel, $"User {c.Username} has added command {command} with text: {message}");
            
        }

        [Command("teamspeak", "ts")]
        internal static void TeamSpeak(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Teamspeak (Klickbarer Link in der Beschreibung): ts3server://drachenfeste.eu/");

        }

        [Command("twitter")]
        internal static void Twitter(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Twitter: https://twitter.com/Noob_Dev_Tv");

        }

        [Command("youtube", "yt")]
        internal static void Youtube(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Youtube: https://www.youtube.com/channel/UCIWEvJ9SHMQoouIe86z6buQ");

        }

        internal static void Discord(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Discord: https://discord.gg/3UGVAfK");

        }

        [Command("time")]
        internal static void Time(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Aktuelle Uhrzeit: " + DateTime.Now.ToLongTimeString());

        }

        [Command("streamer")]
        internal static void Streamer(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Namen der Streamer: Marcus Aurelius, susch19");

        }

        [Command("projects")]
        internal static void Project(TwitchContext context, CommandMessage message)
        {
            context.Client.SendMessage(context.Channel, "Für die aktuellen Projekte wirf bitte ein Blick in unsere Beschreibung. Alle Projekte sind auch auf !Github zu finden.");

        }

        //[Command("whoami")]
        //internal static void WhoAmI(TwitchContext context, CommandMessage message)
        //{
        //    AsyncWhoAmI(context, message);
        //}

        //private static async Task<bool> AsyncWhoAmI(TwitchContext context, CommandMessage message)
        //{
        //    string result = "";
        //    var user = (await context.Api.V5.Users.GetUsersByNameAsync(new List<string> { context.UserId })).Users[0];

        //    if (user == null)
        //    {
        //        context.Client.SendMessage(context.Channel, $"Sorry {args.Message.Username}, aber WhoAmI scheint für dich nicht zu funktionieren :(");
        //        return false;
        //    }

        //    result += $"Hallo {user.DisplayName}. Du bist registriert seit {user.CreatedAt} und folgst uns ";

        //    var follow = (await context.Api.Helix.Users.GetUsersFollowsAsync(fromId: args.Message.UserId, toId: args.Bot.ChannelId)).Follows.FirstOrDefault();
        //    if (follow == default)
        //    {
        //        result += "leider nicht :(.";
        //    }
        //    else
        //    {
        //        result += "seit dem " + follow.FollowedAt + ". Danke dafür :). ";
        //    }

        //    context.Client.SendMessage(context.Channel, result);

        //}

        private static async Task UptimeAsync(TwitchContext context, CommandMessage message)
        {
            var uptime = await context.Api.V5.Streams.GetUptimeAsync(context.Channel); //Helix.Streams.GetStreamsAsync().

            if (uptime == null)
            {
                context.Client.SendMessage(context.Channel, "Irgendwas ist mit dem !uptime Befehl kaputt gegangen :(");

                return;
            }

            context.Client.SendMessage(context.Channel,
                $"Der Stream läuft schon {uptime.Value.Hours.ToString("d2")}:{uptime.Value.Minutes.ToString("d2")}:{uptime.Value.Seconds.ToString("d2")}");

        }
    }
}

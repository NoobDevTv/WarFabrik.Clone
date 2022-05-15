﻿using BotMaster.MessageContract;
using BotMaster.RightsManagement;
using BotMaster.Telegram.Database;
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

            SendMessage(context, message, msg);
        }

        [Command("uptime")]
        internal static void Uptime(TwitchContext context, CommandMessage message) => UptimeAsync(context, message);

        [Command("?", "help")]
        internal static void Help(TwitchContext context, CommandMessage message)
        {
            var commandsList = context.CommandoCentral.Commands;
            string toPrintMessage = "Folgende Befehle sind verfügbar:";

            //SendMessage(context, message, "Folgende Befehle sind verfügbar: !uptime, !hype, !telegram, !flipacoin, !donate, !teamspeak, !twitter, !youtube, !github, !time, !streamer, !projects, !whoami");
            foreach (var commandTags in commandsList)
                toPrintMessage += $" !{commandTags},";

            SendMessage(context, message, toPrintMessage.Trim(',', ' '));

        }

        [Command("telegram")]
        internal static void TelegramGroup(TwitchContext context, CommandMessage message)
        {
            SendMessage(context, message, "Telegramgruppe: https://t.me/NoobDevStream | @gallimathias | @susch19");

        }

        [Command("flipacoin")]
        internal static void FlipACoin(TwitchContext context, CommandMessage message)
        {
            if (random.Next(0, 2) == 0)
                SendMessage(context, message, "Kopf");
            else
                SendMessage(context, message, "Zahl");


        }

        [Command("donate")]
        internal static void Donate(TwitchContext context, CommandMessage message)
        {
            SendMessage(context, message, "Betterplace: https://goo.gl/QK5FF3");

        }

        [Command("github")]
        internal static void Github(TwitchContext context, CommandMessage message)
        {
            //TODO Check current project
            SendMessage(context, message, "Github: https://github.com/NoobDevTv");

        }
        internal static void Add(TwitchContext context, CommandMessage message)
        {
            var command = message.Parameter.First().ToLower();
            var text = " " + string.Join(" ", message.Parameter.Skip(1));
            context.AddCommand((command) => SendMessage(context, message, text), command);
            SendMessage(context,message, $"User {message.Username} has added command {command} with text: {text}");

        }
   

        internal static void Register(TwitchContext context, CommandMessage c)
        {

            using var cont = new RightsDbContext();
            var user = cont.PlattformUsers
                .FirstOrDefault(x => x.PlattformUserId == c.PlattformUserId
                    && x.Platform == c.SourcePlattform);

            if (user is not null)
            {
                if (c.Username != user.Name)
                    user.Name = c.Username;

                context.Client.SendWhisper(c.Username, $"You are already registered");
            }
            else
            {
                user = new() { Name = c.Username, Platform = c.SourcePlattform, PlattformUserId = c.PlattformUserId };
                cont.Add(user);
                context.Client.SendWhisper(c.Username, $"Welcome you are now registered :)");
            }
            cont.SaveChanges();
        }

        internal static void PublicConnect(TwitchContext context, CommandMessage c)
        {
            context.Client.SendWhisper(c.Username, "Connection of two accounts will be added in the future :)");
        }

        internal static void PrivateConnect(TwitchContext context, CommandMessage c)
        {
            context.Client.SendWhisper(c.Username, "Connection of two accounts will be added in the future, here you will see a connection code or can enter one :)");
        }

        [Command("teamspeak", "ts")]
        internal static void TeamSpeak(TwitchContext context, CommandMessage message)
        {

            SendMessage(context, message, "Teamspeak (Klickbarer Link in der Beschreibung): ts3server://drachenfeste.eu/");

        }

        [Command("twitter")]
        internal static void Twitter(TwitchContext context, CommandMessage message)
        {

            SendMessage(context, message, "Twitter: https://twitter.com/Noob_Dev_Tv");

        }

        [Command("youtube", "yt")]
        internal static void Youtube(TwitchContext context, CommandMessage message)
        {
            SendMessage(context, message, "Youtube: https://www.youtube.com/channel/UCIWEvJ9SHMQoouIe86z6buQ");

        }

        internal static void Discord(TwitchContext context, CommandMessage message)
        {
            SendMessage(context, message, "Discord: https://discord.gg/3UGVAfK");

        }

        [Command("time")]
        internal static void Time(TwitchContext context, CommandMessage message)
        {
            SendMessage(context, message, "Aktuelle Uhrzeit: " + DateTime.Now.ToLongTimeString());

        }

        [Command("streamer")]
        internal static void Streamer(TwitchContext context, CommandMessage message)
        {
            SendMessage(context, message, "Namen der Streamer: Marcus Aurelius, susch19");

        }

        [Command("projects")]
        internal static void Project(TwitchContext context, CommandMessage message)
        {
            SendMessage(context, message, "Für die aktuellen Projekte wirf bitte ein Blick in unsere Beschreibung. Alle Projekte sind auch auf !Github zu finden.");

        }


        private static void SendMessage(TwitchContext context, CommandMessage message, string text)
        {

            if (message.Secure)
                context.Client.SendWhisper(message.Username, text);
            else
                context.Client.SendMessage(context.Channel, text);
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
        //        SendMessage(context.Channel, $"Sorry {args.Message.Username}, aber WhoAmI scheint für dich nicht zu funktionieren :(");
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

        //    SendMessage(context.Channel, result);

        //}

        private static async Task UptimeAsync(TwitchContext context, CommandMessage message)
        {
            var uptime = await context.Api.V5.Streams.GetUptimeAsync(context.Channel); //Helix.Streams.GetStreamsAsync().

            if (uptime == null)
            {
                SendMessage(context, message, "Irgendwas ist mit dem !uptime Befehl kaputt gegangen :(");

                return;
            }

            SendMessage(context, message,
                $"Der Stream läuft schon {uptime.Value.Hours.ToString("d2")}:{uptime.Value.Minutes.ToString("d2")}:{uptime.Value.Seconds.ToString("d2")}");

        }
    }
}

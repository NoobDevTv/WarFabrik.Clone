using BotMaster.Commandos;
using BotMaster.MessageContract;
using BotMaster.RightsManagement;

namespace BotMaster.YouTube.Commands
{
    public static class SimpleCommands
    {
        static readonly Random random;
        private static readonly string[] smilies;

        static SimpleCommands()
        {
            random = new Random();
            smilies = new string[] { ":)", ":D", "O_o", "B)", ":O", "<3", ";)", ":P", ";P", "R)" };
        }

        internal static void Hype(YoutubeContext context, CommandMessage message)
        {
            string msg = "";

            for (int i = 0; i < 15; i++)
            {
                msg += smilies[random.Next(0, smilies.Length)] + " ";
            }

            SendMessage(context, message, msg);
        }

        internal static void Uptime(YoutubeContext context, CommandMessage message)
        {
            if (context.Client.CurrentBroadcast is null || !context.Client.CurrentBroadcast.ActualStartTime.HasValue)
                return;
            var runsSince = DateTime.Now.Subtract(context.Client.CurrentBroadcast.ActualStartTime.Value);
            SendMessage(context, message,
                  $"Der Stream läuft schon seit {runsSince.Hours:d2}:{runsSince.Minutes:d2}:{runsSince.Seconds:d2}");
        }

        internal static void Help(YoutubeContext context, CommandMessage message)
        {
            var commandsList = context.CommandoCentral.Commands;
            string toPrintMessage = "Folgende Befehle sind verfügbar:";

            //SendMessage(context, message, "Folgende Befehle sind verfügbar: !uptime, !hype, !telegram, !flipacoin, !donate, !teamspeak, !twitter, !youtube, !github, !time, !streamer, !projects, !whoami");
            foreach (var commandTags in commandsList)
                toPrintMessage += $" !{commandTags.Command},";

            SendMessage(context, message, toPrintMessage.Trim(',', ' '));

        }

        internal static void TelegramGroup(YoutubeContext context, CommandMessage message)
        {
            SendMessage(context, message, "Telegramgruppe: https://t.me/NoobDevStream | @gallimathias | @susch19");

        }

        internal static void FlipACoin(YoutubeContext context, CommandMessage message)
        {
            if (random.Next(0, 2) == 0)
                SendMessage(context, message, "Kopf");
            else
                SendMessage(context, message, "Zahl");
        }

        internal static void Donate(YoutubeContext context, CommandMessage message)
        {
            SendMessage(context, message, "Betterplace: https://goo.gl/QK5FF3");

        }

        internal static void Github(YoutubeContext context, CommandMessage message)
        {
            //TODO Check current project
            SendMessage(context, message, "Github: https://github.com/NoobDevTv");
        }

        internal static void Add(YoutubeContext context, CommandMessage message, bool global = false)
        {
            var toAddCommand = message.Parameter.First().ToLower();
            var text = " " + string.Join(" ", message.Parameter.Skip(1));

            context.AddCommand((command) => SendMessage(context, message, text, command), toAddCommand);

            SendMessage(context, message, $"User {message.Username} has added command {toAddCommand} with text: {text}");
            using var ctx = new CommandosDbContext();
            var existing = ctx.Commands.FirstOrDefault(x => x.Command == toAddCommand);
            if (existing is null)
            {
                ctx.Commands.Add(new()
                {
                    Secure = message.Secure,
                    Text = text,
                    Command = toAddCommand,
                    Target = context.Channel.Id,
                    Global = global
                });

                ctx.SaveChanges();
            }

        }

        internal static void SendTextCommand(CommandMessage commandMessage, PersistentCommand command, YoutubeContext context)
        {
            if (command.Secure && (!commandMessage.Secure || command.Target != commandMessage.Username))
                return;
            SendMessage(context, commandMessage, command.Text);
        }

        internal static void PublicConnect(YoutubeContext context, CommandMessage message)
        {
            if (message.Parameter.Count > 0)
            {
                if (UserConnectionService.EndConnection(message.PlattformUserId, message.Parameter.First()))
                    context.Client.SendMessage($"You have connected successfully");
                else
                    context.Client.SendMessage($"You have connected unsuccessfully, did you try to connect to the same plattform or did you already link these plattforms?");

            }

        }

        internal static void TeamSpeak(YoutubeContext context, CommandMessage message)
        {

            SendMessage(context, message, "Teamspeak (Klickbarer Link in der Beschreibung): ts3server://drachenfeste.eu/");

        }

        internal static void Twitter(YoutubeContext context, CommandMessage message)
        {

            SendMessage(context, message, "Twitter: https://twitter.com/Noob_Dev_Tv");

        }

        internal static void Twitch(YoutubeContext context, CommandMessage message)
        {
            SendMessage(context, message, $"{nameof(Twitch)}: https://twitch.tv/noobdevtv");
        }

        internal static void Discord(YoutubeContext context, CommandMessage message)
        {
            SendMessage(context, message, "Discord: https://discord.gg/3UGVAfK");
        }

        internal static void Time(YoutubeContext context, CommandMessage message)
        {
            SendMessage(context, message, "Aktuelle Uhrzeit: " + DateTime.Now.ToLongTimeString());
        }

        internal static void Streamer(YoutubeContext context, CommandMessage message)
        {
            SendMessage(context, message, "Namen der Streamer: Marcus Aurelius, susch19");
        }

        internal static void Project(YoutubeContext context, CommandMessage message)
        {
            SendMessage(context, message, "Für die aktuellen Projekte wirf bitte ein Blick in unsere Beschreibung. Alle Projekte sind auch auf !Github zu finden.");

        }


        private static void SendMessage(YoutubeContext context, CommandMessage toAddMessage, string text, CommandMessage incommingMessage)
        {
            if (toAddMessage.Secure && (!incommingMessage.Secure || toAddMessage.Username != incommingMessage.Username))
                return;
            SendMessage(context, toAddMessage, text);
        }

        private static void SendMessage(YoutubeContext context, CommandMessage message, string text)
        {

            context.Client.SendMessage(text);
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

        //private static async Task UptimeAsync(YoutubeContext context, CommandMessage message)
        //{
        //    var uptime = await context.Api.V5.Streams.GetUptimeAsync(context.Channel); //Helix.Streams.GetStreamsAsync().

        //    if (uptime == null)
        //    {
        //        SendMessage(context, message, "Irgendwas ist mit dem !uptime Befehl kaputt gegangen :(");

        //        return;
        //    }

        //    SendMessage(context, message,
        //        $"Der Stream läuft schon {uptime.Value.Hours.ToString("d2")}:{uptime.Value.Minutes.ToString("d2")}:{uptime.Value.Seconds.ToString("d2")}");

        //}
    }
}

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NoobDevBot.Telegram
{
    public class TelegramBot
    {
        private readonly Logger logger;
        private TelegramBotClient bot;
        private TelegramCommandManager manager;

        public TelegramBot()
        {
            logger = LogManager.GetCurrentClassLogger();

            var info = new FileInfo(Path.Combine(".", "additionalfiles", "Telegram_Token.txt"));

            if (!info.Directory.Exists)
                info.Directory.Create();

            bot = new TelegramBotClient(System.IO.File.ReadAllText(info.FullName));
            manager = new TelegramCommandManager();
            DatabaseManager.Initialize();
            //DatabaseManager.CreateGroup("NoobDev");
            bot.OnMessage += BotOnMessage;
            bot.StartReceiving();
        }

        private void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type != MessageType.Text)
                return;

            if (message.Text[0] == '/')
            {
                //Command
                //TODO Get real end of command
                var command = message.Text.Substring(message.Text.IndexOf('/') + 1, message.Text.Length - 1).ToLower();

                manager.DispatchAsync(command, new TelegramCommandArgs(e.Message, bot));
                return;
            }

            var user = DatabaseManager.InsertUserIfNotExist(e.Message.From, e.Message);

            //New Member(s) for the Databse
            if (e.Message.NewChatMembers != null)
            {
                foreach (var chatMember in e.Message.NewChatMembers)
                    DatabaseManager.InsertUserIfNotExist(chatMember, message);
            }

            if (e.Message.NewChatMembers?.Length > 0)
                foreach (var item in e.Message.NewChatMembers)
                    DatabaseManager.InsertUserIfNotExist(item, message);
        }

        public void Exit()
        {
            bot.StopReceiving();
        }

        public void SendMessageToGroup(string groupName, string message)
        {
            var group = DatabaseManager.GetGroupByName(groupName);

            try
            {
                group.Member.ForEach(async x => await bot.SendTextMessageAsync(x.ChatId, message));
            }
            catch (Exception)
            {
            }
        }
    }
}

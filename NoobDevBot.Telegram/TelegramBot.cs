using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NoobDevBot.Telegram
{
    public class TelegramBot
    {
        private TelegramBotClient bot;
        private TelegramCommandManager manager;

        public TelegramBot()
        {
            bot = new TelegramBotClient(System.IO.File.ReadAllText(@".\Telegram_Token.txt"));
            manager = new TelegramCommandManager();
            DatabaseManager.Initialize();
            
            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
        }

        private void Bot_OnMessage(object sender, global::Telegram.Bot.Args.MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type != MessageType.TextMessage)
                return;

            if (message.Text[0] == '/')
            {
                //Command
                //TODO Get real end of command
                var command = message.Text.Substring(message.Text.IndexOf('/'), message.Text.Length - 1).ToLower();

                manager.DispatchAsync(command, new TelegramCommandArgs(e.Message, bot));
                return;
            }

            var user = DatabaseManager.InsertUserIfNotExist(e.Message.From, e.Message);

            //New Member(s) for the Databse
            if (e.Message.NewChatMember != null)
                DatabaseManager.InsertUserIfNotExist(e.Message.NewChatMember, message);

            if (e.Message.NewChatMembers?.Length > 0)
                foreach (var item in e.Message.NewChatMembers)
                    DatabaseManager.InsertUserIfNotExist(item, message);
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

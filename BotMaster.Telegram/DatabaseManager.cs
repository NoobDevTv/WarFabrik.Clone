using BotMaster.Telegram.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoobDevBot.Telegram.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NoobDevBot.Telegram
{
    internal static class DatabaseManager
    {
        public static TelegramUser InsertUserIfNotExist(User user, Message message)
        {
            if (message.Chat.Type == ChatType.Group)
                return InsertUserFromGroupIfNotExist(user, message);

            if (message.Chat.Type != ChatType.Private)
                return null;

            var tempUser = Context.User.FirstOrDefault(u => u.Id == user.Id);

            if (tempUser == null)
            {
                tempUser = SaveNewUser(user, message.Chat.Id);
                //PutUserInGroup(GetGroupByName("NoobDev"), tempUser);
                Submit();
            }

            return tempUser;
        }

        private static TelegramUser InsertUserFromGroupIfNotExist(User user, Message message)
        {
            var tempUser = Context.User.FirstOrDefault(u => u.Id == user.Id);
            var group = Context.GroupChat.FirstOrDefault(x => x.ChatId == message.Chat.Id);

            if (group == null)
            {
                Context.GroupChat.Add(new GroupChat { ChatId = message.Chat.Id, Name = message.Chat.Title });
                Submit();
                group = Context.GroupChat.FirstOrDefault(x => x.ChatId == message.Chat.Id);
            }

            if (tempUser == null)
            {
                tempUser = SaveNewUser(user);
                Submit();
            }

            Context.GroupChat_User.Add(new GroupChatUser { ChatId = group.ChatId, UserId = tempUser.Id });

            return tempUser;
        }

       

        private static TelegramUser SaveNewUser(User user, ChatId chatId = null)
        {
            var table = Context.User;

            var tempUser = new TelegramUser
            {
                Id = user.Id,
                ChatId = chatId?.Identifier ?? 0,
                Name = string.IsNullOrWhiteSpace(user.Username) ? user.FirstName : user.Username,
            };

            table.Add(tempUser);
            return tempUser;
        }

      

        internal static Model.Group GetGroupByName(string groupName)
        {
            var group = Context.Groups.FirstOrDefault(x => x.Name == groupName);

            if (group == null)
                return null;

            Context.User.Load();
            return group;
        }


        public static void Submit() => Context.SaveChanges();

    }

}
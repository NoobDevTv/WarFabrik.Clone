using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoobDevBot.Telegram.DatabaseModels;
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
        public static TelegramBotContext Context
        {
            get
            {
                lock (mainLock)
                    return context;
            }
        }

        private static TelegramBotContext context;

        private static object mainLock;

        internal static void Initialize()
        {
            mainLock = new object();
            context = new TelegramBotContext();
            Context.Database.EnsureCreated();
        }

        public static void PutUserInGroup(DatabaseModels.Group group, NoobUser user)
        {
            var gu = Context.Group_User.FirstOrDefault(x => x.GroupId == group.Id && x.UserId == user.Id);
            if (gu == null)
                Context.Group_User.Add(new Group_User { Id = Context.Group_User.Count() + 1, GroupId = group.Id, UserId = user.Id });
            Submit();
        }

        public static bool UserExists(int id) => Context.User.Any(u => u.Id == id);

        public static NoobUser InsertUserIfNotExist(User user, Message message)
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

        private static NoobUser InsertUserFromGroupIfNotExist(User user, Message message)
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

            Context.GroupChat_User.Add(new GroupChat_User { ChatId = group.ChatId, UserId = tempUser.Id });

            return tempUser;
        }

        internal static void DeleteUser(int id)
        {
            //TODO Delete every remain of a user in our database
            var user = Context.User.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return;

            Context.Remove(user);
            Submit();

        }

        private static NoobUser SaveNewUser(User user, ChatId chatId = null)
        {
            var table = Context.User;

            var tempUser = new NoobUser
            {
                Id = user.Id,
                ChatId = chatId?.Identifier ?? 0,
                Name = string.IsNullOrWhiteSpace(user.Username) ? user.FirstName : user.Username,
            };

            table.Add(tempUser);
            return tempUser;
        }

        public static Stream InsertNewStream(int user, DateTime dateTime, string name)
        {
            var table = Context.Streams;

            var tempStream = new Stream
            {
                UserId = user,
                Start = dateTime,
                Title = name
            };

            table.Add(tempStream);
            Submit();
            GetNextStream();
            return table.FirstOrDefault(s => s.UserId == user && s.Start == dateTime && s.Title == name);
        }

        internal static DatabaseModels.Group GetGroupByName(string groupName)
        {
            var group = Context.Groups.FirstOrDefault(x => x.Name == groupName);

            if (group == null)
                return null;

            Context.User.Load();
            return group;
        }

        internal static List<DatabaseModels.Group> GetGroupsFromUser(NoobUser user)
        {
            var usergroups = Context.Group_User.Where(x => x.UserId == user.Id).ToList();

            var groups = Context.Groups.Where(x => usergroups.FirstOrDefault(y => y.GroupId == x.Id) != null).ToList();

            return groups;
        }


        internal static void InsertNewSmiley(string smiley)
        {
            //context.GetTable<smilies>().InsertOnSubmit(new smilies { unicode = smiley});
        }

        public static bool DeleteStream(int user, int id)
        {
            var table = Context.Streams;
            var stream = table.FirstOrDefault(s => s.Id == id && s.UserId == user);

            if (stream == null)
                return false;

            table.Remove(stream);
            Submit();
            return true;
        }

        public static Stream GetNextStream()
        {
            var table = Context.Streams;

            return table?.Where(s => s.Start > DateTime.UtcNow)?.OrderBy(s => s.Start)?.FirstOrDefault();
        }

        public static Stream GetStreamById(int id)
        {
            var table = Context.Streams;

            return table?.Where(s => s.Id == id)?.FirstOrDefault();
        }

        public static List<Stream> GetUserStreams(int id) => Context.Streams.Where(x => x.UserId == id).ToList();

        public static NoobUser GetUserById(int id) => Context.User.FirstOrDefault(u => u.Id == id);
        public static NoobUser GetUserByName(string name) => Context.User.First(u => u.Name == name);

        public static DatabaseModels.Group CreateGroup(string name)
        {
            var group = Context.Groups.FirstOrDefault(x => x.Name == name);
            if (group != null)
                return group;
            Context.Groups.Add(new DatabaseModels.Group { Id = Context.Groups.Count() + 1, Name = name });
            Submit();
            return Context.Groups.FirstOrDefault(x => x.Name == name);
        }

        public static void Submit() => Context.SaveChanges();

        //public static List<groups_relation> GetGroups(int user_id) =>
        //    context.GetTable<groups_relation>().Where(r => r.user_id == user_id).ToList();

        //public static byte? GetPower(string id, int reference, bool is_Group) =>
        //    context.GetTable<rights_relation>().FirstOrDefault(
        //        r => r.is_group == is_Group && r.reference == reference && r.right_id == id)?.power;

        //public static string GetSmiley(int id)
        //{
        //    //Von TheBlubb14 
        //    return char.ConvertFromUtf32(int.Parse(
        //        context.GetTable<smilies>().FirstOrDefault(s => s.id == id).unicode,
        //        NumberStyles.HexNumber));
        //}

        //public static int GetNumOfSmilies() => context.GetTable<smilies>().Count();
    }

}
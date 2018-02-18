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
    internal class DatabaseManager
    {
        static TelegramBotContext context;
        internal static void Initialize()
        {
            context = new TelegramBotContext();

            context.Database.EnsureCreated();
        }

        public static void PutUserInGroup(DatabaseModels.Group group, NoobUser user)
        {
            var gu = context.Group_User.FirstOrDefault(x => x.GroupId == group.Id && x.UserId == user.Id);
            if (gu == null)
                context.Group_User.Add(new Group_User { Id = context.Group_User.Count() + 1, GroupId = group.Id, UserId = user.Id });
            Submit();
        }

        public static bool UserExists(int id) => context.User.Any(u => u.Id == id);

        public static NoobUser InsertUserIfNotExist(User user, Message message)
        {
            if (message.Chat.Type == ChatType.Group)
                return InsertUserFromGroupIfNotExist(user, message);

            if (message.Chat.Type != ChatType.Private)
                return null;

            var tempUser = context.User.FirstOrDefault(u => u.Id == user.Id);

            if (tempUser == null)
            {
                tempUser = SaveNewUser(user, message.Chat.Id);
                Submit();
            }

            return tempUser;
        }

        private static NoobUser InsertUserFromGroupIfNotExist(User user, Message message)
        {
            var tempUser = context.User.FirstOrDefault(u => u.Id == user.Id);
            var group = context.GroupChat.FirstOrDefault(x => x.ChatId == message.Chat.Id);

            if (group == null)
            {
                context.GroupChat.Add(new GroupChat { ChatId = message.Chat.Id, Name = message.Chat.Title });
                Submit();
                group = context.GroupChat.FirstOrDefault(x => x.ChatId == message.Chat.Id);
            }

            if (tempUser == null)
            {
                tempUser = SaveNewUser(user);
                Submit();
            }

            context.GroupChat_User.Add(new GroupChat_User { ChatId = group.ChatId, UserId = tempUser.Id });

            return tempUser;
        }

        internal static void DeleteUser(int id)
        {
//TODO Delete every remain of a user in our database
            var user = context.User.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return;

            context.Remove(user);
            Submit();

        }

        private static NoobUser SaveNewUser(User user, ChatId chatId = null)
        {
            var table = context.User;

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
            var table = context.Streams;

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
            var group = context.Groups.FirstOrDefault(x => x.Name == groupName);

            if (group == null)
                return null;

            var usergroup = context.Group_User.Where(x => x.GroupId == group.Id).ToList();

            foreach (var item in usergroup)
                group.Member.Add(context.User.FirstOrDefault(x => x.Id == item.UserId));

            return group;
        }

        internal static List<DatabaseModels.Group> GetGroupsFromUser(NoobUser user)
        {
            var usergroups = context.Group_User.Where(x => x.UserId== user.Id).ToList();

            var groups = context.Groups.Where(x => usergroups.FirstOrDefault(y=>y.GroupId == x.Id) != null).ToList();

            return groups;
        }


        internal static void InsertNewSmiley(string smiley)
        {
            //context.GetTable<smilies>().InsertOnSubmit(new smilies { unicode = smiley});
        }

        public static bool DeleteStream(int user, int id)
        {
            var table = context.Streams;
            var stream = table.FirstOrDefault(s => s.Id == id && s.UserId == user);

            if (stream == null)
                return false;

            table.Remove(stream);
            Submit();
            return true;
        }

        public static Stream GetNextStream()
        {
            var table = context.Streams;

            return table?.Where(s => s.Start > DateTime.UtcNow)?.OrderBy(s => s.Start)?.FirstOrDefault();
        }

        public static Stream GetStreamById(int id)
        {
            var table = context.Streams;

            return table?.Where(s => s.Id == id)?.FirstOrDefault();
        }

        public static List<Stream> GetUserStreams(int id) => context.Streams.Where(x => x.UserId == id).ToList();

        public static NoobUser GetUserById(int id) => context.User.First(u => u.Id == id);
        public static NoobUser GetUserByName(string name) => context.User.First(u => u.Name == name);

        public static DatabaseModels.Group CreateGroup(string name)
        {
            var group = context.Groups.FirstOrDefault(x => x.Name == name);
            if (group != null)
                return group;
            context.Groups.Add(new DatabaseModels.Group { Id = context.Groups.Count() + 1, Name = name });
            Submit();
            return context.Groups.FirstOrDefault(x => x.Name == name);
        }

        public static void Submit() => context.SaveChanges();

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
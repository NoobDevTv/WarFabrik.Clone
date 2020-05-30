using System;
using System.Collections.Generic;
using System.Text;

namespace NoobDevBot.Telegram.DatabaseModels
{
    public class GroupChat
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string JoinLink { get; set; }
        public string Name { get; set; }

        public List<NoobUser> Member;

        public GroupChat()
        {
            Member = new List<NoobUser>();
        }
    }
}

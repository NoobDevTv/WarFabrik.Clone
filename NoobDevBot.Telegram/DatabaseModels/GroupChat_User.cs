using System;
using System.Collections.Generic;
using System.Text;

namespace NoobDevBot.Telegram.DatabaseModels
{
    public class GroupChat_User
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
    }
}

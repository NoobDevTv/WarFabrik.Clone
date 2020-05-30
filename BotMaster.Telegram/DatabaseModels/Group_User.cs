using System;
using System.Collections.Generic;
using System.Text;

namespace NoobDevBot.Telegram.DatabaseModels
{
    public class Group_User
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}

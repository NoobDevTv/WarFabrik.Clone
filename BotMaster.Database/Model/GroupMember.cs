using System;
using System.Collections.Generic;
using System.Text;

namespace BotMaster.Database.Model
{
    public sealed class GroupMember : Entity
    {
        public User User { get; set; }
        public Group Group { get; set; }
    }
}

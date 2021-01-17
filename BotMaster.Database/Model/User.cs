using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BotMaster.Database.Model
{
    public class User : IdEntity<int>
    {
        public string Name { get; set; }

        [InverseProperty(nameof(GroupMember.User))]
        public virtual List<GroupMember> GroupMembers { get; set; }

        [NotMapped]
        public IEnumerable<Group> Groups => GroupMembers.Select(g => g.Group);
    }
}

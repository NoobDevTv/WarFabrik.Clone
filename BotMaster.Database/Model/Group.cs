using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace BotMaster.Database.Model
{
    public class Group : IdEntity<int>
    {
        public string Name { get; set; }

        [InverseProperty(nameof(GroupMember.Group))]
        public virtual List<GroupMember> GroupMembers { get; set; }

        [NotMapped]
        public IEnumerable<User> User => GroupMembers.Select(m => m.User);

    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.Database.Model
{
    public class Group : IdEntity<int>
    {
        public string Name { get; set; }

        [InverseProperty(nameof(GroupMember.Group))]
        public virtual List<GroupMember> GroupMembers { get; set; }

        [NotMapped]
        public IEnumerable<User> Users => GroupMembers.Select(m => m.User);

    }
}

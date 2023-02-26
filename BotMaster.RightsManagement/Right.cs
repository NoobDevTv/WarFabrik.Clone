using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Rights")]
public class Right : IdEntity<int>, ICloneableGeneric<Right>
{
    public string Name { get; set; }

    [InverseProperty(nameof(PlattformUser.Rights))]
    public virtual ICollection<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
    [InverseProperty(nameof(Group.Rights))]
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    [InverseProperty(nameof(User.Rights))]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public Right Clone()
    {
        return new Right { Name = Name, Id = Id, Groups = Groups, PlattformUsers = PlattformUsers, Users = Users };
    }
}
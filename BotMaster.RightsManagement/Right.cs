using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Rights")]
public class Right : IdEntity<int>, ICloneableGeneric<Right>
{
    public string Name { get; set; }

    [InverseProperty(nameof(PlattformUser.Rights))]
    public virtual List<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
    [InverseProperty(nameof(Group.Rights))]
    public virtual List<Group> Groups { get; set; } = new List<Group>();
    [InverseProperty(nameof(User.Rights))]
    public virtual List<User> Users { get; set; } = new List<User>();

    public Right Clone()
    {
        return new Right { Name = Name, Id = Id, Groups = Groups.ToList(), PlattformUsers = PlattformUsers.ToList(), Users = Users.ToList() };
    }
}
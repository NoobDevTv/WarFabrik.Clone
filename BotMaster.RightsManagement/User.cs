using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Users")]
public class User : IdEntity<int>, ICloneableGeneric<User>
{
    public string DisplayName { get; set; }

    [InverseProperty(nameof(PlattformUser.User))]
    public virtual List<PlattformUser>? PlatformIdentities { get; set; } = new List<PlattformUser>();
    [InverseProperty(nameof(Group.Users))]
    public virtual List<Group>? Groups { get; set; } = new List<Group>();
    [InverseProperty(nameof(Right.Users))]
    public virtual List<Right>? Rights { get; set; } = new List<Right>();

    public User Clone()
    {
        return new User()
        {
            DisplayName = DisplayName,
            PlatformIdentities = PlatformIdentities?.ToList(),
            Groups = Groups?.ToList(),
            Rights = Rights?.ToList(),
            Id = Id
        };
    }
}

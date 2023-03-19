using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Groups")]
public class Group : IdEntity<int>, ICloneableGeneric<Group>
{
    public bool IsDefault { get; set; }
    public string Name { get; set; }

    [InverseProperty(nameof(User.Groups))]
    public virtual List<User> Users { get; set; } = new List<User>();
    [InverseProperty(nameof(PlattformUser.Groups))]
    public virtual List<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
    [InverseProperty(nameof(Right.Groups))]
    public virtual List<Right> Rights { get; set; } = new List<Right>();

    public Group Clone()
    {
        return new Group {Id = Id, IsDefault = IsDefault, Name = Name, Users = Users.ToList(), PlattformUsers = PlattformUsers.ToList(), Rights = Rights.ToList() };
    }
}

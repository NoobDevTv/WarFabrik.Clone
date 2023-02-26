using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Groups")]
public class Group : IdEntity<int>, ICloneableGeneric<Group>
{
    public bool IsDefault { get; set; }
    public string Name { get; set; }

    [InverseProperty(nameof(User.Groups))]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    [InverseProperty(nameof(PlattformUser.Groups))]
    public virtual ICollection<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
    [InverseProperty(nameof(Right.Groups))]
    public virtual ICollection<Right> Rights { get; set; } = new List<Right>();

    public Group Clone()
    {
        return new Group {Id = Id, IsDefault = IsDefault, Name = Name, Users = Users, PlattformUsers = PlattformUsers, Rights = Rights };
    }
}

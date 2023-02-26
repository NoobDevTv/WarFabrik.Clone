using BotMaster.Database;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("PlattformUsers")]
public class PlattformUser : IdEntity<int>, ICloneableGeneric<PlattformUser>
{
    [Required]
    public string Platform { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string PlattformUserId { get; set; }

    public virtual User? User { get; set; }
    
    [InverseProperty(nameof(Right.PlattformUsers))]
    public virtual ICollection<Right> Rights { get; set; } = new List<Right>();

    [InverseProperty(nameof(Group.PlattformUsers))]
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public PlattformUser Clone()
    {
        return new PlattformUser { Id = Id, Platform = Platform, Name = Name, PlattformUserId = PlattformUserId, User = User, Rights = Rights, Groups = Groups };
    }
}

using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Rights")]
public class Right : IdEntity<int>
{
    public string Name { get; set; }

    public virtual ICollection<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
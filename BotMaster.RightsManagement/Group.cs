using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Groups")]
public class Group : IdEntity<int>
{
    public bool IsDefault { get; set; }
    public string Name { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
    public virtual ICollection<Right> Rights { get; set; } = new List<Right>();
}

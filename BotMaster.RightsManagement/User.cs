using BotMaster.Database;

using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

[Table("Users")]
public class User : IdEntity<int>
{
    public string DisplayName { get; set; }

    public virtual ICollection<PlattformUser> PlatformIdentities { get; set; } = new List<PlattformUser>();
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    public virtual ICollection<Right> Rights { get; set; } = new List<Right>();

}

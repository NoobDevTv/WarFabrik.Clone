using BotMaster.Database;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.RightsManagement;

public class Right : Entity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Name { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
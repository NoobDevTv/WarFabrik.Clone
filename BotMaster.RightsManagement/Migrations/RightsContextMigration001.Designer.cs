using BotMaster.Database;
using BotMaster.Database.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.RightsManagement.Migrations;

#nullable enable
public partial class RightsContextMigration001 : IAutoMigrationTypeProvider
{
    //2022_10_23-20_39_01-RightsDbContext-InitialMigration
    //2022_10_23-20_39_01-RightsDbContext-InitialMigration
    public const string Id = $"2022_10_23-20_39_01-{nameof(RightsDbContext)}-InitialMigration";
    public IReadOnlyList<Type> GetEntityTypes()
    {
        return new[]
        {
            typeof(Group),
            typeof(User),
            typeof(PlattformUser),
            typeof(Right),

        };
    }

    [Table("Groups")]
    public class Group : IdEntity<int>
    {
        public bool IsDefault { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
        public virtual ICollection<Right> Rights { get; set; } = new List<Right>();
    }
    [Table("Users")]
    public class User : IdEntity<int>
    {
        public string DisplayName { get; set; }

        public virtual ICollection<PlattformUser> PlatformIdentities { get; set; } = new List<PlattformUser>();
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public virtual ICollection<Right> Rights { get; set; } = new List<Right>();

    }
    [Table("PlattformUsers")]
    public class PlattformUser : IdEntity<int>
    {


        [Required]
        public string Platform { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PlattformUserId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Right> Rights { get; set; } = new List<Right>();
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    }

    [Table("Rights")]
    public class Right : IdEntity<int>
    {
        public string Name { get; set; }

        public virtual ICollection<PlattformUser> PlattformUsers { get; set; } = new List<PlattformUser>();
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}

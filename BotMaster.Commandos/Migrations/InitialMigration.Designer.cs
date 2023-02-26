using BotMaster.Database;
using BotMaster.Database.Migrations;


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.Commandos.Migrations;


#nullable enable
public partial class InitialMigration : IAutoMigrationTypeProvider
{
    public const string Id = $"2022_11_06-20_39_01-{nameof(CommandosDbContext)}-InitialMigration";
    public IReadOnlyList<Type> GetEntityTypes()
    {
        return new Type[]
        {
            typeof(PersistentCommandClass)

        };
    }

    [Table("PersistentCommands")]
    public class PersistentCommandClass : IdEntity<int>
    {
        public string? Target { get; set; }
        public string Command { get; set; }
        public string Text { get; set; }
        public bool Secure { get; set; }
        public bool Global { get; set; }

        public string Plattforms { get; set; }

    }

}

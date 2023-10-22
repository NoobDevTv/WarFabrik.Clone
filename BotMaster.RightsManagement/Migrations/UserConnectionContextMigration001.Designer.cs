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
public partial class UserConnectionContextMigration001 : IAutoMigrationTypeProvider
{
    public const string Id = $"2022_10_23-20_39_01-{nameof(UserConnectionContext)}-InitialMigration";
    public IReadOnlyList<Type> GetEntityTypes()
    {
        return new[]
        {
            typeof(UserConnection),
        };
    }

    [Table("UserConnections")]
    public class UserConnection : IdEntity<int>
    {
        public string ConnectionCode { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool Connected { get; set; }

        public int PlattformUserId { get; set; }

    }
   
}

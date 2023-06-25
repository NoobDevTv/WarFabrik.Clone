using BotMaster.Database.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BotMaster.RightsManagement.Migrations;

[Migration(Id)]
[DbContext(typeof(RightsDbContext))]
public partial class RightsContextMigration001 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.SetUpgradeOperations(this);
    }
}

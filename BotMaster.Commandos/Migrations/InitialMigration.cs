using BotMaster.Database.Migrations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BotMaster.Commandos.Migrations;

[Migration(Id)]
[DbContext(typeof(CommandosDbContext))]
public partial class InitialMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.SetUpgradeOperations(this);
    }
}

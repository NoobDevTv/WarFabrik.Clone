using BotMaster.Database.Migrations;
using BotMaster.Telegram.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotMaster.RightsManagement.Migrations;

[Migration(Id)]
[DbContext(typeof(RightsDbContext))]
public partial class InitialMigration : Migration
{
    public InitialMigration()
    {
        ;
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        //TODO: Context
        using var ctx = new RightsDbContext();
        migrationBuilder.SetUpgradeOperations(this, ctx);
    }

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        base.BuildTargetModel(modelBuilder);
    }
}

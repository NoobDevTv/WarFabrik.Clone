using BotMaster.Database.Migrations;

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
[DbContext(typeof(UserConnectionContext))]
public partial class UserConnectionContextMigration001 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.SetUpgradeOperations(this);
        migrationBuilder.AddForeignKey("FK_UserConnections_PlattformUsers_PlattformUserId", "UserConnections",
            "PlattformUserId", "PlattformUsers", principalColumn: "Id", onDelete: ReferentialAction.Cascade);

    }
}

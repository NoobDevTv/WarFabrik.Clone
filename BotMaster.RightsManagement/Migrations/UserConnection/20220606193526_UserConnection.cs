using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotMaster.RightsManagement.Migrations.UserConnection
{
    public partial class UserConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserConnections",
                columns: table => new
                {
                    ConnectionCode = table.Column<string>(type: "TEXT", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Connected = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlattformUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnections", x => x.ConnectionCode);
                    table.ForeignKey(
                        name: "FK_UserConnections_PlattformUsers_PlattformUserId",
                        column: x => x.PlattformUserId,
                        principalTable: "PlattformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_PlattformUserId",
                table: "UserConnections",
                column: "PlattformUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserConnections");
        }
    }
}

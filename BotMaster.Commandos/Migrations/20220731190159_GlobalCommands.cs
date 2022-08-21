using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotMaster.Commandos.Migrations
{
    public partial class GlobalCommands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Global",
                table: "Commands",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Global",
                table: "Commands");
        }
    }
}

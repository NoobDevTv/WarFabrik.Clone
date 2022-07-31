using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BotMaster.RightsManagement.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rights", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupRight",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "INTEGER", nullable: false),
                    RightsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupRight", x => new { x.GroupsId, x.RightsId });
                    table.ForeignKey(
                        name: "FK_GroupRight_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupRight_Rights_RightsId",
                        column: x => x.RightsId,
                        principalTable: "Rights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupUser",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupUser", x => new { x.GroupsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GroupUser_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlattformUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Platform = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PlattformUserId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlattformUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlattformUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RightUser",
                columns: table => new
                {
                    RightsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RightUser", x => new { x.RightsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RightUser_Rights_RightsId",
                        column: x => x.RightsId,
                        principalTable: "Rights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RightUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupPlattformUser",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlattformUsersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupPlattformUser", x => new { x.GroupsId, x.PlattformUsersId });
                    table.ForeignKey(
                        name: "FK_GroupPlattformUser_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPlattformUser_PlattformUsers_PlattformUsersId",
                        column: x => x.PlattformUsersId,
                        principalTable: "PlattformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlattformUserRight",
                columns: table => new
                {
                    PlattformUsersId = table.Column<int>(type: "INTEGER", nullable: false),
                    RightsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlattformUserRight", x => new { x.PlattformUsersId, x.RightsId });
                    table.ForeignKey(
                        name: "FK_PlattformUserRight_PlattformUsers_PlattformUsersId",
                        column: x => x.PlattformUsersId,
                        principalTable: "PlattformUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlattformUserRight_Rights_RightsId",
                        column: x => x.RightsId,
                        principalTable: "Rights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupPlattformUser_PlattformUsersId",
                table: "GroupPlattformUser",
                column: "PlattformUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupRight_RightsId",
                table: "GroupRight",
                column: "RightsId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupUser_UsersId",
                table: "GroupUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_PlattformUserRight_RightsId",
                table: "PlattformUserRight",
                column: "RightsId");

            migrationBuilder.CreateIndex(
                name: "IX_PlattformUsers_UserId",
                table: "PlattformUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RightUser_UsersId",
                table: "RightUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupPlattformUser");

            migrationBuilder.DropTable(
                name: "GroupRight");

            migrationBuilder.DropTable(
                name: "GroupUser");

            migrationBuilder.DropTable(
                name: "PlattformUserRight");

            migrationBuilder.DropTable(
                name: "RightUser");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "PlattformUsers");

            migrationBuilder.DropTable(
                name: "Rights");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

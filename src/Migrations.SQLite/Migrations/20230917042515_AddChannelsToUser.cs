using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Users_BotUserId",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_BotUserId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "BotUserId",
                table: "Channels");

            migrationBuilder.CreateTable(
                name: "BotChannelBotUser",
                columns: table => new
                {
                    ChannelsId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotChannelBotUser", x => new { x.ChannelsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_BotChannelBotUser_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotChannelBotUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotChannelBotUser_UsersId",
                table: "BotChannelBotUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotChannelBotUser");

            migrationBuilder.AddColumn<long>(
                name: "BotUserId",
                table: "Channels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_BotUserId",
                table: "Channels",
                column: "BotUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Users_BotUserId",
                table: "Channels",
                column: "BotUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

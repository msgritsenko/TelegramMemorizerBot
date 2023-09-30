using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class AddEntitiesForMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ReplyableMessages",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "Entities",
                table: "Questions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ReplyableMessages_MessageId",
                table: "ReplyableMessages",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_BotChannelid",
                table: "Questions",
                column: "BotChannelid");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Name",
                table: "Channels",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Channels_BotChannelid",
                table: "Questions",
                column: "BotChannelid",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Channels_BotChannelid",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_ReplyableMessages_MessageId",
                table: "ReplyableMessages");

            migrationBuilder.DropIndex(
                name: "IX_Questions_BotChannelid",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Channels_Name",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Entities",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "ReplyableMessages",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}

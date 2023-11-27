using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class LastQuestionIdForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastQuestionId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastQuestionId",
                table: "Users");
        }
    }
}

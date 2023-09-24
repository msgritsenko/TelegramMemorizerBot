using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.SQLite.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAnswerFieldFromCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answer",
                table: "Questions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Answer",
                table: "Questions",
                type: "TEXT",
                nullable: true);
        }
    }
}

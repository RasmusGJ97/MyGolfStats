using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGolfStatsApi.Migrations
{
    /// <inheritdoc />
    public partial class loggingErrorMiddleware : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "ApiLogEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StackTrace",
                table: "ApiLogEntries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "ApiLogEntries");

            migrationBuilder.DropColumn(
                name: "StackTrace",
                table: "ApiLogEntries");
        }
    }
}

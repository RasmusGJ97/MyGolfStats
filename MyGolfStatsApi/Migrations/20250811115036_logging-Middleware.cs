using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGolfStatsApi.Migrations
{
    /// <inheritdoc />
    public partial class loggingMiddleware : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ApiLogEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ApiLogEntries",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

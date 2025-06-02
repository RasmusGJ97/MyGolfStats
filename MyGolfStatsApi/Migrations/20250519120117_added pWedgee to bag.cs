using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGolfStatsApi.Migrations
{
    /// <inheritdoc />
    public partial class addedpWedgeetobag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PWedge",
                table: "Bag",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PWedge",
                table: "Bag");
        }
    }
}

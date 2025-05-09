using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGolfStatsApi.Migrations
{
    /// <inheritdoc />
    public partial class smalldbchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bag",
                table: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Bag",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Driver = table.Column<bool>(type: "bit", nullable: false),
                    TwoW = table.Column<bool>(type: "bit", nullable: false),
                    ThreeW = table.Column<bool>(type: "bit", nullable: false),
                    FourW = table.Column<bool>(type: "bit", nullable: false),
                    FiveW = table.Column<bool>(type: "bit", nullable: false),
                    SixW = table.Column<bool>(type: "bit", nullable: false),
                    SevenW = table.Column<bool>(type: "bit", nullable: false),
                    TwoHy = table.Column<bool>(type: "bit", nullable: false),
                    ThreeHy = table.Column<bool>(type: "bit", nullable: false),
                    FourHy = table.Column<bool>(type: "bit", nullable: false),
                    FiveHy = table.Column<bool>(type: "bit", nullable: false),
                    OneI = table.Column<bool>(type: "bit", nullable: false),
                    TwoI = table.Column<bool>(type: "bit", nullable: false),
                    ThreeI = table.Column<bool>(type: "bit", nullable: false),
                    FourI = table.Column<bool>(type: "bit", nullable: false),
                    FiveI = table.Column<bool>(type: "bit", nullable: false),
                    SixI = table.Column<bool>(type: "bit", nullable: false),
                    SevenI = table.Column<bool>(type: "bit", nullable: false),
                    EightI = table.Column<bool>(type: "bit", nullable: false),
                    NineI = table.Column<bool>(type: "bit", nullable: false),
                    AWedge = table.Column<bool>(type: "bit", nullable: false),
                    GWedge = table.Column<bool>(type: "bit", nullable: false),
                    SWedge = table.Column<bool>(type: "bit", nullable: false),
                    LWedge = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bag_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bag_UserId",
                table: "Bag",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bag");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Bag",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

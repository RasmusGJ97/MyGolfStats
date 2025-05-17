using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGolfStatsApi.Migrations
{
    /// <inheritdoc />
    public partial class dbchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PenaltyCauses_Statistics_StatisticsId",
                table: "PenaltyCauses");

            migrationBuilder.DropIndex(
                name: "IX_PenaltyCauses_StatisticsId",
                table: "PenaltyCauses");

            migrationBuilder.RenameColumn(
                name: "StatisticsId",
                table: "PenaltyCauses",
                newName: "PenaltyStrokes");

            migrationBuilder.AddColumn<int>(
                name: "Strokes",
                table: "Statistics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PenaltyCauseStatistics",
                columns: table => new
                {
                    PenaltyCauseId = table.Column<int>(type: "int", nullable: false),
                    StatisticsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PenaltyCauseStatistics", x => new { x.PenaltyCauseId, x.StatisticsId });
                    table.ForeignKey(
                        name: "FK_PenaltyCauseStatistics_PenaltyCauses_PenaltyCauseId",
                        column: x => x.PenaltyCauseId,
                        principalTable: "PenaltyCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PenaltyCauseStatistics_Statistics_StatisticsId",
                        column: x => x.StatisticsId,
                        principalTable: "Statistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyCauseStatistics_StatisticsId",
                table: "PenaltyCauseStatistics",
                column: "StatisticsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PenaltyCauseStatistics");

            migrationBuilder.DropColumn(
                name: "Strokes",
                table: "Statistics");

            migrationBuilder.RenameColumn(
                name: "PenaltyStrokes",
                table: "PenaltyCauses",
                newName: "StatisticsId");

            migrationBuilder.CreateIndex(
                name: "IX_PenaltyCauses_StatisticsId",
                table: "PenaltyCauses",
                column: "StatisticsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PenaltyCauses_Statistics_StatisticsId",
                table: "PenaltyCauses",
                column: "StatisticsId",
                principalTable: "Statistics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockTrader.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Approaches",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    ApproachDefinitionJson = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approaches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(maxLength: 10, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Open = table.Column<decimal>(type: "decimal(14,4)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(14,4)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(14,4)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(14,4)", nullable: false),
                    AdjustedClose = table.Column<decimal>(type: "decimal(14,4)", nullable: false),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Ticker = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Ticker);
                });

            migrationBuilder.CreateTable(
                name: "Runs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApproachId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Definition = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    MachineId = table.Column<Guid>(nullable: false),
                    AssignedKey = table.Column<Guid>(nullable: false),
                    AssignedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runs_Approaches_ApproachId",
                        column: x => x.ApproachId,
                        principalTable: "Approaches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Runs_ApproachId",
                table: "Runs",
                column: "ApproachId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Runs");

            migrationBuilder.DropTable(
                name: "StockData");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Approaches");
        }
    }
}

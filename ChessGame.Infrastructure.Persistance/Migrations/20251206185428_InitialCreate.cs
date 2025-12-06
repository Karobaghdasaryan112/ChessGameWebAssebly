using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessGame.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChessGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player1Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Player2Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WinnerPlayer = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player1Time = table.Column<int>(type: "int", nullable: false),
                    Player2Time = table.Column<int>(type: "int", nullable: false),
                    GameEvent = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChessGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChessGamesHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChessGamesHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChessGames");

            migrationBuilder.DropTable(
                name: "ChessGamesHistory");
        }
    }
}

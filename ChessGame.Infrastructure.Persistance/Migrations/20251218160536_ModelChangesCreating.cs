using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessGame.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class ModelChangesCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ChessGames",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ChessGames",
                newName: "CreateDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "ChessGames",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "ChessGames",
                newName: "CreatedAt");
        }
    }
}

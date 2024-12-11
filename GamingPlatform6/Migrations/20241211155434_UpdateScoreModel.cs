using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingPlatform6.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScoreModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Scores",
                newName: "UserUsername");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_UserId",
                table: "Scores",
                newName: "IX_Scores_UserUsername");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Users_UserUsername",
                table: "Scores",
                column: "UserUsername",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scores_Users_UserUsername",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "UserUsername",
                table: "Scores",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Scores_UserUsername",
                table: "Scores",
                newName: "IX_Scores_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_Users_UserId",
                table: "Scores",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingPlatform6.Migrations
{
    /// <inheritdoc />
    public partial class AddLobbyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lobbies",
                columns: table => new
                {
                    LobbyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HostUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuestUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lobbies", x => x.LobbyId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lobbies");
        }
    }
}

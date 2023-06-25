using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UbertweakNfcReaderWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToScan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Scans",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Scans_UserId",
                table: "Scans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Scans_Users_UserId",
                table: "Scans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scans_Users_UserId",
                table: "Scans");

            migrationBuilder.DropIndex(
                name: "IX_Scans_UserId",
                table: "Scans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Scans");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UbertweakNfcReaderWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaderToPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeaderId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_LeaderId",
                table: "Purchases",
                column: "LeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_LeaderId",
                table: "Purchases",
                column: "LeaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_LeaderId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_LeaderId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "LeaderId",
                table: "Purchases");
        }
    }
}

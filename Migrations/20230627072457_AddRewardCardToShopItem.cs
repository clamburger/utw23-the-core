using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UbertweakNfcReaderWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardCardToShopItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RewardCardId",
                table: "ShopItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopItems_RewardCardId",
                table: "ShopItems",
                column: "RewardCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopItems_Cards_RewardCardId",
                table: "ShopItems",
                column: "RewardCardId",
                principalTable: "Cards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopItems_Cards_RewardCardId",
                table: "ShopItems");

            migrationBuilder.DropIndex(
                name: "IX_ShopItems_RewardCardId",
                table: "ShopItems");

            migrationBuilder.DropColumn(
                name: "RewardCardId",
                table: "ShopItems");
        }
    }
}

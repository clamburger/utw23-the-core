using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UbertweakNfcReaderWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Balance",
                table: "Teams",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Teams");
        }
    }
}

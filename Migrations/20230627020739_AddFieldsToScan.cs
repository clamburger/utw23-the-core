using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UbertweakNfcReaderWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToScan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scans_Teams_TeamId",
                table: "Scans");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Scans",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Scans",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Scans_Teams_TeamId",
                table: "Scans",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Scans_Teams_TeamId",
                table: "Scans");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Scans");

            migrationBuilder.AlterColumn<int>(
                name: "TeamId",
                table: "Scans",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Scans_Teams_TeamId",
                table: "Scans",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

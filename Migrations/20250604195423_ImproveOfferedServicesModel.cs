using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class ImproveOfferedServicesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OfferedServices",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<char>(
                name: "Currency",
                table: "OfferedServices",
                type: "character(1)",
                nullable: false,
                defaultValue: '€',
                oldClrType: typeof(char),
                oldType: "character(1)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "OfferedServices",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "OfferedServices");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "OfferedServices",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<char>(
                name: "Currency",
                table: "OfferedServices",
                type: "character(1)",
                nullable: true,
                oldClrType: typeof(char),
                oldType: "character(1)");
        }
    }
}

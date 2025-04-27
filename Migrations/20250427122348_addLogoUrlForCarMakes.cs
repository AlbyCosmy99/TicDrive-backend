using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class addLogoUrlForCarMakes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "CarMakes",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "CarMakes");
        }
    }
}

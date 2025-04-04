using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class addFKtoOfferedServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferedServices_Users_WorkshopId",
                table: "OfferedServices");

            migrationBuilder.AlterColumn<string>(
                name: "WorkshopId",
                table: "OfferedServices",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OfferedServices_Users_WorkshopId",
                table: "OfferedServices",
                column: "WorkshopId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OfferedServices_Users_WorkshopId",
                table: "OfferedServices");

            migrationBuilder.AlterColumn<string>(
                name: "WorkshopId",
                table: "OfferedServices",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_OfferedServices_Users_WorkshopId",
                table: "OfferedServices",
                column: "WorkshopId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

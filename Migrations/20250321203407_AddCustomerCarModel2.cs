using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCarModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCar_Users_CustomerId",
                table: "CustomerCar");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "CustomerCar");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "CustomerCar",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCar_Users_CustomerId",
                table: "CustomerCar",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCar_Users_CustomerId",
                table: "CustomerCar");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "CustomerCar",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "CustomerCar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCar_Users_CustomerId",
                table: "CustomerCar",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

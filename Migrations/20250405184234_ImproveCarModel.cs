using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class ImproveCarModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCar_Cars_CarId",
                table: "CustomerCar");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCar_Users_CustomerId",
                table: "CustomerCar");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerCar",
                table: "CustomerCar");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Cars");

            migrationBuilder.RenameTable(
                name: "CustomerCar",
                newName: "CustomerCars");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerCar_CustomerId",
                table: "CustomerCars",
                newName: "IX_CustomerCars_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerCar_CarId",
                table: "CustomerCars",
                newName: "IX_CustomerCars_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerCars",
                table: "CustomerCars",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCars_Cars_CarId",
                table: "CustomerCars",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCars_Users_CustomerId",
                table: "CustomerCars",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCars_Cars_CarId",
                table: "CustomerCars");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerCars_Users_CustomerId",
                table: "CustomerCars");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerCars",
                table: "CustomerCars");

            migrationBuilder.RenameTable(
                name: "CustomerCars",
                newName: "CustomerCar");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerCars_CustomerId",
                table: "CustomerCar",
                newName: "IX_CustomerCar_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_CustomerCars_CarId",
                table: "CustomerCar",
                newName: "IX_CustomerCar_CarId");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerCar",
                table: "CustomerCar",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCar_Cars_CarId",
                table: "CustomerCar",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerCar_Users_CustomerId",
                table: "CustomerCar",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

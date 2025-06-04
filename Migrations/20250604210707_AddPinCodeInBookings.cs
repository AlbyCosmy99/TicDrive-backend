using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddPinCodeInBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PinCode",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "Bookings");
        }
    }
}

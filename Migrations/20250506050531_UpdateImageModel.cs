using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "UserImages",
                newName: "Filename");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Filename",
                table: "UserImages",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}

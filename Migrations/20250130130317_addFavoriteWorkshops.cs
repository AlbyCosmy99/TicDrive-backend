using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class addFavoriteWorkshops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteWorkshops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WorkshopId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteWorkshops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteWorkshops_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoriteWorkshops_Users_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteWorkshops_CustomerId",
                table: "FavoriteWorkshops",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteWorkshops_WorkshopId",
                table: "FavoriteWorkshops",
                column: "WorkshopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteWorkshops");
        }
    }
}

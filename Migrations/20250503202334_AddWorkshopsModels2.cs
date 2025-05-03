using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopsModels2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userConsents_LegalDeclarations_LegalDeclarationId",
                table: "userConsents");

            migrationBuilder.DropForeignKey(
                name: "FK_userConsents_Users_UserId",
                table: "userConsents");

            migrationBuilder.DropForeignKey(
                name: "FK_workshopsSchedules_Days_DayId",
                table: "workshopsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_workshopsSchedules_Users_WorkshopId",
                table: "workshopsSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workshopsSchedules",
                table: "workshopsSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userConsents",
                table: "userConsents");

            migrationBuilder.RenameTable(
                name: "workshopsSchedules",
                newName: "WorkshopsSchedules");

            migrationBuilder.RenameTable(
                name: "userConsents",
                newName: "UserConsents");

            migrationBuilder.RenameIndex(
                name: "IX_workshopsSchedules_WorkshopId",
                table: "WorkshopsSchedules",
                newName: "IX_WorkshopsSchedules_WorkshopId");

            migrationBuilder.RenameIndex(
                name: "IX_workshopsSchedules_DayId",
                table: "WorkshopsSchedules",
                newName: "IX_WorkshopsSchedules_DayId");

            migrationBuilder.RenameIndex(
                name: "IX_userConsents_UserId",
                table: "UserConsents",
                newName: "IX_UserConsents_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_userConsents_LegalDeclarationId",
                table: "UserConsents",
                newName: "IX_UserConsents_LegalDeclarationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkshopsSchedules",
                table: "WorkshopsSchedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConsents",
                table: "UserConsents",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    IsMainImage = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserImages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserImages_UserId",
                table: "UserImages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserConsents_LegalDeclarations_LegalDeclarationId",
                table: "UserConsents",
                column: "LegalDeclarationId",
                principalTable: "LegalDeclarations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConsents_Users_UserId",
                table: "UserConsents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkshopsSchedules_Days_DayId",
                table: "WorkshopsSchedules",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkshopsSchedules_Users_WorkshopId",
                table: "WorkshopsSchedules",
                column: "WorkshopId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserConsents_LegalDeclarations_LegalDeclarationId",
                table: "UserConsents");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConsents_Users_UserId",
                table: "UserConsents");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopsSchedules_Days_DayId",
                table: "WorkshopsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkshopsSchedules_Users_WorkshopId",
                table: "WorkshopsSchedules");

            migrationBuilder.DropTable(
                name: "UserImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkshopsSchedules",
                table: "WorkshopsSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConsents",
                table: "UserConsents");

            migrationBuilder.RenameTable(
                name: "WorkshopsSchedules",
                newName: "workshopsSchedules");

            migrationBuilder.RenameTable(
                name: "UserConsents",
                newName: "userConsents");

            migrationBuilder.RenameIndex(
                name: "IX_WorkshopsSchedules_WorkshopId",
                table: "workshopsSchedules",
                newName: "IX_workshopsSchedules_WorkshopId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkshopsSchedules_DayId",
                table: "workshopsSchedules",
                newName: "IX_workshopsSchedules_DayId");

            migrationBuilder.RenameIndex(
                name: "IX_UserConsents_UserId",
                table: "userConsents",
                newName: "IX_userConsents_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserConsents_LegalDeclarationId",
                table: "userConsents",
                newName: "IX_userConsents_LegalDeclarationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_workshopsSchedules",
                table: "workshopsSchedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userConsents",
                table: "userConsents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_userConsents_LegalDeclarations_LegalDeclarationId",
                table: "userConsents",
                column: "LegalDeclarationId",
                principalTable: "LegalDeclarations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userConsents_Users_UserId",
                table: "userConsents",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workshopsSchedules_Days_DayId",
                table: "workshopsSchedules",
                column: "DayId",
                principalTable: "Days",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workshopsSchedules_Users_WorkshopId",
                table: "workshopsSchedules",
                column: "WorkshopId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TicDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopsModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LegalDeclarations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Issued = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Context = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalDeclarations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopsDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<string>(type: "text", nullable: false),
                    WorkshopName = table.Column<string>(type: "text", nullable: false),
                    AcceptUpdates = table.Column<bool>(type: "boolean", nullable: false),
                    AcceptPrivacyPolicy = table.Column<bool>(type: "boolean", nullable: false),
                    PersonalPhoneNumber = table.Column<string>(type: "text", nullable: false),
                    PersonalEmail = table.Column<string>(type: "text", nullable: false),
                    OffersHomeServices = table.Column<bool>(type: "boolean", nullable: true),
                    MaxDailyVehicles = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    LaborWarrantyMonths = table.Column<int>(type: "integer", nullable: false),
                    SignatureName = table.Column<string>(type: "text", nullable: false),
                    SignatureSurname = table.Column<string>(type: "text", nullable: false),
                    SignatureDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopsDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopsDetails_Users_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopsNonWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopsNonWorkingDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopsNonWorkingDays_Users_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "workshopsSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<string>(type: "text", nullable: false),
                    DayId = table.Column<int>(type: "integer", nullable: false),
                    MorningStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    MorningEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    AfternoonStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    AfternoonEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workshopsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_workshopsSchedules_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_workshopsSchedules_Users_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userConsents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LegalDeclarationId = table.Column<int>(type: "integer", nullable: false),
                    When = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userConsents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userConsents_LegalDeclarations_LegalDeclarationId",
                        column: x => x.LegalDeclarationId,
                        principalTable: "LegalDeclarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userConsents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkshopsSpecializations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkshopId = table.Column<string>(type: "text", nullable: false),
                    SpecializationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopsSpecializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopsSpecializations_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkshopsSpecializations_Users_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userConsents_LegalDeclarationId",
                table: "userConsents",
                column: "LegalDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_userConsents_UserId",
                table: "userConsents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopsDetails_WorkshopId",
                table: "WorkshopsDetails",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopsNonWorkingDays_WorkshopId",
                table: "WorkshopsNonWorkingDays",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_workshopsSchedules_DayId",
                table: "workshopsSchedules",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_workshopsSchedules_WorkshopId",
                table: "workshopsSchedules",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopsSpecializations_SpecializationId",
                table: "WorkshopsSpecializations",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopsSpecializations_WorkshopId",
                table: "WorkshopsSpecializations",
                column: "WorkshopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userConsents");

            migrationBuilder.DropTable(
                name: "WorkshopsDetails");

            migrationBuilder.DropTable(
                name: "WorkshopsNonWorkingDays");

            migrationBuilder.DropTable(
                name: "workshopsSchedules");

            migrationBuilder.DropTable(
                name: "WorkshopsSpecializations");

            migrationBuilder.DropTable(
                name: "LegalDeclarations");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "Specializations");
        }
    }
}

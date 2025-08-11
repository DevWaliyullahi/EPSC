using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPSC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "TMembers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "TMembers",
                type: "nvarchar(55)",
                maxLength: 55,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "TMembers",
                type: "nvarchar(55)",
                maxLength: 55,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TMembers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployerId",
                table: "TMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "TMembers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TEmployerEmployerId",
                table: "TMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TBenefitEligibilities",
                columns: table => new
                {
                    BenefitEligibilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsEligible = table.Column<bool>(type: "bit", nullable: false),
                    EligibilityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MonthsContributed = table.Column<int>(type: "int", nullable: false),
                    TotalContributions = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBenefitEligibilities", x => x.BenefitEligibilityId);
                    table.ForeignKey(
                        name: "FK_TBenefitEligibilities_TMembers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "TMembers",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TBenefitEligibilities_TMembers_MemberId1",
                        column: x => x.MemberId1,
                        principalTable: "TMembers",
                        principalColumn: "MemberId");
                });

            migrationBuilder.CreateTable(
                name: "TEmployers",
                columns: table => new
                {
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RCNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEmployers", x => x.EmployerId);
                });

            migrationBuilder.CreateTable(
                name: "TTransactionLogs",
                columns: table => new
                {
                    TransactionLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TTransactionLogs", x => x.TransactionLogId);
                });

            migrationBuilder.CreateTable(
                name: "TContributions",
                columns: table => new
                {
                    ContributionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContributionType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContributionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsValidated = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ValidationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployerId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TContributions", x => x.ContributionId);
                    table.ForeignKey(
                        name: "FK_TContributions_TEmployers_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "TEmployers",
                        principalColumn: "EmployerId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TContributions_TEmployers_EmployerId1",
                        column: x => x.EmployerId1,
                        principalTable: "TEmployers",
                        principalColumn: "EmployerId");
                    table.ForeignKey(
                        name: "FK_TContributions_TMembers_MemberId",
                        column: x => x.MemberId,
                        principalTable: "TMembers",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TContributions_TMembers_MemberId1",
                        column: x => x.MemberId1,
                        principalTable: "TMembers",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TMembers_Email",
                table: "TMembers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TMembers_TEmployerEmployerId",
                table: "TMembers",
                column: "TEmployerEmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_TBenefitEligibilities_MemberId",
                table: "TBenefitEligibilities",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TBenefitEligibilities_MemberId1",
                table: "TBenefitEligibilities",
                column: "MemberId1");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyContribution_Validation",
                table: "TContributions",
                columns: new[] { "MemberId", "ContributionDate", "ContributionType" });

            migrationBuilder.CreateIndex(
                name: "IX_TContributions_EmployerId",
                table: "TContributions",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_TContributions_EmployerId1",
                table: "TContributions",
                column: "EmployerId1");

            migrationBuilder.CreateIndex(
                name: "IX_TContributions_MemberId1",
                table: "TContributions",
                column: "MemberId1");

            migrationBuilder.CreateIndex(
                name: "IX_TEmployers_RCNumber",
                table: "TEmployers",
                column: "RCNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TMembers_TEmployers_TEmployerEmployerId",
                table: "TMembers",
                column: "TEmployerEmployerId",
                principalTable: "TEmployers",
                principalColumn: "EmployerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TMembers_TEmployers_TEmployerEmployerId",
                table: "TMembers");

            migrationBuilder.DropTable(
                name: "TBenefitEligibilities");

            migrationBuilder.DropTable(
                name: "TContributions");

            migrationBuilder.DropTable(
                name: "TTransactionLogs");

            migrationBuilder.DropTable(
                name: "TEmployers");

            migrationBuilder.DropIndex(
                name: "IX_TMembers_Email",
                table: "TMembers");

            migrationBuilder.DropIndex(
                name: "IX_TMembers_TEmployerEmployerId",
                table: "TMembers");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "TMembers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "TMembers");

            migrationBuilder.DropColumn(
                name: "TEmployerEmployerId",
                table: "TMembers");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "TMembers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TMembers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(55)",
                oldMaxLength: 55);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(55)",
                oldMaxLength: 55);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "TMembers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}

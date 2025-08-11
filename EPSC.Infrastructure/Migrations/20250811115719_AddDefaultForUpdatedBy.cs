using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPSC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultForUpdatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "TEmployers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "system",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "system",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "TEmployers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "system");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "TMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "system");
        }
    }
}

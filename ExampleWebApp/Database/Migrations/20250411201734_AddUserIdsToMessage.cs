using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdsToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProcessedData_UserId",
                table: "Events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedData_UserTagId",
                table: "Events",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedData_UserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProcessedData_UserTagId",
                table: "Events");
        }
    }
}

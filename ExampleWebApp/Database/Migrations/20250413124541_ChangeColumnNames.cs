using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedData_UserTagId",
                table: "Events",
                newName: "ProcessedData_InitiatorSystemId");

            migrationBuilder.RenameColumn(
                name: "ProcessedData_UserId",
                table: "Events",
                newName: "ProcessedData_InitiatorTagEPC");

            migrationBuilder.RenameColumn(
                name: "ProcessedData_User",
                table: "Events",
                newName: "ProcessedData_InitiatorName");

            migrationBuilder.RenameColumn(
                name: "ProcessedData_Tools",
                table: "Events",
                newName: "ProcessedData_TagNames");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProcessedData_TagNames",
                table: "Events",
                newName: "ProcessedData_Tools");

            migrationBuilder.RenameColumn(
                name: "ProcessedData_InitiatorTagEPC",
                table: "Events",
                newName: "ProcessedData_UserId");

            migrationBuilder.RenameColumn(
                name: "ProcessedData_InitiatorSystemId",
                table: "Events",
                newName: "ProcessedData_UserTagId");

            migrationBuilder.RenameColumn(
                name: "ProcessedData_InitiatorName",
                table: "Events",
                newName: "ProcessedData_User");
        }
    }
}

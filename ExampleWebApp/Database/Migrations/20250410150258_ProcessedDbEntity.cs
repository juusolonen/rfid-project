using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ProcessedDbEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProcessedData_Action",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedData_ProcessedAt",
                table: "Events",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedData_Tag",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "ProcessedData_Tools",
                table: "Events",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessedData_User",
                table: "Events",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedData_Action",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProcessedData_ProcessedAt",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProcessedData_Tag",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProcessedData_Tools",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProcessedData_User",
                table: "Events");
        }
    }
}

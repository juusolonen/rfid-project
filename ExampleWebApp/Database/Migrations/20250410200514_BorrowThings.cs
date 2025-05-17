using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class BorrowThings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowedAt",
                table: "Tools",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastBorrower",
                table: "Tools",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnedAt",
                table: "Tools",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<JsonDocument>(
                name: "Data",
                table: "Events",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(JsonDocument),
                oldType: "jsonb");

            migrationBuilder.AddColumn<bool>(
                name: "Faulted",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowedAt",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "LastBorrower",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "ReturnedAt",
                table: "Tools");

            migrationBuilder.DropColumn(
                name: "Faulted",
                table: "Events");

            migrationBuilder.AlterColumn<JsonDocument>(
                name: "Data",
                table: "Events",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(JsonDocument),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}

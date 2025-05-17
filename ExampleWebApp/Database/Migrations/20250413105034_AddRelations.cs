using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CallerId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetUserId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventBaseDbEntityToolDbEntity",
                columns: table => new
                {
                    TargetedEventsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToolsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBaseDbEntityToolDbEntity", x => new { x.TargetedEventsId, x.ToolsId });
                    table.ForeignKey(
                        name: "FK_EventBaseDbEntityToolDbEntity_Events_TargetedEventsId",
                        column: x => x.TargetedEventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventBaseDbEntityToolDbEntity_Tools_ToolsId",
                        column: x => x.ToolsId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_CallerId",
                table: "Events",
                column: "CallerId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_TargetUserId",
                table: "Events",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventBaseDbEntityToolDbEntity_ToolsId",
                table: "EventBaseDbEntityToolDbEntity",
                column: "ToolsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_CallerId",
                table: "Events",
                column: "CallerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_TargetUserId",
                table: "Events",
                column: "TargetUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_CallerId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_TargetUserId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventBaseDbEntityToolDbEntity");

            migrationBuilder.DropIndex(
                name: "IX_Events_CallerId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_TargetUserId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CallerId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TargetUserId",
                table: "Events");
        }
    }
}

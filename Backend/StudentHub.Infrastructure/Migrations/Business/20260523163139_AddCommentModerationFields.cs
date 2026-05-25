using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHub.Infrastructure.Migrations.Business
{
    /// <inheritdoc />
    public partial class AddCommentModerationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppealMessage",
                schema: "Business",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppealStatus",
                schema: "Business",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                schema: "Business",
                table: "Comments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToxicFilterTaskId",
                schema: "Business",
                table: "Comments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppealMessage",
                schema: "Business",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AppealStatus",
                schema: "Business",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                schema: "Business",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "ToxicFilterTaskId",
                schema: "Business",
                table: "Comments");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHub.Infrastructure.Migrations.Business
{
    /// <inheritdoc />
    public partial class idk44423 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CriterionScores_TeacherId",
                schema: "Business",
                table: "CriterionScores",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_CriterionScores_Users_TeacherId",
                schema: "Business",
                table: "CriterionScores",
                column: "TeacherId",
                principalSchema: "Business",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CriterionScores_Users_TeacherId",
                schema: "Business",
                table: "CriterionScores");

            migrationBuilder.DropIndex(
                name: "IX_CriterionScores_TeacherId",
                schema: "Business",
                table: "CriterionScores");
        }
    }
}

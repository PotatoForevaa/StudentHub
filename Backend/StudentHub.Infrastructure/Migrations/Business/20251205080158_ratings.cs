using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHub.Infrastructure.Migrations.Business
{
    /// <inheritdoc />
    public partial class ratings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserId",
                schema: "Business",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UserId",
                schema: "Business",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Business",
                table: "Projects");

            migrationBuilder.CreateTable(
                name: "ProjectRatings",
                schema: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRatings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Business",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AuthorId",
                schema: "Business",
                table: "Projects",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRatings_ProjectId",
                schema: "Business",
                table: "ProjectRatings",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_AuthorId",
                schema: "Business",
                table: "Projects",
                column: "AuthorId",
                principalSchema: "Business",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_AuthorId",
                schema: "Business",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectRatings",
                schema: "Business");

            migrationBuilder.DropIndex(
                name: "IX_Projects_AuthorId",
                schema: "Business",
                table: "Projects");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "Business",
                table: "Projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                schema: "Business",
                table: "Projects",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UserId",
                schema: "Business",
                table: "Projects",
                column: "UserId",
                principalSchema: "Business",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHub.Infrastructure.Migrations.Business
{
    /// <inheritdoc />
    public partial class AddProjectComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectComments",
                schema: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectComments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Business",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Business",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectComments_ProjectId",
                schema: "Business",
                table: "ProjectComments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectComments_AuthorId",
                schema: "Business",
                table: "ProjectComments",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectComments",
                schema: "Business");
        }
    }
}

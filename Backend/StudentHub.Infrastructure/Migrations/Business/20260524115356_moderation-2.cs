using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHub.Infrastructure.Migrations.Business
{
    /// <inheritdoc />
    public partial class moderation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMutes",
                schema: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MutedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    MutedUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMutes_Users_MutedByUserId",
                        column: x => x.MutedByUserId,
                        principalSchema: "Business",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMutes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Business",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Criteria",
                schema: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Criteria_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Business",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCategories",
                schema: "Business",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCategories", x => new { x.ProjectId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ProjectCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Business",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectCategories_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Business",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTags",
                schema: "Business",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTags", x => new { x.ProjectId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ProjectTags_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Business",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "Business",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriterionScores",
                schema: "Business",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriterionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriterionScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriterionScores_Criteria_CriterionId",
                        column: x => x.CriterionId,
                        principalSchema: "Business",
                        principalTable: "Criteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriterionScores_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Business",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Criteria_CategoryId",
                schema: "Business",
                table: "Criteria",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionScores_CriterionId",
                schema: "Business",
                table: "CriterionScores",
                column: "CriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_CriterionScores_ProjectId",
                schema: "Business",
                table: "CriterionScores",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCategories_CategoryId",
                schema: "Business",
                table: "ProjectCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_TagId",
                schema: "Business",
                table: "ProjectTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMutes_MutedByUserId",
                schema: "Business",
                table: "UserMutes",
                column: "MutedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMutes_UserId",
                schema: "Business",
                table: "UserMutes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriterionScores",
                schema: "Business");

            migrationBuilder.DropTable(
                name: "ProjectCategories",
                schema: "Business");

            migrationBuilder.DropTable(
                name: "ProjectTags",
                schema: "Business");

            migrationBuilder.DropTable(
                name: "UserMutes",
                schema: "Business");

            migrationBuilder.DropTable(
                name: "Criteria",
                schema: "Business");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Business");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Business");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Alumni.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationsAndJobPostingRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_posts_users_posted_by_user_id",
                table: "job_posts");

            migrationBuilder.CreateTable(
                name: "applications",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    job_post_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Applied"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_applications_job_posts_job_post_id",
                        column: x => x.job_post_id,
                        principalTable: "job_posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_applications_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_applications_job_post_id",
                table: "applications",
                column: "job_post_id");

            migrationBuilder.CreateIndex(
                name: "ix_applications_user_id",
                table: "applications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_applications_user_id_job_post_id",
                table: "applications",
                columns: new[] { "user_id", "job_post_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_job_posts_users_posted_by_user_id",
                table: "job_posts",
                column: "posted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_job_posts_users_posted_by_user_id",
                table: "job_posts");

            migrationBuilder.DropTable(
                name: "applications");

            migrationBuilder.AddForeignKey(
                name: "FK_job_posts_users_posted_by_user_id",
                table: "job_posts",
                column: "posted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

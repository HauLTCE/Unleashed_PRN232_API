using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    order_id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 255, nullable: false),
                    review_rating = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("review_pkey", x => x.review_id);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    review_id = table.Column<int>(type: "int", nullable: true),
                    comment_content = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    comment_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    comment_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("comment_pkey", x => x.comment_id);
                    table.ForeignKey(
                        name: "comment_review_id_fkey",
                        column: x => x.review_id,
                        principalTable: "review",
                        principalColumn: "review_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment_parent",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false),
                    comment_parent_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment_parent", x => new { x.comment_id, x.comment_parent_id });
                    table.ForeignKey(
                        name: "comment_parent_comment_id_fkey",
                        column: x => x.comment_id,
                        principalTable: "comment",
                        principalColumn: "comment_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "comment_parent_comment_parent_id_fkey",
                        column: x => x.comment_parent_id,
                        principalTable: "comment",
                        principalColumn: "comment_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_comment_review_id",
                table: "comment",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "IX_comment_parent_comment_parent_id",
                table: "comment_parent",
                column: "comment_parent_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comment_parent");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "review");
        }
    }
}

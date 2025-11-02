using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReviewService.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteToReviewComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "comment_review_id_fkey",
                table: "comment");

            migrationBuilder.AddForeignKey(
                name: "comment_review_id_fkey",
                table: "comment",
                column: "review_id",
                principalTable: "review",
                principalColumn: "review_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "comment_review_id_fkey",
                table: "comment");

            migrationBuilder.AddForeignKey(
                name: "comment_review_id_fkey",
                table: "comment",
                column: "review_id",
                principalTable: "review",
                principalColumn: "review_id");
        }
    }
}

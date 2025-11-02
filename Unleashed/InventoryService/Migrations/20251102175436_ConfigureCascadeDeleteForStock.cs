using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCascadeDeleteForStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "stock_variation_stock_id_fkey",
                table: "stock_variation");

            migrationBuilder.DropForeignKey(
                name: "transaction_stock_id_fkey",
                table: "transaction");

            migrationBuilder.AddForeignKey(
                name: "stock_variation_stock_id_fkey",
                table: "stock_variation",
                column: "stock_id",
                principalTable: "stock",
                principalColumn: "stock_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "transaction_stock_id_fkey",
                table: "transaction",
                column: "stock_id",
                principalTable: "stock",
                principalColumn: "stock_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "stock_variation_stock_id_fkey",
                table: "stock_variation");

            migrationBuilder.DropForeignKey(
                name: "transaction_stock_id_fkey",
                table: "transaction");

            migrationBuilder.AddForeignKey(
                name: "stock_variation_stock_id_fkey",
                table: "stock_variation",
                column: "stock_id",
                principalTable: "stock",
                principalColumn: "stock_id");

            migrationBuilder.AddForeignKey(
                name: "transaction_stock_id_fkey",
                table: "transaction",
                column: "stock_id",
                principalTable: "stock",
                principalColumn: "stock_id");
        }
    }
}

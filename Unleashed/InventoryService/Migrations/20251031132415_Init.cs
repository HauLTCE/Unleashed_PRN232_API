using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "provider",
                columns: table => new
                {
                    provider_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    provider_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    provider_image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    provider_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    provider_phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    provider_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    provider_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    provider_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("provider_pkey", x => x.provider_id);
                });

            migrationBuilder.CreateTable(
                name: "stock",
                columns: table => new
                {
                    stock_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stock_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    stock_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stock_pkey", x => x.stock_id);
                });

            migrationBuilder.CreateTable(
                name: "transaction_type",
                columns: table => new
                {
                    transaction_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    transaction_type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transaction_type_pkey", x => x.transaction_type_id);
                });

            migrationBuilder.CreateTable(
                name: "stock_variation",
                columns: table => new
                {
                    variation_id = table.Column<int>(type: "int", nullable: false),
                    stock_id = table.Column<int>(type: "int", nullable: false),
                    stock_quantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("stock_variation_pkey", x => new { x.stock_id, x.variation_id });
                    table.ForeignKey(
                        name: "stock_variation_stock_id_fkey",
                        column: x => x.stock_id,
                        principalTable: "stock",
                        principalColumn: "stock_id");
                });

            migrationBuilder.CreateTable(
                name: "transaction",
                columns: table => new
                {
                    transaction_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    stock_id = table.Column<int>(type: "int", nullable: true),
                    variation_id = table.Column<int>(type: "int", nullable: true),
                    provider_id = table.Column<int>(type: "int", nullable: true),
                    incharge_employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    transaction_type_id = table.Column<int>(type: "int", nullable: true),
                    transaction_quantity = table.Column<int>(type: "int", nullable: true),
                    transaction_date = table.Column<DateTimeOffset>(type: "datetimeoffset(6)", precision: 6, nullable: true),
                    transaction_product_price = table.Column<decimal>(type: "decimal(22,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("transaction_pkey", x => x.transaction_id);
                    table.ForeignKey(
                        name: "transaction_provider_id_fkey",
                        column: x => x.provider_id,
                        principalTable: "provider",
                        principalColumn: "provider_id");
                    table.ForeignKey(
                        name: "transaction_stock_id_fkey",
                        column: x => x.stock_id,
                        principalTable: "stock",
                        principalColumn: "stock_id");
                    table.ForeignKey(
                        name: "transaction_transaction_type_id_fkey",
                        column: x => x.transaction_type_id,
                        principalTable: "transaction_type",
                        principalColumn: "transaction_type_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_transaction_provider_id",
                table: "transaction",
                column: "provider_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_stock_id",
                table: "transaction",
                column: "stock_id");

            migrationBuilder.CreateIndex(
                name: "IX_transaction_transaction_type_id",
                table: "transaction",
                column: "transaction_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_variation");

            migrationBuilder.DropTable(
                name: "transaction");

            migrationBuilder.DropTable(
                name: "provider");

            migrationBuilder.DropTable(
                name: "stock");

            migrationBuilder.DropTable(
                name: "transaction_type");
        }
    }
}

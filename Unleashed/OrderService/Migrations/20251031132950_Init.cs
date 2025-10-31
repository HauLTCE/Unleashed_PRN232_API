using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "order_status",
                columns: table => new
                {
                    order_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_status_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_status_pkey", x => x.order_status_id);
                });

            migrationBuilder.CreateTable(
                name: "payment_method",
                columns: table => new
                {
                    payment_method_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payment_method_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_method_pkey", x => x.payment_method_id);
                });

            migrationBuilder.CreateTable(
                name: "shipping_method",
                columns: table => new
                {
                    shipping_method_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    shipping_method_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("shipping_method_pkey", x => x.shipping_method_id);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 255, nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    order_status_id = table.Column<int>(type: "int", nullable: true),
                    payment_method_id = table.Column<int>(type: "int", nullable: true),
                    shipping_method_id = table.Column<int>(type: "int", nullable: true),
                    discount_id = table.Column<int>(type: "int", nullable: true),
                    incharge_employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    order_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    order_total_amount = table.Column<decimal>(type: "decimal(22,2)", nullable: true),
                    order_tracking_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    order_note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    order_billing_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    order_expected_delivery_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    order_transaction_reference = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    order_tax = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    order_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    order_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_pkey", x => x.order_id);
                    table.ForeignKey(
                        name: "order_order_status_id_fkey",
                        column: x => x.order_status_id,
                        principalTable: "order_status",
                        principalColumn: "order_status_id");
                    table.ForeignKey(
                        name: "order_payment_method_id_fkey",
                        column: x => x.payment_method_id,
                        principalTable: "payment_method",
                        principalColumn: "payment_method_id");
                    table.ForeignKey(
                        name: "order_shipping_method_id_fkey",
                        column: x => x.shipping_method_id,
                        principalTable: "shipping_method",
                        principalColumn: "shipping_method_id");
                });

            migrationBuilder.CreateTable(
                name: "order_variation",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 255, nullable: false),
                    variation_id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    sale_id = table.Column<int>(type: "int", nullable: true),
                    variation_price_at_purchase = table.Column<decimal>(type: "decimal(22,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("order_variation_pkey", x => new { x.order_id, x.variation_id });
                    table.ForeignKey(
                        name: "order_variation_id_fkey",
                        column: x => x.order_id,
                        principalTable: "order",
                        principalColumn: "order_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_order_status_id",
                table: "order",
                column: "order_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_payment_method_id",
                table: "order",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_shipping_method_id",
                table: "order",
                column: "shipping_method_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_variation");

            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "order_status");

            migrationBuilder.DropTable(
                name: "payment_method");

            migrationBuilder.DropTable(
                name: "shipping_method");
        }
    }
}

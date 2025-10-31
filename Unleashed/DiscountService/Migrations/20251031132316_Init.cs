using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscountService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "discount_status",
                columns: table => new
                {
                    discount_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_status_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("discount_status_pkey", x => x.discount_status_id);
                });

            migrationBuilder.CreateTable(
                name: "discount_type",
                columns: table => new
                {
                    discount_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("discount_type_pkey", x => x.discount_type_id);
                });

            migrationBuilder.CreateTable(
                name: "sale_status",
                columns: table => new
                {
                    sale_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_status_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sale_status_pkey", x => x.sale_status_id);
                });

            migrationBuilder.CreateTable(
                name: "sale_type",
                columns: table => new
                {
                    sale_type_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sale_type_pkey", x => x.sale_type_id);
                });

            migrationBuilder.CreateTable(
                name: "discount",
                columns: table => new
                {
                    discount_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    discount_status_id = table.Column<int>(type: "int", nullable: true),
                    discount_type_id = table.Column<int>(type: "int", nullable: true),
                    discount_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    discount_value = table.Column<decimal>(type: "decimal(22,2)", nullable: true),
                    discount_description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    discount_rank_requirement = table.Column<int>(type: "int", nullable: true),
                    discount_minimum_order_value = table.Column<decimal>(type: "decimal(22,2)", nullable: true),
                    discount_maximum_value = table.Column<decimal>(type: "decimal(22,2)", nullable: true),
                    discount_usage_limit = table.Column<int>(type: "int", nullable: true),
                    discount_start_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    discount_end_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    discount_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    discount_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    discount_usage_count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("discount_pkey", x => x.discount_id);
                    table.ForeignKey(
                        name: "discount_discount_status_id_fkey",
                        column: x => x.discount_status_id,
                        principalTable: "discount_status",
                        principalColumn: "discount_status_id");
                    table.ForeignKey(
                        name: "discount_discount_type_id_fkey",
                        column: x => x.discount_type_id,
                        principalTable: "discount_type",
                        principalColumn: "discount_type_id");
                });

            migrationBuilder.CreateTable(
                name: "sale",
                columns: table => new
                {
                    sale_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_type_id = table.Column<int>(type: "int", nullable: true),
                    sale_status_id = table.Column<int>(type: "int", nullable: true),
                    sale_value = table.Column<decimal>(type: "decimal(22,2)", nullable: true),
                    sale_start_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    sale_end_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    sale_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    sale_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sale_pkey", x => x.sale_id);
                    table.ForeignKey(
                        name: "sale_sale_status_id_fkey",
                        column: x => x.sale_status_id,
                        principalTable: "sale_status",
                        principalColumn: "sale_status_id");
                    table.ForeignKey(
                        name: "sale_sale_type_id_fkey",
                        column: x => x.sale_type_id,
                        principalTable: "sale_type",
                        principalColumn: "sale_type_id");
                });

            migrationBuilder.CreateTable(
                name: "user_discount",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    discount_id = table.Column<int>(type: "int", nullable: false),
                    is_discount_used = table.Column<bool>(type: "bit", nullable: false),
                    discount_used_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_discount_pkey", x => new { x.discount_id, x.user_id });
                    table.ForeignKey(
                        name: "user_discount_discount_id_fkey",
                        column: x => x.discount_id,
                        principalTable: "discount",
                        principalColumn: "discount_id");
                });

            migrationBuilder.CreateTable(
                name: "sale_product",
                columns: table => new
                {
                    sale_id = table.Column<int>(type: "int", nullable: false),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sale_product_pkey", x => new { x.sale_id, x.product_id });
                    table.ForeignKey(
                        name: "sale_product_sale_id_fkey",
                        column: x => x.sale_id,
                        principalTable: "sale",
                        principalColumn: "sale_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_discount_discount_status_id",
                table: "discount",
                column: "discount_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_discount_discount_type_id",
                table: "discount",
                column: "discount_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_sale_status_id",
                table: "sale",
                column: "sale_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_sale_sale_type_id",
                table: "sale",
                column: "sale_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale_product");

            migrationBuilder.DropTable(
                name: "user_discount");

            migrationBuilder.DropTable(
                name: "sale");

            migrationBuilder.DropTable(
                name: "discount");

            migrationBuilder.DropTable(
                name: "sale_status");

            migrationBuilder.DropTable(
                name: "sale_type");

            migrationBuilder.DropTable(
                name: "discount_status");

            migrationBuilder.DropTable(
                name: "discount_type");
        }
    }
}

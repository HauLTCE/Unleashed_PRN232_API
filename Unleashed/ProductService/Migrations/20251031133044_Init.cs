using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "brand",
                columns: table => new
                {
                    brand_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    brand_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    brand_description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    brand_image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    brand_website_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    brand_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    brand_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("brand_pkey", x => x.brand_id);
                });

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    category_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    category_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    category_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("category_pkey", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "color",
                columns: table => new
                {
                    color_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    color_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    color_hex_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("color_pkey", x => x.color_id);
                });

            migrationBuilder.CreateTable(
                name: "product_status",
                columns: table => new
                {
                    product_status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_status_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_status_pkey", x => x.product_status_id);
                });

            migrationBuilder.CreateTable(
                name: "size",
                columns: table => new
                {
                    size_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    size_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("size_pkey", x => x.size_id);
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    brand_id = table.Column<int>(type: "int", nullable: true),
                    product_status_id = table.Column<int>(type: "int", nullable: true),
                    product_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    product_code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    product_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    product_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    product_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("product_pkey", x => x.product_id);
                    table.ForeignKey(
                        name: "product_brand_id_fkey",
                        column: x => x.brand_id,
                        principalTable: "brand",
                        principalColumn: "brand_id");
                    table.ForeignKey(
                        name: "product_product_status_id_fkey",
                        column: x => x.product_status_id,
                        principalTable: "product_status",
                        principalColumn: "product_status_id");
                });

            migrationBuilder.CreateTable(
                name: "product_category",
                columns: table => new
                {
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "product_category_category_id_fkey",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "category_id");
                    table.ForeignKey(
                        name: "product_category_product_id_fkey",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id");
                });

            migrationBuilder.CreateTable(
                name: "variation",
                columns: table => new
                {
                    variation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    size_id = table.Column<int>(type: "int", nullable: true),
                    color_id = table.Column<int>(type: "int", nullable: true),
                    variation_image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    variation_price = table.Column<decimal>(type: "decimal(22,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variation_temp", x => x.variation_id);
                    table.ForeignKey(
                        name: "FK1hxfv06p366bhb8sce1djt2v7",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "product_id");
                    table.ForeignKey(
                        name: "FK9gqn7oby75ixq0fg8w902pjcf",
                        column: x => x.color_id,
                        principalTable: "color",
                        principalColumn: "color_id");
                    table.ForeignKey(
                        name: "FKe3asl55h6omj6x27479hnprov",
                        column: x => x.size_id,
                        principalTable: "size",
                        principalColumn: "size_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_product_brand_id",
                table: "product",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_product_status_id",
                table: "product",
                column: "product_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_category_category_id",
                table: "product_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_category_product_id",
                table: "product_category",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_variation_color_id",
                table: "variation",
                column: "color_id");

            migrationBuilder.CreateIndex(
                name: "IX_variation_product_id",
                table: "variation",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_variation_size_id",
                table: "variation",
                column: "size_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "product_category");

            migrationBuilder.DropTable(
                name: "variation");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "color");

            migrationBuilder.DropTable(
                name: "size");

            migrationBuilder.DropTable(
                name: "brand");

            migrationBuilder.DropTable(
                name: "product_status");
        }
    }
}

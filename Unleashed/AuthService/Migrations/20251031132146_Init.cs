using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rank",
                columns: table => new
                {
                    rank_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rank_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rank_num = table.Column<int>(type: "int", nullable: true),
                    rank_payment_requirement = table.Column<decimal>(type: "decimal(22,2)", nullable: true),
                    rank_base_discount = table.Column<decimal>(type: "numeric(3,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rank_pkey", x => x.rank_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("role_pkey", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    is_user_enabled = table.Column<bool>(type: "bit", nullable: true),
                    user_google_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_fullname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    user_birthdate = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_current_payment_method = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    user_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    user_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "user_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "role_id");
                });

            migrationBuilder.CreateTable(
                name: "user_rank",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rank_id = table.Column<int>(type: "int", nullable: false),
                    money_spent = table.Column<decimal>(type: "decimal(22,2)", nullable: true),
                    rank_status = table.Column<short>(type: "smallint", nullable: false),
                    rank_expire_date = table.Column<DateOnly>(type: "date", nullable: false),
                    rank_created_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    rank_updated_date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_rank_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "user_rank_rank_id_fkey",
                        column: x => x.rank_id,
                        principalTable: "rank",
                        principalColumn: "rank_id");
                    table.ForeignKey(
                        name: "user_rank_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_rank_rank_id",
                table: "user_rank",
                column: "rank_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_rank");

            migrationBuilder.DropTable(
                name: "rank");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "role");
        }
    }
}

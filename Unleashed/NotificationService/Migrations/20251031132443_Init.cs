using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationService.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id_sender = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    notification_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    notification_content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_notification_draft = table.Column<bool>(type: "bit", nullable: true),
                    notification_created_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    notification_updated_at = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notification_pkey", x => x.notification_id);
                });

            migrationBuilder.CreateTable(
                name: "notification_user",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_notification_viewed = table.Column<bool>(type: "bit", nullable: true),
                    is_notification_deleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "notification_user_notification_id_fkey",
                        column: x => x.notification_id,
                        principalTable: "notification",
                        principalColumn: "notification_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_notification_id",
                table: "notification_user",
                column: "notification_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification_user");

            migrationBuilder.DropTable(
                name: "notification");
        }
    }
}

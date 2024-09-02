using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciQuery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropToNotificationToUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Notification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Notification",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Notification");
        }
    }
}

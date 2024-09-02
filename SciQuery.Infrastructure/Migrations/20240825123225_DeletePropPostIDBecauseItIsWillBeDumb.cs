using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciQuery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeletePropPostIDBecauseItIsWillBeDumb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Notification");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PostId",
                table: "Notification",
                type: "int",
                nullable: true);
        }
    }
}

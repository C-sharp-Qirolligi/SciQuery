using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciQuery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeNameImagePathtoImagePaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Question",
                newName: "ImagePaths");

            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "Answer",
                newName: "ImagePaths");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePaths",
                table: "Question",
                newName: "ImagePath");

            migrationBuilder.RenameColumn(
                name: "ImagePaths",
                table: "Answer",
                newName: "ImagePath");
        }
    }
}

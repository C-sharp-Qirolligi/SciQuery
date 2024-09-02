using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciQuery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editpropnameuserProfileImagepathtoimagepath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImagePath",
                table: "AspNetUsers",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "AspNetUsers",
                newName: "ProfileImagePath");
        }
    }
}

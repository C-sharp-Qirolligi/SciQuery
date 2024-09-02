using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciQuery.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addVotedUsersToQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasVotedUsersIds",
                table: "Answer",
                newName: "VotedUsersIds");

            migrationBuilder.AddColumn<string>(
                name: "VotedUsersIds",
                table: "Question",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VotedUsersIds",
                table: "Question");

            migrationBuilder.RenameColumn(
                name: "VotedUsersIds",
                table: "Answer",
                newName: "HasVotedUsersIds");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDrive3.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixColunaUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Alunos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Alunos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

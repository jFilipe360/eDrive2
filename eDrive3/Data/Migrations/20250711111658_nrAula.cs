using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDrive3.Data.Migrations
{
    /// <inheritdoc />
    public partial class nrAula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Aulas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Numero",
                table: "Aulas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Aulas");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Aulas");
        }
    }
}

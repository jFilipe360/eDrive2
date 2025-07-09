using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDrive3.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSecretaria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Instrutores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Alunos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Secretarias",
                columns: table => new
                {
                    SecretariaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secretarias", x => x.SecretariaID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Secretarias");

            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Instrutores");

            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Alunos");
        }
    }
}

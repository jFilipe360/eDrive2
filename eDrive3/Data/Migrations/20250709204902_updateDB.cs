using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDrive3.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlunoID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InstrutorID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecretariaID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AlunoID",
                table: "AspNetUsers",
                column: "AlunoID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_InstrutorID",
                table: "AspNetUsers",
                column: "InstrutorID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SecretariaID",
                table: "AspNetUsers",
                column: "SecretariaID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Alunos_AlunoID",
                table: "AspNetUsers",
                column: "AlunoID",
                principalTable: "Alunos",
                principalColumn: "AlunoID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Instrutores_InstrutorID",
                table: "AspNetUsers",
                column: "InstrutorID",
                principalTable: "Instrutores",
                principalColumn: "InstrutorID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Secretarias_SecretariaID",
                table: "AspNetUsers",
                column: "SecretariaID",
                principalTable: "Secretarias",
                principalColumn: "SecretariaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Alunos_AlunoID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Instrutores_InstrutorID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Secretarias_SecretariaID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AlunoID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_InstrutorID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SecretariaID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AlunoID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InstrutorID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecretariaID",
                table: "AspNetUsers");
        }
    }
}

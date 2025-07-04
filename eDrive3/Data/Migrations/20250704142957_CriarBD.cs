using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eDrive3.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriarBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alunos",
                columns: table => new
                {
                    AlunoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NrTelemovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alunos", x => x.AlunoID);
                });

            migrationBuilder.CreateTable(
                name: "Instrutores",
                columns: table => new
                {
                    InstrutorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NrTelemovel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrutores", x => x.InstrutorID);
                });

            migrationBuilder.CreateTable(
                name: "Aulas",
                columns: table => new
                {
                    AulaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    InstrutorID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aulas", x => x.AulaID);
                    table.ForeignKey(
                        name: "FK_Aulas_Instrutores_InstrutorID",
                        column: x => x.InstrutorID,
                        principalTable: "Instrutores",
                        principalColumn: "InstrutorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Presencas",
                columns: table => new
                {
                    PresencaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AlunoID = table.Column<int>(type: "int", nullable: false),
                    AulaID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Presencas", x => x.PresencaID);
                    table.ForeignKey(
                        name: "FK_Presencas_Alunos_AlunoID",
                        column: x => x.AlunoID,
                        principalTable: "Alunos",
                        principalColumn: "AlunoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Presencas_Aulas_AulaID",
                        column: x => x.AulaID,
                        principalTable: "Aulas",
                        principalColumn: "AulaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aulas_InstrutorID",
                table: "Aulas",
                column: "InstrutorID");

            migrationBuilder.CreateIndex(
                name: "IX_Presencas_AlunoID",
                table: "Presencas",
                column: "AlunoID");

            migrationBuilder.CreateIndex(
                name: "IX_Presencas_AulaID",
                table: "Presencas",
                column: "AulaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Presencas");

            migrationBuilder.DropTable(
                name: "Alunos");

            migrationBuilder.DropTable(
                name: "Aulas");

            migrationBuilder.DropTable(
                name: "Instrutores");
        }
    }
}

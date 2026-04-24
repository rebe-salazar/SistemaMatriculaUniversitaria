using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaMatriculaUniversitaria.Data.Migrations
{
    public partial class Fase1_Docentes_Estudiantes_Matricula : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matriculas_Cursos_CursoId",
                table: "Matriculas");

            migrationBuilder.RenameColumn(
                name: "CursoId",
                table: "Matriculas",
                newName: "PeriodoAcademicoId");

            migrationBuilder.RenameIndex(
                name: "IX_Matriculas_CursoId",
                table: "Matriculas",
                newName: "IX_Matriculas_PeriodoAcademicoId");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Matriculas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EstudianteId1",
                table: "Matriculas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cedula",
                table: "Estudiantes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Estudiantes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Docentes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Docentes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Cursos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DocenteId",
                table: "Cursos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DetallesMatricula",
                columns: table => new
                {
                    DetalleMatriculaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatriculaId = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesMatricula", x => x.DetalleMatriculaId);
                    table.ForeignKey(
                        name: "FK_DetallesMatricula_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "CursoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesMatricula_Matriculas_MatriculaId",
                        column: x => x.MatriculaId,
                        principalTable: "Matriculas",
                        principalColumn: "MatriculaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosAcademicos",
                columns: table => new
                {
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosAcademicos", x => x.PeriodoAcademicoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_EstudianteId1",
                table: "Matriculas",
                column: "EstudianteId1");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_DocenteId",
                table: "Cursos",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesMatricula_CursoId",
                table: "DetallesMatricula",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesMatricula_MatriculaId",
                table: "DetallesMatricula",
                column: "MatriculaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cursos_Docentes_DocenteId",
                table: "Cursos",
                column: "DocenteId",
                principalTable: "Docentes",
                principalColumn: "DocenteId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Matriculas_Estudiantes_EstudianteId1",
                table: "Matriculas",
                column: "EstudianteId1",
                principalTable: "Estudiantes",
                principalColumn: "EstudianteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matriculas_PeriodosAcademicos_PeriodoAcademicoId",
                table: "Matriculas",
                column: "PeriodoAcademicoId",
                principalTable: "PeriodosAcademicos",
                principalColumn: "PeriodoAcademicoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cursos_Docentes_DocenteId",
                table: "Cursos");

            migrationBuilder.DropForeignKey(
                name: "FK_Matriculas_Estudiantes_EstudianteId1",
                table: "Matriculas");

            migrationBuilder.DropForeignKey(
                name: "FK_Matriculas_PeriodosAcademicos_PeriodoAcademicoId",
                table: "Matriculas");

            migrationBuilder.DropTable(
                name: "DetallesMatricula");

            migrationBuilder.DropTable(
                name: "PeriodosAcademicos");

            migrationBuilder.DropIndex(
                name: "IX_Matriculas_EstudianteId1",
                table: "Matriculas");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_DocenteId",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Matriculas");

            migrationBuilder.DropColumn(
                name: "EstudianteId1",
                table: "Matriculas");

            migrationBuilder.DropColumn(
                name: "Cedula",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "DocenteId",
                table: "Cursos");

            migrationBuilder.RenameColumn(
                name: "PeriodoAcademicoId",
                table: "Matriculas",
                newName: "CursoId");

            migrationBuilder.RenameIndex(
                name: "IX_Matriculas_PeriodoAcademicoId",
                table: "Matriculas",
                newName: "IX_Matriculas_CursoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matriculas_Cursos_CursoId",
                table: "Matriculas",
                column: "CursoId",
                principalTable: "Cursos",
                principalColumn: "CursoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

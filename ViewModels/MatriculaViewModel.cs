using System.ComponentModel.DataAnnotations;
using SistemaMatriculaUniversitaria.Models;

namespace SistemaMatriculaUniversitaria.ViewModels
{
    public class MatriculaViewModel
    {
        // Aquí guardamos el periodo académico seleccionado
        [Required(ErrorMessage = "Debe seleccionar un período académico")]
        public int PeriodoAcademicoId { get; set; }

        // Aquí se guardan los cursos marcados por el estudiante
        public List<int> CursosSeleccionados { get; set; } = new List<int>();

        // Lista de cursos que se muestran en la vista
        public List<Curso> CursosDisponibles { get; set; } = new List<Curso>();

        // Lista de periodos disponibles
        public List<PeriodoAcademico> Periodos { get; set; } = new List<PeriodoAcademico>();
    }
}
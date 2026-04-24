using System.ComponentModel.DataAnnotations;

namespace SistemaMatriculaUniversitaria.Models
{
    public class PeriodoAcademico
    {
        public int PeriodoAcademicoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty; // Ej: I Cuatrimestre 2026

        public bool Activo { get; set; } = true;

        public ICollection<Matricula>? Matriculas { get; set; }
    }
}
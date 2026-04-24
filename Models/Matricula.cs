using System.ComponentModel.DataAnnotations;

namespace SistemaMatriculaUniversitaria.Models
{
    public class Matricula
    {
        public int MatriculaId { get; set; }

        [Required]
        public int EstudianteId { get; set; }
        public Estudiante? Estudiante { get; set; }

        [Required]
        public int PeriodoAcademicoId { get; set; }
        public PeriodoAcademico? PeriodoAcademico { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaMatricula { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Estado { get; set; } = "Activa";

        public ICollection<DetalleMatricula>? Detalles { get; set; }
    }
}
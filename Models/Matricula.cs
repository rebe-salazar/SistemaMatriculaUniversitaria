using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMatriculaUniversitaria.Models
{
    public class Matricula
    {
        [Key]
        public int MatriculaId { get; set; }

        [ForeignKey("Estudiante")]
        public int EstudianteId { get; set; }

        public Estudiante? Estudiante { get; set; }

        [ForeignKey("Curso")]
        public int CursoId { get; set; }

        public Curso? Curso { get; set; }

        [Required]
        public DateTime FechaMatricula { get; set; } = DateTime.Now;
    }
}
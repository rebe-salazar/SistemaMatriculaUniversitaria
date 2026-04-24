using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMatriculaUniversitaria.Models
{
    public class Curso
    {
        [Key]
        public int CursoId { get; set; }

        [Required(ErrorMessage = "El nombre del curso es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [Range(1, 10, ErrorMessage = "Los créditos deben estar entre 1 y 10")]
        public int Creditos { get; set; }

        [ForeignKey("Carrera")]
        [Required(ErrorMessage = "La carrera es obligatoria")]
        public int CarreraId { get; set; }

        public Carrera? Carrera { get; set; }
        [Display(Name = "Docente asignado")]
        public int? DocenteId { get; set; }
        public Docente? Docente { get; set; }
    }
}
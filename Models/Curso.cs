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
        public string Nombre { get; set; }

        [Required]
        public int Creditos { get; set; }

        [ForeignKey("Carrera")]
        public int CarreraId { get; set; }

        public Carrera? Carrera { get; set; }
    }
}
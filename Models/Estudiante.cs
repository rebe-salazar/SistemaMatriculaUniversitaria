using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaMatriculaUniversitaria.Models
{
    public class Estudiante
    {
        [Key]
        public int EstudianteId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Cedula { get; set; } = string.Empty;
        [Required]

        [ForeignKey("Carrera")]
        public int CarreraId { get; set; }

        public Carrera? Carrera { get; set; }

        // Relación opcional con Identity
        public string? UsuarioId { get; set; }

        public ICollection<Matricula>? Matriculas { get; set; }
    }
}
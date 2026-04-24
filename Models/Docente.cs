using System.ComponentModel.DataAnnotations;

namespace SistemaMatriculaUniversitaria.Models
{
    public class Docente
    {
        public int DocenteId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        [StringLength(100)]
        public string Especialidad { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        // Relación: un docente puede impartir varios cursos
        public ICollection<Curso>? Cursos { get; set; }
    }
}
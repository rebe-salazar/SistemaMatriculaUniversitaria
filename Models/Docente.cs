using System.ComponentModel.DataAnnotations;

namespace SistemaMatriculaUniversitaria.Models
{
    public class Docente
    {
        [Key]
        public int DocenteId { get; set; }

        [Required(ErrorMessage = "El nombre del docente es obligatorio.")]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [StringLength(100)]
        public string Especialidad { get; set; }
    }
}
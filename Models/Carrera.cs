using System.ComponentModel.DataAnnotations;

namespace SistemaMatriculaUniversitaria.Models
{
    public class Carrera
    {
        [Key]
        public int CarreraId { get; set; }

        [Required(ErrorMessage = "El nombre de la carrera es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(250)]
        public string Descripcion { get; set; }
    }
}
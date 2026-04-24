namespace SistemaMatriculaUniversitaria.Models
{
    public class DetalleMatricula
    {
        public int DetalleMatriculaId { get; set; }

        public int MatriculaId { get; set; }
        public Matricula? Matricula { get; set; }

        public int CursoId { get; set; }
        public Curso? Curso { get; set; }
    }
}
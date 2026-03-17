using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Models;

namespace SistemaMatriculaUniversitaria.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Tablas principales del sistema
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relación Curso -> Carrera
            // Se desactiva el borrado en cascada para evitar rutas múltiples
            builder.Entity<Curso>()
                .HasOne(c => c.Carrera)
                .WithMany()
                .HasForeignKey(c => c.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Estudiante -> Carrera
            // Se desactiva el borrado en cascada para evitar rutas múltiples
            builder.Entity<Estudiante>()
                .HasOne(e => e.Carrera)
                .WithMany()
                .HasForeignKey(e => e.CarreraId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Matricula -> Estudiante
            // Se desactiva el borrado en cascada para evitar conflictos
            builder.Entity<Matricula>()
                .HasOne(m => m.Estudiante)
                .WithMany()
                .HasForeignKey(m => m.EstudianteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Matricula -> Curso
            // Se desactiva el borrado en cascada para evitar conflictos
            builder.Entity<Matricula>()
                .HasOne(m => m.Curso)
                .WithMany()
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
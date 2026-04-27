using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Data;
using SistemaMatriculaUniversitaria.Models;
using SistemaMatriculaUniversitaria.ViewModels;

namespace SistemaMatriculaUniversitaria.Controllers
{
    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public MatriculasController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // Muestra el formulario para seleccionar cursos
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var modelo = new MatriculaViewModel
            {
                CursosDisponibles = await _contexto.Cursos
                    .Include(c => c.Carrera)
                    .Include(c => c.Docente)
                    .ToListAsync(),

                Periodos = await _contexto.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .ToListAsync()
            };

            return View(modelo);
        }

        // Guarda la matrícula y los cursos seleccionados
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MatriculaViewModel modelo)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var estudiante = await _contexto.Estudiantes
                .FirstOrDefaultAsync(e => e.UsuarioId == userId);

            if (estudiante == null)
            {
                ModelState.AddModelError("", "El usuario no tiene perfil de estudiante.");

                modelo.CursosDisponibles = await _contexto.Cursos
                    .Include(c => c.Carrera)
                    .Include(c => c.Docente)
                    .ToListAsync();

                modelo.Periodos = await _contexto.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .ToListAsync();

                return View(modelo);
            }

            if (modelo.CursosSeleccionados == null || !modelo.CursosSeleccionados.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un curso.");
            }
            // Obtener los cursos que el estudiante seleccionó
            var cursosSeleccionados = await _contexto.Cursos
                .Where(c => modelo.CursosSeleccionados.Contains(c.CursoId))
                .ToListAsync();
            // Validar duplicados en la selección actual
            if (modelo.CursosSeleccionados
                .GroupBy(c => c)
                .Any(g => g.Count() > 1))
            {
                ModelState.AddModelError("", "No puede seleccionar el mismo curso más de una vez.");
            }
            //  Validar límite de créditos
            int totalCreditos = cursosSeleccionados.Sum(c => c.Creditos);
            int limiteCreditos = 18;

            if (totalCreditos > limiteCreditos)
            {
                ModelState.AddModelError("", $"No puede matricular más de {limiteCreditos} créditos. Actualmente seleccionó {totalCreditos} créditos.");
            }

            //  Validar cursos ya matriculados anteriormente
            var cursosYaMatriculados = await _contexto.DetallesMatricula
                .Include(d => d.Matricula)
                .Include(d => d.Curso)
                .Where(d => d.Matricula.EstudianteId == estudiante.EstudianteId
                            && d.Matricula.Estado == "Activa"
                            && modelo.CursosSeleccionados.Contains(d.CursoId))
                .Select(d => d.Curso.Nombre)
                .ToListAsync();

            if (cursosYaMatriculados.Any())
            {
                foreach (var nombreCurso in cursosYaMatriculados)
                {
                    ModelState.AddModelError("", $"Ya matriculaste este curso: {nombreCurso}");
                }
            }
            if (!ModelState.IsValid)
            {
                modelo.CursosDisponibles = await _contexto.Cursos
                    .Include(c => c.Carrera)
                    .Include(c => c.Docente)
                    .ToListAsync();

                modelo.Periodos = await _contexto.PeriodosAcademicos
                    .Where(p => p.Activo)
                    .ToListAsync();

                return View(modelo);
            }

            var matricula = new Matricula
            {
                EstudianteId = estudiante.EstudianteId,
                PeriodoAcademicoId = modelo.PeriodoAcademicoId,
                FechaMatricula = DateTime.Now,
                Estado = "Activa"
            };

            _contexto.Matriculas.Add(matricula);
            await _contexto.SaveChangesAsync();

            foreach (var cursoId in modelo.CursosSeleccionados)
            {
                var detalle = new DetalleMatricula
                {
                    MatriculaId = matricula.MatriculaId,
                    CursoId = cursoId
                };

                _contexto.DetallesMatricula.Add(detalle);
            }

            await _contexto.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = matricula.MatriculaId });
        }

        // Muestra el resumen de la matrícula realizada
        public async Task<IActionResult> Details(int id)
        {
            var matricula = await _contexto.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.PeriodoAcademico)
                .Include(m => m.Detalles)
                    .ThenInclude(d => d.Curso)
                        .ThenInclude(c => c.Docente)
                .FirstOrDefaultAsync(m => m.MatriculaId == id);

            if (matricula == null)
            {
                return NotFound();
            }

            return View(matricula);
        }
        public async Task<IActionResult> Index()
        {
            // Obtener usuario logueado
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Buscar estudiante asociado
            var estudiante = await _contexto.Estudiantes
                .FirstOrDefaultAsync(e => e.UsuarioId == userId);

            if (estudiante == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Filtrar SOLO sus matrículas
            var matriculas = await _contexto.Matriculas
                .Include(m => m.Estudiante)
                .Include(m => m.PeriodoAcademico)
                .Include(m => m.Detalles)
                    .ThenInclude(d => d.Curso)
                .Where(m => m.EstudianteId == estudiante.EstudianteId)
                .OrderByDescending(m => m.FechaMatricula)
                .ToListAsync();

            return View(matriculas);
        }
    }

}
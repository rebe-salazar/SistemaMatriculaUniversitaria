using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Data;
using SistemaMatriculaUniversitaria.Models;

namespace SistemaMatriculaUniversitaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public CursosController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // LISTAR CURSOS CON FILTRO Y PAGINACIÓN
        public async Task<IActionResult> Index(int? docenteId, string? buscar, int pagina = 1)
        {
            int registrosPorPagina = 10;

            var cursos = _contexto.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .AsQueryable();

            // Filtro por docente seleccionado en dropdown
            if (docenteId.HasValue)
            {
                cursos = cursos.Where(c => c.DocenteId == docenteId);
            }

            // Filtro por texto: busca por curso, código, carrera o docente
            // Filtro por texto: busca por curso, código, carrera o docente
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                cursos = cursos.Where(c =>
                    c.Nombre.Contains(buscar) ||
                    c.Codigo.Contains(buscar) ||
                    (c.Carrera != null && c.Carrera.Nombre.Contains(buscar)) ||
                    (c.Docente != null && c.Docente.NombreCompleto.ToLower().Contains(buscar))
                );
            }

            int totalRegistros = await cursos.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);

            var cursosPaginados = await cursos
                .OrderBy(c => c.Nombre)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewBag.Docentes = new SelectList(
                await _contexto.Docentes
                    .Where(d => d.Activo)
                    .OrderBy(d => d.NombreCompleto)
                    .ToListAsync(),
                "DocenteId",
                "NombreCompleto",
                docenteId
            );

            ViewBag.Buscar = buscar;
            ViewBag.DocenteId = docenteId;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(cursosPaginados);
        }

        // VER DETALLE DE UN CURSO
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _contexto.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null) return NotFound();

            return View(curso);
        }

        // MOSTRAR FORMULARIO DE CREACIÓN
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarCarrerasAsync();
            await CargarDocentesAsync();

            return View();
        }

        // GUARDAR NUEVO CURSO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (ModelState.IsValid)
            {
                _contexto.Cursos.Add(curso);
                await _contexto.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await CargarCarrerasAsync(curso.CarreraId);
            await CargarDocentesAsync(curso.DocenteId);

            return View(curso);
        }

        // MOSTRAR FORMULARIO DE EDICIÓN
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _contexto.Cursos.FindAsync(id);

            if (curso == null) return NotFound();

            await CargarCarrerasAsync(curso.CarreraId);
            await CargarDocentesAsync(curso.DocenteId);

            return View(curso);
        }

        // GUARDAR CAMBIOS DE EDICIÓN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.CursoId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _contexto.Cursos.Update(curso);
                    await _contexto.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExiste(curso.CursoId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await CargarCarrerasAsync(curso.CarreraId);
            await CargarDocentesAsync(curso.DocenteId);

            return View(curso);
        }

        // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _contexto.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null) return NotFound();

            return View(curso);
        }

        // ELIMINAR CURSO
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _contexto.Cursos.FindAsync(id);

            if (curso != null)
            {
                _contexto.Cursos.Remove(curso);
                await _contexto.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CursoExiste(int id)
        {
            return _contexto.Cursos.Any(c => c.CursoId == id);
        }

        private async Task CargarCarrerasAsync(object? carreraSeleccionada = null)
        {
            var carreras = await _contexto.Carreras
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.CarreraId = new SelectList(carreras, "CarreraId", "Nombre", carreraSeleccionada);
        }

        private async Task CargarDocentesAsync(object? docenteSeleccionado = null)
        {
            var docentes = await _contexto.Docentes
                .Where(d => d.Activo)
                .OrderBy(d => d.NombreCompleto)
                .ToListAsync();

            ViewBag.DocenteId = new SelectList(docentes, "DocenteId", "NombreCompleto", docenteSeleccionado);
        }
    }
}
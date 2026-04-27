using Microsoft.AspNetCore.Authorization; // Permite restringir acceso por roles
using Microsoft.AspNetCore.Mvc; // Base para controladores MVC
using Microsoft.AspNetCore.Mvc.Rendering; // Para dropdowns (SelectList)
using Microsoft.EntityFrameworkCore; // Para consultas con Include, async, etc
using SistemaMatriculaUniversitaria.Data; // Contexto de base de datos
using SistemaMatriculaUniversitaria.Models; // Modelos (Curso, Docente, etc)

namespace SistemaMatriculaUniversitaria.Controllers
{
    // Solo administradores pueden acceder a este controlador
    [Authorize(Roles = "Administrador")]
    public class CursosController : Controller
    {
        // Contexto de base de datos (inyección de dependencias)
        private readonly ApplicationDbContext _contexto;

        public CursosController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // ============================================
        // LISTAR CURSOS
        // ============================================
        public async Task<IActionResult> Index(int? docenteId)
        {
            var cursos = _contexto.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .AsQueryable();

            // Filtro por docente
            if (docenteId.HasValue)
            {
                cursos = cursos.Where(c => c.DocenteId == docenteId);
            }

            // Cargar docentes para el filtro
            ViewBag.Docentes = new SelectList(
                await _contexto.Docentes.Where(d => d.Activo).ToListAsync(),
                "DocenteId",
                "NombreCompleto",
                docenteId
            );

            return View(await cursos.ToListAsync());
        }

        // ============================================
        // VER DETALLE DE UN CURSO
        // ============================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Busca el curso incluyendo relaciones
            var curso = await _contexto.Cursos
                .Include(c => c.Carrera)
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null) return NotFound();

            return View(curso);
        }

        // ============================================
        // MOSTRAR FORMULARIO DE CREACIÓN
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Cargar dropdown de carreras
            await CargarCarrerasAsync();

            // Cargar dropdown de docentes
            await CargarDocentesAsync();

            return View();
        }

        // ============================================
        // GUARDAR NUEVO CURSO
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            // Si los datos del formulario son válidos
            if (ModelState.IsValid)
            {
                _contexto.Cursos.Add(curso);
                await _contexto.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, recargar dropdowns
            await CargarCarrerasAsync(curso.CarreraId);
            await CargarDocentesAsync(curso.DocenteId);

            return View(curso);
        }

        // ============================================
        // MOSTRAR FORMULARIO DE EDICIÓN
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var curso = await _contexto.Cursos.FindAsync(id);

            if (curso == null) return NotFound();

            // Cargar dropdowns con valores seleccionados
            await CargarCarrerasAsync(curso.CarreraId);
            await CargarDocentesAsync(curso.DocenteId);

            return View(curso);
        }

        // ============================================
        // GUARDAR CAMBIOS DE EDICIÓN
        // ============================================
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

            // Si hay error, volver a cargar dropdowns
            await CargarCarrerasAsync(curso.CarreraId);
            await CargarDocentesAsync(curso.DocenteId);

            return View(curso);
        }

        // ============================================
        // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
        // ============================================
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

        // ============================================
        // ELIMINAR CURSO
        // ============================================
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

        // ============================================
        // VALIDAR EXISTENCIA
        // ============================================
        private bool CursoExiste(int id)
        {
            return _contexto.Cursos.Any(c => c.CursoId == id);
        }

        // ============================================
        // CARGAR CARRERAS EN DROPDOWN
        // ============================================
        private async Task CargarCarrerasAsync(object? carreraSeleccionada = null)
        {
            var carreras = await _contexto.Carreras
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            // Se envía al ViewBag para usar en la vista
            ViewBag.CarreraId = new SelectList(carreras, "CarreraId", "Nombre", carreraSeleccionada);
        }

        // ============================================
        // CARGAR DOCENTES EN DROPDOWN
        // ============================================
        private async Task CargarDocentesAsync(object? docenteSeleccionado = null)
        {
            var docentes = await _contexto.Docentes
                .Where(d => d.Activo) // Solo docentes activos
                .OrderBy(d => d.NombreCompleto)
                .ToListAsync();

            ViewBag.DocenteId = new SelectList(docentes, "DocenteId", "NombreCompleto", docenteSeleccionado);
        }
    }
}
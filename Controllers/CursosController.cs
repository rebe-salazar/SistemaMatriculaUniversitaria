using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Data;
using SistemaMatriculaUniversitaria.Models;

namespace SistemaMatriculaUniversitaria.Controllers
{
    // Solo el administrador puede gestionar cursos
    [Authorize(Roles = "Administrador")]
    public class CursosController : Controller
    {
        // Contexto de base de datos
        private readonly ApplicationDbContext _contexto;

        public CursosController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // =====================================
        // LISTAR TODOS LOS CURSOS
        // =====================================
        public async Task<IActionResult> Index()
        {
            var cursos = await _contexto.Cursos
                .Include(c => c.Carrera)
                .ToListAsync();

            return View(cursos);
        }

        // =====================================
        // VER DETALLE DE UN CURSO
        // =====================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _contexto.Cursos
                .Include(c => c.Carrera)
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // =====================================
        // MOSTRAR FORMULARIO DE CREACIÓN
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Carga la lista de carreras para el dropdown
            await CargarCarrerasAsync();
            return View();
        }

        // =====================================
        // GUARDAR NUEVO CURSO
        // =====================================
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

            // Si hubo error, vuelve a cargar carreras y regresa a la vista
            await CargarCarrerasAsync(curso.CarreraId);
            return View(curso);
        }

        // =====================================
        // MOSTRAR FORMULARIO DE EDICIÓN
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _contexto.Cursos.FindAsync(id);

            if (curso == null)
            {
                return NotFound();
            }

            await CargarCarrerasAsync(curso.CarreraId);
            return View(curso);
        }

        // =====================================
        // GUARDAR CAMBIOS DE EDICIÓN
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.CursoId)
            {
                return NotFound();
            }

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
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            await CargarCarrerasAsync(curso.CarreraId);
            return View(curso);
        }

        // =====================================
        // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
        // =====================================
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _contexto.Cursos
                .Include(c => c.Carrera)
                .FirstOrDefaultAsync(c => c.CursoId == id);

            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // =====================================
        // ELIMINAR DEFINITIVAMENTE
        // =====================================
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

        // =====================================
        // MÉTODO PRIVADO PARA VALIDAR EXISTENCIA
        // =====================================
        private bool CursoExiste(int id)
        {
            return _contexto.Cursos.Any(c => c.CursoId == id);
        }

        // =====================================
        // MÉTODO PRIVADO PARA CARGAR CARRERAS
        // =====================================
        private async Task CargarCarrerasAsync(object? carreraSeleccionada = null)
        {
            var carreras = await _contexto.Carreras
                .OrderBy(c => c.Nombre)
                .ToListAsync();

            ViewBag.CarreraId = new SelectList(carreras, "CarreraId", "Nombre", carreraSeleccionada);
        }
    }
}

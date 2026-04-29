using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Data;
using SistemaMatriculaUniversitaria.Models;

namespace SistemaMatriculaUniversitaria.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DocentesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocentesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Docentes
        public async Task<IActionResult> Index(string? buscar, bool? activo, int pagina = 1)
        {
            if (_context.Docentes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Docentes' is null.");
            }

            int registrosPorPagina = 10;

            var docentes = _context.Docentes.AsQueryable();

            // Filtro por texto: permite buscar por nombre, correo o especialidad.
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                docentes = docentes.Where(d =>
                    d.NombreCompleto.Contains(buscar) ||
                    d.Correo.Contains(buscar) ||
                    d.Especialidad.Contains(buscar)
                );
            }

            // Filtro por estado: permite mostrar solo docentes activos o inactivos.
            if (activo.HasValue)
            {
                docentes = docentes.Where(d => d.Activo == activo.Value);
            }

            int totalRegistros = await docentes.CountAsync();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)registrosPorPagina);

            var docentesPaginados = await docentes
                .OrderBy(d => d.NombreCompleto)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewBag.Buscar = buscar;
            ViewBag.Activo = activo;
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(docentesPaginados);
        }

        // GET: Docentes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Docentes == null)
            {
                return NotFound();
            }

            var docente = await _context.Docentes
                .FirstOrDefaultAsync(m => m.DocenteId == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        // GET: Docentes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Docentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DocenteId,NombreCompleto,Correo,Especialidad,Activo")] Docente docente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(docente);
        }

        // GET: Docentes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Docentes == null)
            {
                return NotFound();
            }

            var docente = await _context.Docentes.FindAsync(id);
            if (docente == null)
            {
                return NotFound();
            }
            return View(docente);
        }

        // POST: Docentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DocenteId,NombreCompleto,Correo,Especialidad,Activo")] Docente docente)
        {
            if (id != docente.DocenteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(docente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocenteExists(docente.DocenteId))
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
            return View(docente);
        }

        // GET: Docentes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Docentes == null)
            {
                return NotFound();
            }

            var docente = await _context.Docentes
                .FirstOrDefaultAsync(m => m.DocenteId == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        // POST: Docentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Docentes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Docentes'  is null.");
            }
            var docente = await _context.Docentes.FindAsync(id);
            if (docente != null)
            {
                _context.Docentes.Remove(docente);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocenteExists(int id)
        {
          return (_context.Docentes?.Any(e => e.DocenteId == id)).GetValueOrDefault();
        }
    }
}

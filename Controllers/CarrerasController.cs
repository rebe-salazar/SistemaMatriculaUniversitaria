using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Data;
using SistemaMatriculaUniversitaria.Models;

namespace SistemaMatriculaUniversitaria.Controllers
{
    // Solo los usuarios con rol Administrador pueden acceder a este controlador
    [Authorize(Roles = "Administrador")]
    public class CarrerasController : Controller
    {
        // Contexto de base de datos para acceder a la tabla Carreras
        private readonly ApplicationDbContext _contexto;

        // Constructor que recibe el contexto por inyección de dependencias
        public CarrerasController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // ==============================
        // LISTAR TODAS LAS CARRERAS
        // ==============================
        public async Task<IActionResult> Index()
        {
            // Obtiene todas las carreras de la base de datos
            var carreras = await _contexto.Carreras.ToListAsync();

            // Envía la lista a la vista
            return View(carreras);
        }

        // ==============================
        // VER DETALLE DE UNA CARRERA
        // ==============================
        public async Task<IActionResult> Details(int? id)
        {
            // Verifica que el id sí venga
            if (id == null)
            {
                return NotFound();
            }

            // Busca la carrera por su id
            var carrera = await _contexto.Carreras
                .FirstOrDefaultAsync(c => c.CarreraId == id);

            // Si no existe, devuelve error 404
            if (carrera == null)
            {
                return NotFound();
            }

            return View(carrera);
        }

        // ==============================
        // MOSTRAR FORMULARIO DE CREACIÓN
        // ==============================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ==============================
        // GUARDAR NUEVA CARRERA
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Carrera carrera)
        {
            // Verifica si el formulario cumple las validaciones del modelo
            if (ModelState.IsValid)
            {
                // Agrega la nueva carrera al contexto
                _contexto.Carreras.Add(carrera);

                // Guarda cambios en la base de datos
                await _contexto.SaveChangesAsync();

                // Redirige al listado
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, vuelve a mostrar el formulario
            return View(carrera);
        }

        // ==============================
        // MOSTRAR FORMULARIO DE EDICIÓN
        // ==============================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            // Verifica que el id sí venga
            if (id == null)
            {
                return NotFound();
            }

            // Busca la carrera por id
            var carrera = await _contexto.Carreras.FindAsync(id);

            // Si no existe, devuelve 404
            if (carrera == null)
            {
                return NotFound();
            }

            return View(carrera);
        }

        // ==============================
        // GUARDAR CAMBIOS DE EDICIÓN
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Carrera carrera)
        {
            // Verifica que el id de la URL coincida con el del objeto
            if (id != carrera.CarreraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualiza la carrera
                    _contexto.Carreras.Update(carrera);

                    // Guarda cambios
                    await _contexto.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Si la carrera ya no existe, devuelve error
                    if (!CarreraExiste(carrera.CarreraId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Regresa al listado
                return RedirectToAction(nameof(Index));
            }

            return View(carrera);
        }

        // ==============================
        // MOSTRAR CONFIRMACIÓN DE ELIMINACIÓN
        // ==============================
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrera = await _contexto.Carreras
                .FirstOrDefaultAsync(c => c.CarreraId == id);

            if (carrera == null)
            {
                return NotFound();
            }

            return View(carrera);
        }

        // ==============================
        // ELIMINAR DEFINITIVAMENTE
        // ==============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carrera = await _contexto.Carreras.FindAsync(id);

            if (carrera != null)
            {
                _contexto.Carreras.Remove(carrera);
                await _contexto.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ==============================
        // MÉTODO PRIVADO DE APOYO -- verifica que existan carreras
        // ==============================
        private bool CarreraExiste(int id)
        {
            return _contexto.Carreras.Any(c => c.CarreraId == id);
        }
    }
}
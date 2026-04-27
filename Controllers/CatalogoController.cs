using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Data;

namespace SistemaMatriculaUniversitaria.Controllers
{
    public class CatalogoController : Controller
    {
        // Contexto para consultar las carreras registradas
        private readonly ApplicationDbContext _contexto;

        public CatalogoController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        // Muestra las carreras disponibles para cualquier usuario
        public async Task<IActionResult> Index()
        {
            var carreras = await _contexto.Carreras.ToListAsync();
            return View(carreras);
        }
    }
}
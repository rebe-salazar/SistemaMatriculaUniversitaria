using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SistemaMatriculaUniversitaria.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public ModeloEntrada Input { get; set; } = new ModeloEntrada();

        public class ModeloEntrada
        {
            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
            public string Correo { get; set; } = string.Empty;

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Contrasena { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Contrasena", ErrorMessage = "La contraseña y su confirmación no coinciden.")]
            public string ConfirmarContrasena { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuario = new IdentityUser
            {
                UserName = Input.Correo,
                Email = Input.Correo,
                EmailConfirmed = true
            };

            var resultado = await _userManager.CreateAsync(usuario, Input.Contrasena);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("Se creó una nueva cuenta de usuario.");

                await _signInManager.SignInAsync(usuario, isPersistent: false);

                return LocalRedirect("~/");
            }

            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, TraducirErrorIdentity(error.Description));
            }

            return Page();
        }

        private string TraducirErrorIdentity(string mensaje)
        {
            if (mensaje.Contains("Passwords must have at least one non alphanumeric character"))
                return "La contraseña debe contener al menos un carácter especial.";

            if (mensaje.Contains("Passwords must have at least one digit"))
                return "La contraseña debe contener al menos un número.";

            if (mensaje.Contains("Passwords must have at least one uppercase"))
                return "La contraseña debe contener al menos una letra mayúscula.";

            if (mensaje.Contains("Passwords must have at least one lowercase"))
                return "La contraseña debe contener al menos una letra minúscula.";

            if (mensaje.Contains("is already taken") || mensaje.Contains("is already registered"))
                return "Ese correo electrónico ya está registrado.";

            return mensaje;
        }
    }
}
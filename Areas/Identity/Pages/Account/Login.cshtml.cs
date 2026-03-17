using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SistemaMatriculaUniversitaria.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public ModeloEntrada Input { get; set; } = new ModeloEntrada();

        public string ReturnUrl { get; set; } = string.Empty;

        public class ModeloEntrada
        {
            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
            public string Correo { get; set; } = string.Empty;

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string Contrasena { get; set; } = string.Empty;

            public bool Recordarme { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl ?? Url.Content("~/") ?? "/";
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/") ?? "/";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                Input.Correo,
                Input.Contrasena,
                Input.Recordarme,
                lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                _logger.LogInformation("El usuario inició sesión correctamente.");
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
            return Page();
        }
    }
}
using Microsoft.AspNetCore.Identity;

namespace SistemaMatriculaUniversitaria.DatosIniciales
{
    public static class InicializadorRoles
    {
        public static async Task CrearRolesYUsuarioAdminAsync(IServiceProvider servicios)
        {
            var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = servicios.GetRequiredService<UserManager<IdentityUser>>();

            string nombreRol = "Administrador";
            string correoAdmin = "admin@universidad.ac.cr";
            string contrasenaAdmin = "Admin123!";

            // Crear el rol Administrador si no existe
            if (!await roleManager.RoleExistsAsync(nombreRol))
            {
                await roleManager.CreateAsync(new IdentityRole(nombreRol));
            }

            // Buscar si ya existe el usuario administrador
            var usuarioAdmin = await userManager.FindByEmailAsync(correoAdmin);

            if (usuarioAdmin == null)
            {
                usuarioAdmin = new IdentityUser
                {
                    UserName = correoAdmin,
                    Email = correoAdmin,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuarioAdmin, contrasenaAdmin);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuarioAdmin, nombreRol);
                }
            }
            else
            {
                // Si el usuario ya existe pero no tiene el rol, se le asigna
                if (!await userManager.IsInRoleAsync(usuarioAdmin, nombreRol))
                {
                    await userManager.AddToRoleAsync(usuarioAdmin, nombreRol);
                }
            }
        }
    }
}
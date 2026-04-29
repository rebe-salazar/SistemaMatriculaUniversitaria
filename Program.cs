using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaMatriculaUniversitaria.Data;
using SistemaMatriculaUniversitaria.DatosIniciales;

var builder = WebApplication.CreateBuilder(args);

// Conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    await InicializadorRoles.CrearRolesYUsuarioAdminAsync(servicios);
}
app.Run();

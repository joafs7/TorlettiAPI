using CatalogoAPI.Data;

namespace CatalogoAPI.Services;

public static class SeederService
{
    public static async Task InicializarAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Crear base de datos si no existe
        await db.Database.EnsureCreatedAsync();

        // Aquí van tus seeds: roles, catálogos, superadmin, etc.
        // Ejemplo:
        // if (!await db.Roles.AnyAsync())
        // {
        //     db.Roles.Add(new Role { Nombre = "Admin" });
        //     await db.SaveChangesAsync();
        // }
    }
}
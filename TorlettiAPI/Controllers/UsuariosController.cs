using CatalogoAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CatalogoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "superadmin,admin")]// solo el superadmin accede a estos endpoints
public class UsuariosController(AppDbContext db) : ControllerBase
{
    private readonly PasswordHasher<string> _hasher = new();

    // ── GET api/usuarios ───────────────────────────────────
    // Ver todos los admins creados
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var admins = await db.Usuarios
            .Include(u => u.Rol)
            .Where(u => u.Rol.Nombre == "admin")
            .Select(u => new
            {
                u.IdUsuario,
                u.Nombre,
                u.Apellido,
                u.Email,
                u.Activo,
                u.CreadoEn
            })
            .ToListAsync();

        return Ok(admins);
    }

    // ── POST api/usuarios ──────────────────────────────────
    // Crear un nuevo admin
    [HttpPost]
    public async Task<IActionResult> CrearAdmin([FromBody] CrearAdminRequest req)
    {
        if (await db.Usuarios.AnyAsync(u => u.Email == req.Email))
            return Conflict(new { mensaje = "Ya existe un usuario con ese email." });

        var rolAdmin = await db.Roles.FirstOrDefaultAsync(r => r.Nombre == "admin");
        if (rolAdmin is null)
            return StatusCode(500, new { mensaje = "No se encontró el rol admin en la BD." });

        var admin = new Models.Usuario
        {
            IdRol        = rolAdmin.IdRol,
            Nombre       = req.Nombre,
            Apellido     = req.Apellido,
            Email        = req.Email,
            PasswordHash = _hasher.HashPassword(req.Email, req.Password),
            Activo       = true
        };

        db.Usuarios.Add(admin);
        await db.SaveChangesAsync();

        return Ok(new { mensaje = $"Admin {req.Nombre} {req.Apellido} creado correctamente." });
    }

    // ── PUT api/usuarios/5/desactivar ──────────────────────
    // Desactivar un admin (baja lógica)
    [HttpPut("{id}/desactivar")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound();

        // Evitar que el superadmin se desactive a sí mismo
        var idSuperadmin = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (usuario.IdUsuario == idSuperadmin)
            return BadRequest(new { mensaje = "No podés desactivarte a vos mismo." });

        usuario.Activo        = false;
        usuario.ActualizadoEn = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(new { mensaje = "Admin desactivado." });
    }

    // ── PUT api/usuarios/5/activar ─────────────────────────
    // Reactivar un admin
    [HttpPut("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null) return NotFound();

        usuario.Activo        = true;
        usuario.ActualizadoEn = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(new { mensaje = "Admin activado." });
    }
}

// ── DTO local ──────────────────────────────────────────────
public record CrearAdminRequest(
    string Nombre,
    string Apellido,
    string Email,
    string Password
);

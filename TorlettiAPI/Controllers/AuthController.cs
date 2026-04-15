using CatalogoAPI.Data;
using CatalogoAPI.DTOs;
using CatalogoAPI.Models;
using CatalogoAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CatalogoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, JwtService jwt) : ControllerBase
{
    private readonly PasswordHasher<string> _hasher = new();

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        // 1. Buscar en usuarios del sistema
        var usuario = await db.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Email == req.Email && u.Activo);

        if (usuario is not null)
        {
            var resultado = _hasher.VerifyHashedPassword(req.Email, usuario.PasswordHash, req.Password);
            if (resultado == PasswordVerificationResult.Success)
            {
                var token = jwt.GenerarToken(usuario.IdUsuario, usuario.Email, usuario.Rol.Nombre);
                return Ok(new LoginResponse(token, usuario.Rol.Nombre, $"{usuario.Nombre} {usuario.Apellido}"));
            }
        }

        // 2. Buscar en clientes mayoristas
        var cliente = await db.Clientes
            .FirstOrDefaultAsync(c => c.Email == req.Email && c.Activo);

        if (cliente is not null)
        {
            var resultado = _hasher.VerifyHashedPassword(req.Email, cliente.PasswordHash, req.Password);
            if (resultado == PasswordVerificationResult.Success)
            {
                if (!cliente.Aprobado)
                    return Unauthorized(new { mensaje = "Tu cuenta está pendiente de aprobación. Te avisaremos por email cuando esté lista." });

                var token = jwt.GenerarToken(cliente.IdCliente, cliente.Email, "cliente");
                return Ok(new LoginResponse(token, "cliente", cliente.RazonSocial));
            }
        }

        return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroClienteRequest req)
    {
        if (await db.Clientes.AnyAsync(c => c.Email == req.Email) ||
            await db.Usuarios.AnyAsync(u => u.Email == req.Email))
            return Conflict(new { mensaje = "Ya existe una cuenta con ese email." });

        // Validar CUIT solo si lo mandaron
        if (!string.IsNullOrEmpty(req.Cuit) &&
            await db.Clientes.AnyAsync(c => c.Cuit == req.Cuit))
            return Conflict(new { mensaje = "Ya existe una cuenta con ese CUIT." });

        var cliente = new Cliente
        {
            RazonSocial = req.RazonSocial,   // puede ser null
            Cuit = req.Cuit,           // puede ser null
            Email = req.Email,
            PasswordHash = _hasher.HashPassword(req.Email, req.Password),
            Aprobado = false
        };

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync();

        return Ok(new { mensaje = "Registro exitoso. Un administrador revisará tu solicitud y te avisaremos por email." });
    }
}
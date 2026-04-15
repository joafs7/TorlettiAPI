using CatalogoAPI.Data;
using CatalogoAPI.DTOs;
using CatalogoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CatalogoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "superadmin,admin")]
public class ClientesController(AppDbContext db, EmailService email) : ControllerBase
{
    // ── GET api/clientes/pendientes ────────────────────────
    // Ver clientes que esperan aprobación
    [HttpGet("pendientes")]
    public async Task<IActionResult> Pendientes()
    {
        var lista = await db.Clientes
            .Where(c => !c.Aprobado && c.Activo)
            .Select(c => new ClienteResponse(c.IdCliente, c.RazonSocial, c.Cuit, c.Email, c.Aprobado, c.CreadoEn))
            .ToListAsync();

        return Ok(lista);
    }

    // ── GET api/clientes ───────────────────────────────────
    // Ver todos los clientes aprobados
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await db.Clientes
            .Where(c => c.Aprobado && c.Activo)
            .Select(c => new ClienteResponse(c.IdCliente, c.RazonSocial, c.Cuit, c.Email, c.Aprobado, c.CreadoEn))
            .ToListAsync();

        return Ok(lista);
    }

    // ── POST api/clientes/5/aprobar ────────────────────────
    // Aprobar un cliente y enviarle el mail de bienvenida
    [HttpPost("{id}/aprobar")]
    public async Task<IActionResult> Aprobar(int id)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null)          return NotFound();
        if (cliente.Aprobado)         return BadRequest(new { mensaje = "El cliente ya estaba aprobado." });

        var idAdmin = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        cliente.Aprobado    = true;
        cliente.AprobadoPor = idAdmin;
        cliente.AprobadoEn  = DateTime.UtcNow;

        await db.SaveChangesAsync();

        // Enviar mail de bienvenida
        await email.EnviarBienvenidaAsync(cliente.Email, cliente.RazonSocial);

        return Ok(new { mensaje = $"Cliente {cliente.RazonSocial} aprobado. Se le envió un mail de bienvenida." });
    }

    // ── DELETE api/clientes/5 ──────────────────────────────
    // Baja lógica
    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var cliente = await db.Clientes.FindAsync(id);
        if (cliente is null) return NotFound();

        cliente.Activo = false;
        await db.SaveChangesAsync();

        return Ok(new { mensaje = "Cliente desactivado." });
    }
}

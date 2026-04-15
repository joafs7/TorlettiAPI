using CatalogoAPI.Data;
using CatalogoAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CatalogoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogosController(AppDbContext db, IWebHostEnvironment env) : ControllerBase
{
    // ── GET api/catalogos ──────────────────────────────────
    // Accesible para cualquier usuario autenticado (clientes incluidos)
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Listar()
    {
        var lista = await db.Catalogos
            .Where(c => c.Activo)
            .Select(c => new CatalogoResponse(
                c.IdCatalogo, c.Nombre, c.Descripcion, c.RutaUrl, c.SubidoEn))
            .ToListAsync();

        return Ok(lista);
    }

    // ── POST api/catalogos/5/subir-pdf ─────────────────────
    // Solo superadmin puede subir o reemplazar el PDF
    [HttpPost("{id}/subir-pdf")]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> SubirPdf(int id, IFormFile archivo)
    {
        if (archivo is null || archivo.Length == 0)
            return BadRequest(new { mensaje = "No se recibió ningún archivo." });

        if (!archivo.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { mensaje = "Solo se permiten archivos PDF." });

        var catalogo = await db.Catalogos.FindAsync(id);
        if (catalogo is null) return NotFound();

        // Asegurar ruta wwwroot
        var webRoot = !string.IsNullOrEmpty(env.WebRootPath)
            ? env.WebRootPath
            : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var carpeta = Path.Combine(webRoot, "pdfs");
        Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"catalogo_{id}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var rutaCompleta  = Path.Combine(carpeta, nombreArchivo);

        // Eliminar PDF anterior si existe
        if (!string.IsNullOrEmpty(catalogo.NombreArchivo))
        {
            var anterior = Path.Combine(carpeta, catalogo.NombreArchivo);
            if (System.IO.File.Exists(anterior)) System.IO.File.Delete(anterior);
        }

        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            await archivo.CopyToAsync(stream);

        catalogo.NombreArchivo = nombreArchivo;
        catalogo.RutaUrl       = $"/pdfs/{nombreArchivo}";
        catalogo.TamanioKb     = (int)(archivo.Length / 1024);
        catalogo.SubidoEn      = DateTime.UtcNow;

        // Leer claim de forma segura
        var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claimId, out var idUsuario))
            return Forbid(); // o BadRequest según prefieras

        catalogo.SubidoPor = idUsuario;

        await db.SaveChangesAsync();

        return Ok(new { mensaje = "PDF subido correctamente.", ruta = catalogo.RutaUrl });
    }
}

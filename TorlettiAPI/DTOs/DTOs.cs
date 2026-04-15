namespace CatalogoAPI.DTOs;

// ── AUTH ───────────────────────────────────────────────────

// Login unificado para todos (usuarios del sistema y clientes)
public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, string Rol, string NombreCompleto);

// Registro que hace el cliente desde la web
public record RegistroClienteRequest(
    string Email,
    string Password,
    string? RazonSocial,   // opcional
    string? Cuit           // opcional
);


// ── CLIENTES ───────────────────────────────────────────────

public record ClienteResponse(
    int IdCliente,
    string RazonSocial,
    string Cuit,
    string Email,
    bool Aprobado,
    DateTime CreadoEn
);


// ── CATÁLOGOS ──────────────────────────────────────────────

public record CatalogoResponse(
    int IdCatalogo,
    string Nombre,
    string? Descripcion,
    string? RutaUrl,
    DateTime? SubidoEn
);

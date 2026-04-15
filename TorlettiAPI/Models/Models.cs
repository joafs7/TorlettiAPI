using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogoAPI.Models;

public class Rol
{
    [Key] public int IdRol { get; set; }
    [Required, MaxLength(50)] public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public ICollection<Usuario> Usuarios { get; set; } = [];
}

public class Usuario
{
    [Key] public int IdUsuario { get; set; }
    public int IdRol { get; set; }
    [Required, MaxLength(100)] public string Nombre { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Apellido { get; set; } = string.Empty;
    [Required, MaxLength(150)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime? ActualizadoEn { get; set; }

    [ForeignKey(nameof(IdRol))] public Rol Rol { get; set; } = null!;
}

public class Cliente
{
    [Key] public int IdCliente { get; set; }
    [MaxLength(200)] public string? RazonSocial { get; set; }
    [MaxLength(20)] public string? Cuit { get; set; }
    [Required, MaxLength(150)] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(255)] public string PasswordHash { get; set; } = string.Empty;
    public bool Aprobado { get; set; } = false;    // false = pendiente de aprobación
    public int? AprobadoPor { get; set; }
    public DateTime? AprobadoEn { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(AprobadoPor))] public Usuario? Admin { get; set; }
}

public class Catalogo
{
    [Key] public int IdCatalogo { get; set; }
    [Required, MaxLength(100)] public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? NombreArchivo { get; set; }
    public string? RutaUrl { get; set; }
    public int? TamanioKb { get; set; }
    public DateTime? SubidoEn { get; set; }
    public int? SubidoPor { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(SubidoPor))] public Usuario? Superadmin { get; set; }
}

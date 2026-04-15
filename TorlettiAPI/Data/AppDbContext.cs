using CatalogoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoAPI.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Catalogo> Catalogos { get; set; }

    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Rol>(e => {
            e.ToTable("roles");
            e.Property(r => r.IdRol).HasColumnName("id_rol");
            e.Property(r => r.Nombre).HasColumnName("nombre");
            e.Property(r => r.Descripcion).HasColumnName("descripcion");
            e.Property(r => r.CreadoEn).HasColumnName("creado_en");
            e.HasIndex(r => r.Nombre).IsUnique();
        });

        m.Entity<Usuario>(e => {
            e.ToTable("usuarios");
            e.Property(u => u.IdUsuario).HasColumnName("id_usuario");
            e.Property(u => u.IdRol).HasColumnName("id_rol");
            e.Property(u => u.Nombre).HasColumnName("nombre");
            e.Property(u => u.Apellido).HasColumnName("apellido");
            e.Property(u => u.Email).HasColumnName("email");
            e.Property(u => u.PasswordHash).HasColumnName("password_hash");
            e.Property(u => u.Activo).HasColumnName("activo");
            e.Property(u => u.CreadoEn).HasColumnName("creado_en");
            e.Property(u => u.ActualizadoEn).HasColumnName("actualizado_en");
            e.HasIndex(u => u.Email).IsUnique();
        });

        m.Entity<Cliente>(e => {
            e.ToTable("clientes");
            e.Property(c => c.IdCliente).HasColumnName("id_cliente");
            e.Property(c => c.RazonSocial).HasColumnName("razon_social");
            e.Property(c => c.Cuit).HasColumnName("cuit");
            e.Property(c => c.Email).HasColumnName("email");
            e.Property(c => c.PasswordHash).HasColumnName("password_hash");
            e.Property(c => c.Aprobado).HasColumnName("aprobado");
            e.Property(c => c.AprobadoPor).HasColumnName("aprobado_por");
            e.Property(c => c.AprobadoEn).HasColumnName("aprobado_en");
            e.Property(c => c.Activo).HasColumnName("activo");
            e.Property(c => c.CreadoEn).HasColumnName("creado_en");
            e.HasIndex(c => c.Cuit).IsUnique();
            e.HasIndex(c => c.Email).IsUnique();
        });

        m.Entity<Catalogo>(e => {
            e.ToTable("catalogos");
            e.Property(c => c.IdCatalogo).HasColumnName("id_catalogo");
            e.Property(c => c.Nombre).HasColumnName("nombre");
            e.Property(c => c.Descripcion).HasColumnName("descripcion");
            e.Property(c => c.NombreArchivo).HasColumnName("nombre_archivo");
            e.Property(c => c.RutaUrl).HasColumnName("ruta_url");
            e.Property(c => c.TamanioKb).HasColumnName("tamanio_kb");
            e.Property(c => c.SubidoEn).HasColumnName("subido_en");
            e.Property(c => c.SubidoPor).HasColumnName("subido_por");
            e.Property(c => c.Activo).HasColumnName("activo");
            e.Property(c => c.CreadoEn).HasColumnName("creado_en");
            e.HasIndex(c => c.Nombre).IsUnique();
        });
    }
}

using Gym.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gym.Datos;

public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(
        DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Socio> Socios => Set<Socio>();

    // Configurar las restricciones de unicidad en el modelo
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Validar que el NombreUsuario del Administrador sea único
        modelBuilder.Entity<Administrador>()
            .HasIndex(administrador => administrador.NombreUsuario)
            .IsUnique();
        
        // Validar que el Email del Socio sea único
        modelBuilder.Entity<Socio>()
            .HasIndex(socio => socio.Email)
            .IsUnique();
    }
}
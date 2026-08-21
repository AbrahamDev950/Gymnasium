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

    public DbSet<Administrador> Administradores =>
        Set<Administrador>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Administrador>()
            .HasIndex(administrador => administrador.NombreUsuario)
            .IsUnique();
    }
}
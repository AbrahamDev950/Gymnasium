using Gym.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
    public DbSet<Plan> Planes => Set<Plan>();
    public DbSet<Membresia> Membresias => Set<Membresia>();

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
        
        // Validar que el Nombre del Plan sea único
        modelBuilder.Entity<Plan>()
            .HasIndex(plan => plan.Nombre)
            .IsUnique();
        modelBuilder.Entity<Plan>()
            .Property(plan => plan.Precio)
            .HasColumnType("decimal(10,2)"); // Precios validos como $100.00 o $100.99

        // Evitar que se elimine un socio si tiene membresías asociadas
        modelBuilder.Entity<Membresia>()
            .HasOne(m => m.Socio)
            .WithMany()
            .HasForeignKey(m => m.SocioId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        //Evitar que se elimine un plan si tiene membresías asociadas
        modelBuilder.Entity<Membresia>()
            .HasOne(m => m.Plan)
            .WithMany()
            .HasForeignKey(m => m.PlanId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Precision para el precio aplicado en la membresía
        modelBuilder.Entity<Membresia>()
            .Property(m => m.PrecioAplicado)
            .HasColumnType("decimal(10,2)");
    }
}
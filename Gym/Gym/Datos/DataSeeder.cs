using Gym.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gym.Datos;

public static class DataSeeder
{
    public static async Task InicializarAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDBContext>();

        var existeAdministrador =
            await context.Administradores.AnyAsync();

        if (existeAdministrador)
        {
            return;
        }

        var nombreUsuario =
            configuration["InitialAdmin:Username"];

        var password =
            configuration["InitialAdmin:Password"];

        if (string.IsNullOrWhiteSpace(nombreUsuario) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se configuraron las credenciales del administrador inicial."
            );
        }

        var administrador = new Administrador
        {
            NombreUsuario = nombreUsuario,
            PasswordHash = string.Empty
        };

        var passwordHasher =
            new PasswordHasher<Administrador>();

        administrador.PasswordHash =
            passwordHasher.HashPassword(
                administrador,
                password
            );

        context.Administradores.Add(administrador);

        await context.SaveChangesAsync();
    }
}
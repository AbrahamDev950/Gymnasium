using Gym.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Gym.Datos;
/// <summary>
/// Clase que se encarga de inicializar los datos en la base de datos.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Inicializa los datos en la base de datos, creando un administrador inicial si no existe.
    /// Los datos del administrador inicial se obtienen de la configuración de la aplicación.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="InvalidOperationException"></exception>
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
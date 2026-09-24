using Gym.Datos;
using Gym.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace Gym.Servicios;

public class LoginService : ILoginService
{
    private readonly ApplicationDBContext context;
    private readonly TokenService tokenService;
    private readonly IConfiguration configuration;

    public LoginService(ApplicationDBContext context, TokenService tokenService, IConfiguration configuration)
    {
        this.context = context;
        this.tokenService = tokenService;
        this.configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var administrador = await context.Administradores
            .SingleOrDefaultAsync(administrador =>
                administrador.NombreUsuario == request.NombreUsuario);

        if (administrador is null)
        {
            return null;
        }

        var passwordHasher = new PasswordHasher<Entidades.Administrador>();

        var resultado = passwordHasher.VerifyHashedPassword(
            administrador,
            administrador.PasswordHash,
            request.Password
        );

        if (resultado == PasswordVerificationResult.Failed)
        {
            return null;
        }

        // Si los datos son correctos generamos el token JWT
        var token = tokenService.GenerarToken(administrador);

        return new LoginResponse
        {
            Token = token,
            ExpirationMinutes = configuration.GetValue<int>("Jwt:ExpirationMinutes"),
            Id = administrador.Id,
            NombreUsuario = administrador.NombreUsuario
        };
    }
}
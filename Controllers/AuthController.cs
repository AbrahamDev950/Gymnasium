using Gym.Datos;
using Gym.DTOs;
using Gym.Entidades;
using Gym.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDBContext context;
    private readonly TokenService tokenService;

    public AuthController(ApplicationDBContext context, TokenService tokenService)
    {
        this.context = context;
        this.tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var administrador = await context.Administradores
            .SingleOrDefaultAsync(administrador =>
                administrador.NombreUsuario == request.Email);

        if (administrador is null)
        {
            return Unauthorized(new
            {
                mensaje = "Datos incorrectos."
            });
        }

        var passwordHasher = new PasswordHasher<Entidades.Administrador>();

        var resultado = passwordHasher.VerifyHashedPassword(
            administrador,
            administrador.PasswordHash,
            request.Password
        );

        if (resultado == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new
            {
                mensaje = "Datos incorrectos."
            });
        }
        
        var token = tokenService.GenerarToken(administrador);
        return Ok(new
        {
            token,
            expirationMinutes = 60,
            administrador = new
            {
                mensaje = "Credenciales correctas y token generado.", 
                administrador.Id,
                administrador.NombreUsuario
            }
        });
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet("ruta-protegida")]
    public async Task<IActionResult> RutaProtegida()
    {
        return Ok(new
        {
            mensaje = "Acceso autorizado al panel de administración.",
            usuario = User.Identity?.Name
        });
    }
}
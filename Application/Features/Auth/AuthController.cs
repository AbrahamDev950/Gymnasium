using Gym.Datos;
using Gym.DTOs;
using Gym.Entidades;
using Gym.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers;
/// <summary>
/// Controlador para manejar la autenticación de usuarios.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Servicio para manejar el login y la autenticación.
    /// </summary>
    private readonly ILoginService _loginService;

    public AuthController(ILoginService loginService)
    {
        this._loginService = loginService;
    }

    /// <summary>
    /// Inicia sesión en la aplicación y obtiene un token JWT.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointDescription("Inicia sesión en la aplicación y obtiene un token JWT.")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var respuesta = await _loginService.LoginAsync(request);

        if (respuesta is null)
        {
            return Unauthorized(new { message = "Nombre de usuario o contraseña incorrectos." });
        }

        return Ok(respuesta);
    }
}
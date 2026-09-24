using Gym.Application.Features.Socio.Services;
using Gym.Datos;
using Gym.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Features.Socio;

[ApiController]
[Route("api/socios")]
[Authorize(Roles = "Administrador")]
public class SociosController : ControllerBase
{
    private readonly ISocioRegistro _socioRegistro;

    public SociosController(ISocioRegistro socioRegistro)
    {
        _socioRegistro = socioRegistro;
    }

    [HttpPost]
    public async Task<ActionResult> Post(CrearSocioRequest request)
    {
        var socio = await _socioRegistro.RegistrarSocio(request);
        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = socio.Id },
            socio);
    }

    [HttpGet("total-registrados")]
    public async Task<ActionResult<IEnumerable<SocioResponse>>> ObtenerSociosRegistrados()
    {
        var respuesta = await _socioRegistro.ObtenerSociosRegistrados();

        return Ok(respuesta);
    }
    
    [HttpGet]
    public async Task<ActionResult<List<SocioResponse>>> ObtenerTodos([FromQuery] bool? activos)
    {
        var respuesta = await _socioRegistro.ObtenerTodos(activos);
        return Ok(respuesta);
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<SocioResponse>>> BuscarSocios([FromQuery] string? termino)
    {
        var respuesta = await _socioRegistro.BuscarSocios(termino ?? string.Empty);
        return Ok(respuesta);
    }


    [HttpGet("{id:int}")]
    public async Task<ActionResult<IEnumerable<SocioResponse>>> ObtenerPorId(int id)
    {
        var respuesta = await _socioRegistro.ObtenerPorId(id);
        if (respuesta == null || !respuesta.Any())
        {
            return NotFound(new { mensaje = "Socio no encontrado." });
        }
        return Ok(respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SocioResponse>> Put(int id, ActualizarSocioRequest peticionActualizar)
    {
        var respuesta = await _socioRegistro.ActualizarSocio(id, peticionActualizar);
        if (respuesta == null)
        {
            throw new InvalidOperationException("No se pudo actualizar el socio. Verifique que el ID sea correcto y que el correo electrónico no esté duplicado.");
        }
        return Ok(respuesta);
    }

    [HttpPatch("{id:int}/desactivar")]
    public async Task<ActionResult> DesactivarSocio(int id)
    {
        var respuesta = await _socioRegistro.DesactivarSocio(id);
        if (respuesta == null)
        {
            return NotFound(new { mensaje = "Socio no encontrado." });
        }

        return NoContent();
    }
    
    [HttpPatch("{id:int}/activar")]
    public async Task<ActionResult> ActivarSocio(int id)
    {
        var respuesta = await _socioRegistro.ActivarSocio(id);
        if (respuesta == null)
        {
            return NotFound(new { mensaje = "Socio no encontrado." });
        }

        return NoContent();
    }
    

}
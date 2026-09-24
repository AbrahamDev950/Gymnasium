using Gym.Application.Features.Asistencia.Services;
using Gym.Datos;
using Gym.DTOs;
using Gym.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers;

[ApiController]
[Route("api/asistencias")]
[Authorize(Roles = "Administrador")]
public class AsistenciasController : ControllerBase
{
    private readonly IAsistenciaService _asistenciaService;

    public AsistenciasController( IAsistenciaService asistenciaService)
    {
        _asistenciaService = asistenciaService;
    }

    // POST /api/asistencias
    [HttpPost]
    public async Task<ActionResult<AsistenciaResponse?>> RegistrarAsistencia([FromBody] int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var respuesta = await _asistenciaService.RegistrarAsistencia(id);

        if (respuesta == null)
        {
            return BadRequest(new { mensaje = "No se pudo registrar la asistencia." });
        }
        return CreatedAtAction(nameof(RegistrarAsistencia), new { id = respuesta.Id }, respuesta);
    }

    // GET /api/asistencias/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<AsistenciaResponse?>> ObtenerAsistenciaId(AsistenciaRequest request)
    {
        var respuesta = await _asistenciaService.ObtenerAsistenciaId(request);

        if (respuesta == null)
            return NotFound(new { mensaje = "Asistencia no encontrada." });

        return Ok(respuesta);
    }

    // GET /api/asistencias/dia/hoy
    [HttpGet("dia/hoy")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AsistenciaResponse>>> ObtenerAsistenciasDelDia()
    {
        var respuesta = await _asistenciaService.ObtenerAsistenciasDelDia();
        
        if(respuesta == null || !respuesta.Any())
            return NotFound(new { mensaje = "No se encontraron asistencias para el día de hoy." });
        
        return Ok(respuesta);
    }

    // GET /api/asistencias/socio/{socioId}
    [HttpGet("socio/{socioId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AsistenciaResponse>>> ObtenerHistorialAsistenciasDeSocio(int socioId)
    {
      var respuestas = await _asistenciaService.ObtenerHistorialAsistenciasDeSocio(new AsistenciaRequest
        {
            SocioId = socioId
        });

        return Ok(respuestas);
    }
}
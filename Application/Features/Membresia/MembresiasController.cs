using Gym.Application.Features.Membresia.Services;
using Gym.Controllers.Services;
using Gym.Datos;
using Gym.DTOs;
using Gym.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers;

[ApiController]
[Route("api/membresias")]
[Authorize(Roles = "Administrador")]
public class MembresíasController : ControllerBase
{
    private readonly IMembresiaService _membresiaService;

    public MembresíasController(ApplicationDBContext context, IMembresiaService membresiaService)
    {
        _membresiaService = membresiaService;
    }

    // POST /api/membresias
    [HttpPost]
    public async Task<ActionResult<MembresiaResponse>> CrearMembresia([FromBody] CrearMembresiaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var respuesta = await _membresiaService.CrearMembresia(request);
            return CreatedAtAction(nameof(ObtenerMembresíaPorId), new { id = respuesta.Id }, respuesta);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor" });
        }
    }

    // GET /api/membresias/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MembresiaResponse>> ObtenerMembresíaPorId(int id)
    {
        var respuesta = await _membresiaService.ObtenerMembresíaPorId(id);
        return Ok(respuesta);
    }

    // GET /api/socios/{socioId}/membresia-vigente
    [HttpGet("/api/socios/{socioId}/membresia-vigente")]
    [AllowAnonymous]
    public async Task<ActionResult<MembresiaResponse>> ObtenerMembresiaVigenteDeSocio(int socioId)
    {
        var respuesta = await _membresiaService.ObtenerMembresíaVigenteDeSocio(socioId);
        return Ok(respuesta);
    }

    // GET /api/socios/{socioId}/membresias
    [HttpGet("/api/socios/{socioId}/membresias")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MembresiaResponse>>> ObtenerHistorialMembresiasDeSocio(int socioId)
    {
        var respuesta = await _membresiaService.ObtenerHistorialMembresiasDeSocio(socioId);
        return Ok(respuesta);
    }

    // POST /api/membresias/{id}/renovar
    [HttpPost("{id}/renovar")]
    public async Task<ActionResult<MembresiaResponse>> RenovarMembresia(int id,
        [FromBody] RenovarMembresiaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var respuesta = await _membresiaService.RenovarMembresia(id, request);
        return Ok(respuesta);
    }

    // GET /api/membresias?proximas-a-vencer=true
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MembresiaResponse>>> ObtenerMembresias(
        [FromQuery] bool proximasAVencer = false)
    {
        var respuesta = await _membresiaService.ObtenerMembresias(proximasAVencer);
        return Ok(respuesta);
    }

    // Método auxiliar para calcular el estado dinámicamente
    // private static string CalcularEstado(DateTime fechaVencimiento)
    // {
    //     return DateTime.UtcNow <= fechaVencimiento ? "Activa" : "Vencida";
    // }

    // Método auxiliar para mapear entidad a DTO
    // private async Task<MembresiaResponse> MapearAResponse(Membresia membresia)
    // {
    //     // Asegurarse de que las propiedades de navegación estén cargadas
    //     if (membresia.Socio == null)
    //         membresia.Socio = await _context.Socios.FindAsync(membresia.SocioId);
    //     
    //     if (membresia.Plan == null)
    //         membresia.Plan = await _context.Planes.FindAsync(membresia.PlanId);
    //
    //     return new MembresiaResponse
    //     {
    //         Id = membresia.Id,
    //         SocioId = membresia.SocioId,
    //         NombreSocio = $"{membresia.Socio?.Nombre} {membresia.Socio?.Apellido}",
    //         PlanId = membresia.PlanId,
    //         NombrePlan = membresia.Plan?.Nombre ?? "Plan desconocido",
    //         FechaInicio = membresia.FechaInicio,
    //         FechaVencimiento = membresia.FechaVencimiento,
    //         PrecioAplicado = membresia.PrecioAplicado,
    //         Estado = CalcularEstado(membresia.FechaVencimiento),
    //         FechaCreacion = membresia.FechaCreacion,
    //         DiasRestantes = (int)(membresia.FechaVencimiento - DateTime.UtcNow).TotalDays
    //     };
}
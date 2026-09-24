using Gym.Application.Features.Planes.Services;
using Gym.Controllers.Services;
using Gym.Datos;
using Gym.DTOs;
using Gym.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers;

[ApiController]
[Route("api/planes")]
[Authorize(Roles = "Administrador")]
public class PlanesController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    private readonly IPlanService _planService;

    public PlanesController(IPlanService planService)
    {
        _planService = planService;
    }

    // POST /api/planes
    [HttpPost]
    public async Task<ActionResult<PlanResponse>> CrearPlan(IPlanService planService, [FromBody] CrearPlanRequest request)
    {
        // Las validaciones ocurren aqui de acuerdo a los DataAnnotations en el DTO CrearPlanRequest
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var planResponse = await _planService.CrearPlan(request);
            return CreatedAtAction(nameof(ObtenerPlanPorId), new { id = planResponse.Id }, planResponse);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
    

    // GET /api/planes
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PlanResponse>>> ObtenerTodosLosPlanes()
    {
        var respuesta = await _planService.ObtenerTodosLosPlanes();
        return Ok(respuesta);
    }

    // GET /api/planes/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PlanResponse>> ObtenerPlanPorId(int id)
    {
        var respuesta = await _planService.ObtenerPlanPorId(id);
        return Ok(respuesta);
    }

    // PUT /api/planes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarPlan(int id, [FromBody] ActualizarPlanRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var respuesta = await _planService.ActualizarPlan(id, request);
        return Ok(respuesta);
    }

    // PATCH /api/planes/{id}/desactivar
    [HttpPatch("{id}/desactivar")]
    public async Task<IActionResult> DesactivarPlan(int id)
    {
        var respuesta = await _planService.DesactivarPlan(id);
        return Ok(respuesta);
    }

    // PATCH /api/planes/{id}/reactivar
    [HttpPatch("{id}/reactivar")]
    public async Task<IActionResult> ReactivarPlan(int id)
    {
        var respuesta = await _planService.ReactivarPlan(id);
        return Ok(respuesta);
    }
}
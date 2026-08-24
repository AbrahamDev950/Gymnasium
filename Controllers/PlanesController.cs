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

    public PlanesController(ApplicationDBContext context)
    {
        _context = context;
    }

    // POST /api/planes
    [HttpPost]
    public async Task<ActionResult<PlanResponse>> CrearPlan([FromBody] CrearPlanRequest request)
    {
        // Las validaciones ocurren aqui de acuerdo a los DataAnnotations en el DTO CrearPlanRequest
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Sanitizar el nombre del plan para evitar problemas de espacios y mayúsculas/minúsculas
        var planExistente = await _context.Planes
            .AsNoTracking()
            .AnyAsync(p => p.Nombre.ToLower() == request.Nombre.Trim().ToLower());
        
        if (planExistente)
            return Conflict(new { mensaje = "Ya existe un plan con este nombre." });

        var plan = new Plan
        {
            // El cliente no puede enviar el Id ni la fecha de creación,
            // estos se generan automáticamente
            Nombre = request.Nombre.Trim(),
            Duracion = request.Duracion,
            Precio = request.Precio,
            // El plan se crea como activo por defecto
            Activo = true
        };
        
        // Agregar el plan a la base de datos
        _context.Planes.Add(plan);
        await _context.SaveChangesAsync();
        
        // Mapear la entidad a un DTO de respuesta para devolver al cliente y no exponer la entidad directamente
        return CreatedAtAction(nameof(ObtenerPlanPorId), new { id = plan.Id }, MapearAResponse(plan));
    }

    // GET /api/planes
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PlanResponse>>> ObtenerTodosLosPlanes()
    {
        var planes = await _context.Planes
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        return Ok(planes.Select(MapearAResponse));
    }

    // GET /api/planes/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<PlanResponse>> ObtenerPlanPorId(int id)
    {
        var plan = await _context.Planes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null)
            return NotFound(new { mensaje = "Plan no encontrado." });

        return Ok(MapearAResponse(plan));
    }

    // PUT /api/planes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarPlan(int id, [FromBody] ActualizarPlanRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var plan = await _context.Planes.FindAsync(id);

        if (plan == null)
            return NotFound(new { mensaje = "Plan no encontrado." });

        // Si cambiar nombre, verificar unicidad
        if (!string.IsNullOrWhiteSpace(request.Nombre) && request.Nombre.Trim().ToLower() != plan.Nombre.ToLower())
        {
            var nombreEnUso = await _context.Planes
                .AsNoTracking()
                .AnyAsync(p => p.Nombre.ToLower() == request.Nombre.Trim().ToLower() && p.Id != id);

            if (nombreEnUso)
                return Conflict(new { mensaje = "Ya existe un plan con este nombre." });

            plan.Nombre = request.Nombre.Trim();
        }

        if (request.Duracion.HasValue)
            plan.Duracion = request.Duracion.Value;

        if (request.Precio.HasValue)
            plan.Precio = request.Precio.Value;

        _context.Planes.Update(plan);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Plan actualizado correctamente.", plan = MapearAResponse(plan) });
    }

    // PATCH /api/planes/{id}/desactivar
    [HttpPatch("{id}/desactivar")]
    public async Task<IActionResult> DesactivarPlan(int id)
    {
        var plan = await _context.Planes.FindAsync(id);

        if (plan == null)
            return NotFound(new { mensaje = "Plan no encontrado." });

        if (!plan.Activo)
            return BadRequest(new { mensaje = "El plan ya está desactivado." });

        plan.Activo = false;
        _context.Planes.Update(plan);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Plan desactivado correctamente." });
    }

    // PATCH /api/planes/{id}/reactivar
    [HttpPatch("{id}/reactivar")]
    public async Task<IActionResult> ReactivarPlan(int id)
    {
        var plan = await _context.Planes.FindAsync(id);

        if (plan == null)
            return NotFound(new { mensaje = "Plan no encontrado." });

        if (plan.Activo)
            return BadRequest(new { mensaje = "El plan ya está activo." });

        plan.Activo = true;
        _context.Planes.Update(plan);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Plan reactivado correctamente." });
    }

    // Método auxiliar para mapear entidad a DTO
    private static PlanResponse MapearAResponse(Plan plan)
    {
        return new PlanResponse
        {
            Id = plan.Id,
            Nombre = plan.Nombre,
            Duracion = plan.Duracion,
            Precio = plan.Precio,
            Activo = plan.Activo,
            FechaCreacion = plan.FechaCreacion
        };
    }
}
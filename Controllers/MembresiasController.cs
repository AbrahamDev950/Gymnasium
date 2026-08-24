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
    private readonly ApplicationDBContext _context;

    public MembresíasController(ApplicationDBContext context)
    {
        _context = context;
    }

    // POST /api/membresias
    [HttpPost]
    public async Task<ActionResult<MembresiaResponse>> CrearMembresia([FromBody] CrearMembresiaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar que el socio existe y está activo
        var socio = await _context.Socios
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SocioId);

        if (socio == null)
            return NotFound(new { mensaje = "Socio no encontrado." });

        if (!socio.Activo)
            return BadRequest(new { mensaje = "No se puede asignar membresía a un socio inactivo." });

        // Verificar que el plan existe y está activo
        var plan = await _context.Planes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PlanId);

        if (plan == null)
            return NotFound(new { mensaje = "Plan no encontrado." });

        if (!plan.Activo)
            return BadRequest(new { mensaje = "No se puede asignar un plan inactivo." });

        // Verificar si el socio ya tiene una membresía activa
        var membresiaActiva = await _context.Membresias
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.SocioId == request.SocioId && m.Estado == "Activa");

        if (membresiaActiva != null)
            return Conflict(new { mensaje = "El socio ya tiene una membresía activa. Renuévala en su lugar." });

        // Crear la membresía
        var fechaInicio = DateTime.UtcNow;
        var membresia = new Membresia
        {
            SocioId = request.SocioId,
            PlanId = request.PlanId,
            FechaInicio = fechaInicio,
            FechaVencimiento = fechaInicio.AddDays(plan.Duracion),
            PrecioAplicado = plan.Precio,
            Estado = "Activa"
        };

        _context.Membresias.Add(membresia);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerMembresíaPorId), new { id = membresia.Id }, 
            await MapearAResponse(membresia));
    }

    // GET /api/membresias/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MembresiaResponse>> ObtenerMembresíaPorId(int id)
    {
        var membresia = await _context.Membresias
            .Include(m => m.Socio)
            .Include(m => m.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membresia == null)
            return NotFound(new { mensaje = "Membresía no encontrada." });

        return Ok(await MapearAResponse(membresia));
    }

    // GET /api/socios/{socioId}/membresia-vigente
    [HttpGet("/api/socios/{socioId}/membresia-vigente")]
    [AllowAnonymous]
    public async Task<ActionResult<MembresiaResponse>> ObtenerMembresiaVigenteDeSocio(int socioId)
    {
        var membresia = await _context.Membresias
            .Include(m => m.Socio)
            .Include(m => m.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.SocioId == socioId && m.Estado == "Activa");

        if (membresia == null)
            return NotFound(new { mensaje = "El socio no tiene membresía vigente." });

        return Ok(await MapearAResponse(membresia));
    }

    // GET /api/socios/{socioId}/membresias
    [HttpGet("/api/socios/{socioId}/membresias")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MembresiaResponse>>> ObtenerHistorialMembresiasDeSocio(int socioId)
    {
        var socioExiste = await _context.Socios
            .AsNoTracking()
            .AnyAsync(s => s.Id == socioId);

        if (!socioExiste)
            return NotFound(new { mensaje = "Socio no encontrado." });

        var membresias = await _context.Membresias
            .Include(m => m.Socio)
            .Include(m => m.Plan)
            .AsNoTracking()
            .Where(m => m.SocioId == socioId)
            .OrderByDescending(m => m.FechaCreacion)
            .ToListAsync();

        var respuestas = new List<MembresiaResponse>();
        foreach (var membresia in membresias)
        {
            respuestas.Add(await MapearAResponse(membresia));
        }

        return Ok(respuestas);
    }

    // POST /api/membresias/{id}/renovar
    [HttpPost("{id}/renovar")]
    public async Task<ActionResult<MembresiaResponse>> RenovarMembresia(int id, [FromBody] RenovarMembresiaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Obtener membresía anterior
        var membresiaAnterior = await _context.Membresias
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membresiaAnterior == null)
            return NotFound(new { mensaje = "Membresía no encontrada." });

        // Verificar que el plan existe y está activo
        var plan = await _context.Planes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PlanId);

        if (plan == null)
            return NotFound(new { mensaje = "Plan no encontrado." });

        if (!plan.Activo)
            return BadRequest(new { mensaje = "No se puede renovar con un plan inactivo." });
        
        // Determinar la fecha de inicio: si aun esta vigente, la nueva membresía inicia al final de la anterior; si ya venció, inicia hoy.

        DateTime fechaInicio;
        if (membresiaAnterior.FechaVencimiento > DateTime.UtcNow)
        {
            fechaInicio = membresiaAnterior.FechaVencimiento;
        }
        else
        {
            fechaInicio = DateTime.UtcNow;
        }
        // Crear nueva membresía (la anterior se mantiene en historial)
        var membresiaNueva = new Membresia
        {
            SocioId = membresiaAnterior.SocioId,
            PlanId = request.PlanId,
            FechaInicio = fechaInicio,
            FechaVencimiento = fechaInicio.AddDays(plan.Duracion),
            PrecioAplicado = plan.Precio,
            Estado = "Activa"
        };

        _context.Membresias.Add(membresiaNueva);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerMembresíaPorId), new { id = membresiaNueva.Id }, 
            await MapearAResponse(membresiaNueva));
    }

    // GET /api/membresias?proximas-a-vencer=true
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MembresiaResponse>>> ObtenerMembresias([FromQuery] bool proximasAVencer = false)
    {
        IQueryable<Membresia> query = _context.Membresias
            .Include(m => m.Socio)
            .Include(m => m.Plan)
            .AsNoTracking();

        if (proximasAVencer)
        {
            var hoy = DateTime.UtcNow;
            var proximosDias = hoy.AddDays(30);
            query = query.Where(m => m.Estado == "Activa" && 
                                     m.FechaVencimiento > hoy && 
                                     m.FechaVencimiento <= proximosDias);
        }

        var membresias = await query
            .OrderBy(m => m.FechaVencimiento)
            .ToListAsync();

        var respuestas = new List<MembresiaResponse>();
        foreach (var membresia in membresias)
        {
            respuestas.Add(await MapearAResponse(membresia));
        }

        return Ok(respuestas);
    }
    
    // Método auxiliar para calcular el estado dinámicamente
    private static string CalcularEstado(DateTime fechaVencimiento)
    {
        return DateTime.UtcNow <= fechaVencimiento ? "Activa" : "Vencida";
    }

    // Método auxiliar para mapear entidad a DTO
    private async Task<MembresiaResponse> MapearAResponse(Membresia membresia)
    {
        // Asegurarse de que las propiedades de navegación estén cargadas
        if (membresia.Socio == null)
            membresia.Socio = await _context.Socios.FindAsync(membresia.SocioId);
        
        if (membresia.Plan == null)
            membresia.Plan = await _context.Planes.FindAsync(membresia.PlanId);

        return new MembresiaResponse
        {
            Id = membresia.Id,
            SocioId = membresia.SocioId,
            NombreSocio = $"{membresia.Socio?.Nombre} {membresia.Socio?.Apellido}",
            PlanId = membresia.PlanId,
            NombrePlan = membresia.Plan?.Nombre ?? "Plan desconocido",
            FechaInicio = membresia.FechaInicio,
            FechaVencimiento = membresia.FechaVencimiento,
            PrecioAplicado = membresia.PrecioAplicado,
            Estado = CalcularEstado(membresia.FechaVencimiento),
            FechaCreacion = membresia.FechaCreacion,
            DiasRestantes = (int)(membresia.FechaVencimiento - DateTime.UtcNow).TotalDays
        };
    }
}
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
    private readonly ApplicationDBContext _context;

    public AsistenciasController(ApplicationDBContext context)
    {
        _context = context;
    }

    // POST /api/asistencias
    [HttpPost]
    public async Task<ActionResult<AsistenciaResponse>> RegistrarAsistencia([FromBody] CrearAsistenciaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar que el socio existe
        var socio = await _context.Socios
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SocioId);

        if (socio == null)
            return NotFound(new { mensaje = "Socio no encontrado." });

        // Verificar que el socio está activo
        if (!socio.Activo)
            return BadRequest(new { mensaje = "No se puede registrar entrada: el socio está inactivo." });

        // Verificar que el socio tiene membresía vigente
        var membresiaVigente = await _context.Membresias
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.SocioId == request.SocioId && 
                                      m.FechaVencimiento > DateTime.UtcNow);

        if (membresiaVigente == null)
            return BadRequest(new { mensaje = "El socio no tiene una membresía vigente." });

        // Registrar asistencia
        var asistencia = new Asistencia
        {
            SocioId = request.SocioId,
            FechaHoraEntrada = DateTime.UtcNow
        };

        _context.Asistencias.Add(asistencia);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerAsistenciaPorId), new { id = asistencia.Id }, 
            MapearAResponse(asistencia, socio.Nombre, socio.Apellido));
    }

    // GET /api/asistencias/{id}
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<AsistenciaResponse>> ObtenerAsistenciaPorId(int id)
    {
        var asistencia = await _context.Asistencias
            .Include(a => a.Socio)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asistencia == null)
            return NotFound(new { mensaje = "Asistencia no encontrada." });

        return Ok(MapearAResponse(asistencia, asistencia.Socio?.Nombre ?? "", asistencia.Socio?.Apellido ?? ""));
    }

    // GET /api/asistencias/dia/hoy
    [HttpGet("dia/hoy")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AsistenciaResponse>>> ObtenerAsistenciasDelDia()
    {
        var hoy = DateTime.UtcNow.Date;
        var manana = hoy.AddDays(1);

        var asistencias = await _context.Asistencias
            .Include(a => a.Socio)
            .AsNoTracking()
            .Where(a => a.FechaHoraEntrada >= hoy && a.FechaHoraEntrada < manana)
            .OrderByDescending(a => a.FechaHoraEntrada)
            .ToListAsync();

        var respuestas = asistencias.Select(a => 
            MapearAResponse(a, a.Socio?.Nombre ?? "", a.Socio?.Apellido ?? "")).ToList();

        return Ok(respuestas);
    }

    // GET /api/asistencias/socio/{socioId}
    [HttpGet("socio/{socioId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AsistenciaResponse>>> ObtenerHistorialAsistenciasDeSocio(int socioId)
    {
        // Verificar que el socio existe
        var socioExiste = await _context.Socios
            .AsNoTracking()
            .AnyAsync(s => s.Id == socioId);

        if (!socioExiste)
            return NotFound(new { mensaje = "Socio no encontrado." });

        var asistencias = await _context.Asistencias
            .Include(a => a.Socio)
            .AsNoTracking()
            .Where(a => a.SocioId == socioId)
            .OrderByDescending(a => a.FechaHoraEntrada)
            .Take(30)  // Últimas 30 entradas
            .ToListAsync();

        var respuestas = asistencias.Select(a => 
            MapearAResponse(a, a.Socio?.Nombre ?? "", a.Socio?.Apellido ?? "")).ToList();

        return Ok(respuestas);
    }

    // Método auxiliar para mapear entidad a DTO
    private static AsistenciaResponse MapearAResponse(Asistencia asistencia, string nombre, string apellido)
    {
        return new AsistenciaResponse
        {
            Id = asistencia.Id,
            SocioId = asistencia.SocioId,
            NombreSocio = $"{nombre} {apellido}",
            FechaHoraEntrada = asistencia.FechaHoraEntrada,
            FechaCreacion = asistencia.FechaCreacion
        };
    }
}
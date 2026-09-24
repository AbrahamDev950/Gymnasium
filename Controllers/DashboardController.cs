using Gym.Controllers.Services;
using Gym.Datos;
using Gym.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers;

[ApiController]
[Route("api/dashboard")]
[AllowAnonymous]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDBContext _context;

    public DashboardController(ApplicationDBContext context)
    {
        _context = context;
    }

    // GET /api/dashboard
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> ObtenerResumenDashboard(IAsistenciaService asistenciaService)
    {
        var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var finMes = inicioMes.AddMonths(1);
        

        // Membresías vigentes
        var ahora = DateTime.UtcNow;
        var membresíasVigentes = await _context.Membresias
            .AsNoTracking()
            .CountAsync(m => m.FechaVencimiento > ahora);

        // Membresías próximas a vencer (próximos 7 días)
        var proximosDías = ahora.AddDays(7);
        var membresíasProximasAVencer = await _context.Membresias
            .AsNoTracking()
            .CountAsync(m => m.FechaVencimiento > ahora && m.FechaVencimiento <= proximosDías);

        // Socios activos
        var sociosActivos = await _context.Socios
            .AsNoTracking()
            .CountAsync(s => s.Activo);

        // Socios inactivos
        var sociosInactivos = await _context.Socios
            .AsNoTracking()
            .CountAsync(s => !s.Activo);

        // Ingresos del mes (suma de PrecioAplicado de membresías creadas este mes)
        var ingresosDelMes = await _context.Membresias
            .AsNoTracking()
            .Where(m => m.FechaCreacion >= inicioMes && m.FechaCreacion < finMes)
            .SumAsync(m => m.PrecioAplicado);

        var respuesta = new DashboardResponse
        {
            AsistenciasHoy = asistenciaService.ObtenerAsistenciasDelDia().Result?.Count() ?? 0,
            MembresíasVigentes = membresíasVigentes,
            MembresíasProximasAVencer = membresíasProximasAVencer,
            SociosActivos = sociosActivos,
            SociosInactivos = sociosInactivos,
            IngresosDelMes = ingresosDelMes,
            FechaConsulta = DateTime.UtcNow
        };

        return Ok(respuesta);
    }
}
using Gym.Controllers.Services;
using Gym.Datos;
using Gym.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Features.Membresia.Services;

public class MembresiaService : IMembresiaService
{
    private readonly ApplicationDBContext _context;

    public MembresiaService(ApplicationDBContext context)
    {
        _context = context;
    }
    
    public async Task<MembresiaResponse> CrearMembresia(CrearMembresiaRequest request)
    {
        // Verificar que el socio existe y está activo
        var socio = await _context.Socios
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SocioId);

        if (socio == null)
        {
            throw new KeyNotFoundException("Socio no encontrado.");
        }

        if (!socio.Activo)
        {
            throw new InvalidOperationException("No se puede asignar una membresía a un socio inactivo.");
        }
        
        // Verificar que el plan existe y está activo
        var plan = await _context.Planes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PlanId);

        if (plan == null)
            throw new KeyNotFoundException("Plan no encontrado.");

        if (!plan.Activo)
            throw new InvalidOperationException("No se puede asignar un plan inactivo.");

        // Verificar si el socio ya tiene una membresía activa
        var membresiaActiva = await _context.Membresias
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.SocioId == request.SocioId && m.Estado == "Activa");

        if (membresiaActiva != null)
        {
            throw new InvalidOperationException("El socio ya tiene una membresía activa.");
        }
        
        // Una vez que pasa toda las validaciones, crear la membresía
        var fechaInicio = DateTime.UtcNow;
        var membresia = new Entidades.Membresia
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

        return new MembresiaResponse
        {
            Id = membresia.Id,
            SocioId = membresia.SocioId,
            PlanId = membresia.PlanId,
            FechaInicio = membresia.FechaInicio,
            FechaVencimiento = membresia.FechaVencimiento,
            PrecioAplicado = membresia.PrecioAplicado,
            Estado = membresia.Estado,
            NombreSocio = socio.Nombre,
            NombrePlan = plan.Nombre,
            DiasRestantes = (int)(membresia.FechaVencimiento - DateTime.UtcNow).TotalDays

        };
    }

    public async Task<MembresiaResponse> ObtenerMembresíaPorId(int id)
    {
        var membresia = await _context.Membresias
            .Include(m => m.Socio)
            .Include(m => m.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (membresia == null)
        {
            throw new KeyNotFoundException("Membresía no encontrada.");
        }
        return new MembresiaResponse
        {
            Id = membresia.Id,
            SocioId = membresia.SocioId,
            PlanId = membresia.PlanId,
            FechaInicio = membresia.FechaInicio,
            FechaVencimiento = membresia.FechaVencimiento,
            PrecioAplicado = membresia.PrecioAplicado,
            Estado = membresia.Estado,
            NombreSocio = membresia.Socio?.Nombre ?? "Desconocido",
            NombrePlan = membresia.Plan?.Nombre ?? "Desconocido",
            DiasRestantes = (int)(membresia.FechaVencimiento - DateTime.UtcNow).TotalDays

        };
    }

    public async Task<MembresiaResponse> ObtenerMembresíaVigenteDeSocio(int socioId)
    {
        var membresia = await _context.Membresias
            .Include(m => m.Socio)
            .Include(m => m.Plan)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.SocioId == socioId && m.Estado == "Activa");

        if (membresia == null)
        {
            throw new KeyNotFoundException("No se encontró una membresía activa para el socio especificado.");
        }
        return new MembresiaResponse
        {
            Id = membresia.Id,
            SocioId = membresia.SocioId,
            PlanId = membresia.PlanId,
            FechaInicio = membresia.FechaInicio,
            FechaVencimiento = membresia.FechaVencimiento,
            PrecioAplicado = membresia.PrecioAplicado,
            Estado = membresia.Estado,
            NombreSocio = membresia.Socio?.Nombre ?? "Desconocido",
            NombrePlan = membresia.Plan?.Nombre ?? "Desconocido",
            DiasRestantes = (int)(membresia.FechaVencimiento - DateTime.UtcNow).TotalDays

        };
    }

    public async Task<IEnumerable<MembresiaResponse>> ObtenerHistorialMembresiasDeSocio(int socioId)
    {
        var socioExiste = await _context.Socios
            .AsNoTracking()
            .AnyAsync(s => s.Id == socioId);

        if (!socioExiste)
        {
            throw new KeyNotFoundException("Socio no encontrado.");
        }
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
             respuestas.Add(new MembresiaResponse
            {
                Id = membresia.Id,
                SocioId = membresia.SocioId,
                PlanId = membresia.PlanId,
                FechaInicio = membresia.FechaInicio,
                FechaVencimiento = membresia.FechaVencimiento,
                PrecioAplicado = membresia.PrecioAplicado,
                Estado = membresia.Estado,
                NombreSocio = membresia.Socio?.Nombre ?? "Desconocido",
                NombrePlan = membresia.Plan?.Nombre ?? "Desconocido"
            });
        }
        return respuestas;
    }

    public async Task<MembresiaResponse> RenovarMembresia(int id, RenovarMembresiaRequest request)
    {
        var membresiaAnterior = await _context.Membresias
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (membresiaAnterior == null)
        {
            throw new KeyNotFoundException("Membresía no encontrada.");
        }
        // Verificar que el plan existe y está activo
        var plan = await _context.Planes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PlanId);

        if (plan == null)
            throw new KeyNotFoundException("Plan no encontrado.");

        if (!plan.Activo)
            throw new InvalidOperationException("No se puede renovar con un plan inactivo.");
        
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
        var membresiaNueva = new Entidades.Membresia
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
        return new MembresiaResponse
        {
            Id = membresiaNueva.Id,
            SocioId = membresiaNueva.SocioId,
            PlanId = membresiaNueva.PlanId,
            FechaInicio = membresiaNueva.FechaInicio,
            FechaVencimiento = membresiaNueva.FechaVencimiento,
            PrecioAplicado = membresiaNueva.PrecioAplicado,
            Estado = membresiaNueva.Estado,
            NombreSocio = membresiaNueva.Socio?.Nombre ?? "Desconocido",
            NombrePlan = membresiaNueva.Plan?.Nombre ?? "Desconocido",
            DiasRestantes = (int)(membresiaNueva.FechaVencimiento - DateTime.UtcNow).TotalDays
        };
    }

    public async Task<IEnumerable<MembresiaResponse>> ObtenerMembresias(bool proximasAVencer = false)
    {
        IQueryable<Entidades.Membresia> query = _context.Membresias
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
            respuestas.Add(new MembresiaResponse
            {
                Id = membresia.Id,
                SocioId = membresia.SocioId,
                PlanId = membresia.PlanId,
                FechaInicio = membresia.FechaInicio,
                FechaVencimiento = membresia.FechaVencimiento,
                PrecioAplicado = membresia.PrecioAplicado,
                Estado = membresia.Estado,
                NombreSocio = membresia.Socio?.Nombre ?? "Desconocido",
                NombrePlan = membresia.Plan?.Nombre ?? "Desconocido",
                DiasRestantes = (int)(membresia.FechaVencimiento - DateTime.UtcNow).TotalDays
            });
        }
        return respuestas;
    }
}
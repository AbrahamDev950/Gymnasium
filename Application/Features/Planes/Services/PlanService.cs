using Gym.Application.Features.Planes.Services;
using Gym.Datos;
using Gym.DTOs;
using Gym.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers.Services;

public class PlanService : IPlanService
{
    private readonly ApplicationDBContext _context;
    public PlanService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<PlanResponse> CrearPlan(CrearPlanRequest request)
    {
        var planExistente = await _context.Planes
            .AsNoTracking()
            .AnyAsync(p => p.Nombre.ToLower() == request.Nombre.Trim().ToLower());

        if (planExistente)
        {
            throw new InvalidOperationException("El plan ya existe");
        }

        var plan = new Plan
    {
            Nombre = request.Nombre,
            Duracion = request.Duracion,
            Precio = request.Precio,
            Activo = true
        };
        _context.Planes.Add(plan);
        await _context.SaveChangesAsync();

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
    
    public async Task<IEnumerable<PlanResponse>> ObtenerTodosLosPlanes()
    {
        var planes = await _context.Planes
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .ToListAsync();

        return planes.Select(p => new PlanResponse
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Duracion = p.Duracion,
            Precio = p.Precio,
            Activo = p.Activo,
            FechaCreacion = p.FechaCreacion
        });
    }
    
    public async Task<PlanResponse> ObtenerPlanPorId(int id)
    {
        var plan = await _context.Planes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan == null)
        {
            throw new KeyNotFoundException("Plan no encontrado");
        }

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
    
    public async Task<PlanResponse> ActualizarPlan(int id, ActualizarPlanRequest request)
    {
        var plan = await _context.Planes.FindAsync(id);

        if (plan == null)
        {
            throw new KeyNotFoundException("Plan no encontrado");
        }

        if (!string.IsNullOrWhiteSpace(request.Nombre))
        {
            plan.Nombre = request.Nombre;
        }

        if (request.Duracion.HasValue)
        {
            plan.Duracion = request.Duracion.Value;
        }

        if (request.Precio.HasValue)
        {
            plan.Precio = request.Precio.Value;
        }

        _context.Planes.Update(plan);
        await _context.SaveChangesAsync();

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
    
    public async Task<PlanResponse> DesactivarPlan(int id)
    {
        var plan = await _context.Planes.FindAsync(id);

        if (plan == null)
        {
            throw new KeyNotFoundException("Plan no encontrado");
        }

        if (!plan.Activo)
        {
            throw new InvalidOperationException("El plan ya está desactivado");
        }

        plan.Activo = false;
        _context.Planes.Update(plan);
        await _context.SaveChangesAsync();

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
    
    public async Task<PlanResponse> ReactivarPlan(int id)
    {
        var plan = await _context.Planes.FindAsync(id);

        if (plan == null)
        {
            throw new KeyNotFoundException("Plan no encontrado");
        }

        if (plan.Activo)
        {
            throw new InvalidOperationException("El plan ya está activo");
        }

        plan.Activo = true;
        _context.Planes.Update(plan);
        await _context.SaveChangesAsync();

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
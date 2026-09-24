using Gym.Datos;
using Gym.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Features.Asistencia.Services;

public class AsistenciaService : IAsistenciaService
{
    private readonly ApplicationDBContext _context;

    public AsistenciaService(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<AsistenciaResponse?> RegistrarAsistencia(int id)
    {
        var socio = await _context.Socios
            .AsNoTracking()
            .FirstOrDefaultAsync(socio => socio.Id == id);
        if (socio == null)
        {
            return null;
        }

        if (!socio.Activo)
        {
            return null;
        }

        var membresiaVigente = await _context.Membresias
            .AsNoTracking()
            .FirstOrDefaultAsync(membresia => membresia.SocioId == id &&
                                              membresia.FechaVencimiento > DateTime.UtcNow);

        if (membresiaVigente == null)
        {
            return null;
        }

        var asistencia = new Entidades.Asistencia
        {
            SocioId = id,
            FechaHoraEntrada = DateTime.UtcNow
        };

        _context.Asistencias.Add(asistencia);
        await _context.SaveChangesAsync();

        return new AsistenciaResponse
        {
            Id = asistencia.Id,
            SocioId = asistencia.SocioId,
            NombreSocio = socio.Nombre,
            FechaHoraEntrada = asistencia.FechaHoraEntrada,
            FechaCreacion = asistencia.FechaCreacion
        };
    }



    public async Task<AsistenciaResponse?> ObtenerAsistenciaId(AsistenciaRequest request)
    {
        var asistencia = await _context.Asistencias
            .Include(a => a.Socio)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.SocioId);

        if (asistencia == null)
        {
            return null;
        }


        return new AsistenciaResponse
        {
            Id = asistencia.Id,
            SocioId = asistencia.SocioId,
            NombreSocio = asistencia.Socio?.Nombre ?? "",
            FechaHoraEntrada = asistencia.FechaHoraEntrada,
            FechaCreacion = asistencia.FechaCreacion
        };
    }

    public async Task<IEnumerable<AsistenciaResponse>?> ObtenerAsistenciasDelDia()
    {
        var hoy = DateTime.UtcNow.Date;
        var manana = hoy.AddDays(1);

        var asistencias = await _context.Asistencias
            .Include(a => a.Socio)
            .AsNoTracking()
            .Where(a => a.FechaHoraEntrada >= hoy && a.FechaHoraEntrada < manana)
            .OrderByDescending(a => a.FechaHoraEntrada)
            .ToListAsync();

        if (asistencias == null)
        {
            return null;
        }

        return (asistencias.Select(a => new AsistenciaResponse
        {
            Id = a.Id,
            SocioId = a.SocioId,
            NombreSocio = a.Socio?.Nombre ?? "",
            FechaHoraEntrada = a.FechaHoraEntrada,
            FechaCreacion = a.FechaCreacion
        }));
    }

    public async Task<IEnumerable<AsistenciaResponse>?> ObtenerHistorialAsistenciasDeSocio(AsistenciaRequest request)
    {
        // Verificar que el socio existe
        var socioExiste = await _context.Socios
            .AsNoTracking()
            .AnyAsync(s => s.Id == request.SocioId);

        if (!socioExiste)
        {
            return null;
        }

        var asistencias = await _context.Asistencias
            .Include(a => a.Socio)
            .AsNoTracking()
            .Where(a => a.SocioId == request.SocioId)
            .OrderByDescending(a => a.FechaHoraEntrada)
            .Take(30)  // Últimas 30 entradas
            .ToListAsync();

        var respuesta = asistencias.Select(a => new AsistenciaResponse
        {
            Id = a.Id,
            SocioId = a.SocioId,
            NombreSocio = a.Socio?.Nombre ?? "",
            FechaHoraEntrada = a.FechaHoraEntrada,
            FechaCreacion = a.FechaCreacion
        });
        
        return respuesta;
    }
}
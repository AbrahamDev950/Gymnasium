using Gym.Datos;
using Gym.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Features.Socio.Services;

public class SocioRegistro : ISocioRegistro
{
    private readonly ApplicationDBContext _context;
    private readonly IAlmacenadorArchivos _almacenadorArchivos;

    public SocioRegistro(ApplicationDBContext context, IAlmacenadorArchivos almacenadorArchivos)
    {
        _context = context;
        _almacenadorArchivos = almacenadorArchivos;
    }

    public async Task<SocioResponse?> RegistrarSocio(CrearSocioRequest request)
    {
        var emailSanitizado = request.Email.Trim()
            .ToLower();

        var emailYaRegistrado = await _context.Socios
            .AnyAsync(socio => socio.Email == emailSanitizado);

        if (emailYaRegistrado)
        {
            throw new InvalidOperationException("Este correo electrónico ya está registrado.");
        }
        

        var socio = new Entidades.Socio
        {
            Nombre = request.Nombre.Trim(),
            Apellido = request.Apellido.Trim(),
            Email = emailSanitizado,
            Telefono = request.Telefono.Trim(), 
            // Guardar la foto de perfil si se proporciona, de lo contrario, establecer como cadena vacía
            FotoPerfil = request.FotoPerfil != null
                ? await _almacenadorArchivos.Almacenar("socios", request.FotoPerfil)
                : string.Empty,
            FechaIngreso = DateTime.UtcNow,
        };

        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();

        var socioDto = new SocioResponse()
        {
            Id = socio.Id,
            Nombre = socio.Nombre,
            Apellido = socio.Apellido,
            Email = socio.Email,
            Telefono = socio.Telefono,
            FechaIngreso = socio.FechaIngreso,
            FotoPerfil = socio.FotoPerfil,
            Activo = socio.Activo
        };

        return socioDto;
    }

    public async Task<IEnumerable<SocioResponse>?> ObtenerSociosRegistrados()
    {
        var socios = await _context.Socios
            .AsNoTracking()
            .OrderBy(socio => socio.Id)
            .Select(socio => new SocioResponse
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Email = socio.Email,
                Telefono = socio.Telefono,
                FechaIngreso = socio.FechaIngreso,
                FotoPerfil = socio.FotoPerfil,
                Activo = socio.Activo
            })
            .ToListAsync();

        return socios;
    }

    public async Task<IEnumerable<SocioResponse>?> BuscarSocios(string? termino)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            throw new ArgumentException("El término de búsqueda no puede estar vacío.", nameof(termino));
        }

        termino = termino.Trim().ToLower();

        var socios = await _context.Socios
            .AsNoTracking()
            .Where(socio => socio.Email != null && (socio.Nombre.ToLower().Contains(termino) ||
                                                    socio.Apellido.ToLower().Contains(termino) ||
                                                    socio.Email.ToLower().Contains(termino)))
            .OrderBy(socio => socio.Id)
            .Select(socio => new SocioResponse
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Email = socio.Email,
                Telefono = socio.Telefono,
                FechaIngreso = socio.FechaIngreso,
                FotoPerfil = socio.FotoPerfil,
                Activo = socio.Activo
            })
            .ToListAsync();

        return socios;
    }

    public async Task<IEnumerable<SocioResponse>?> ObtenerPorId(int id)
    {
        var socio = await _context.Socios
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(socio => new SocioResponse
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Email = socio.Email,
                Telefono = socio.Telefono,
                FechaIngreso = socio.FechaIngreso,
                FotoPerfil = socio.FotoPerfil,
                Activo = socio.Activo
            })
            .FirstOrDefaultAsync();

        if (socio == null)
        {
            return null;
        }

        return new List<SocioResponse> { socio };
    }

    public async Task<SocioResponse?> ActualizarSocio(int id, ActualizarSocioRequest request)
    {
        var socioExistente = await _context.Socios.FindAsync(id);
        if (socioExistente == null)
        {
            return null;
        }

        var emailSanitizado = request.Email.Trim().ToLower();

        var emailYaRegistrado = await _context.Socios
            .AnyAsync(socio => socio.Email == emailSanitizado && socio.Id != id);

        if (emailYaRegistrado)
        {
            throw new InvalidOperationException("Este correo electrónico ya está registrado por otro socio.");
        }

        if (socioExistente.Activo && request.Activo == false)
        {
            socioExistente.Activo = false;
        }
        else if (!socioExistente.Activo && request.Activo)
        {
            socioExistente.Activo = true;
        }
        else
        {
            throw new InvalidOperationException(
                "No se puede cambiar el estado del socio. El estado actual es el mismo que el solicitado.");
        }
        if(socioExistente.FotoPerfil != null && request.FotoPerfil != null)
        {
            // Eliminar la foto de perfil anterior si existe
            await _almacenadorArchivos.BorrarArchivoAsync(socioExistente.FotoPerfil, "socios");
            // Guardar la nueva foto de perfil
            socioExistente.FotoPerfil = await _almacenadorArchivos.Almacenar("socios", request.FotoPerfil);
        }
        else if (request.FotoPerfil != null)
        {
            // Guardar la nueva foto de perfil si no había una anterior
            socioExistente.FotoPerfil = await _almacenadorArchivos.Almacenar("socios", request.FotoPerfil);
        }

        socioExistente.Nombre = request.Nombre.Trim();
        socioExistente.Apellido = request.Apellido.Trim();
        socioExistente.Email = request.Email.Trim().ToLower();
        socioExistente.Telefono = request.Telefono.Trim();
        socioExistente.Activo = request.Activo;

        await _context.SaveChangesAsync();

        return new SocioResponse
        {
            Id = socioExistente.Id,
            Nombre = socioExistente.Nombre,
            Apellido = socioExistente.Apellido,
            Email = socioExistente.Email,
            Telefono = socioExistente.Telefono,
            FechaIngreso = socioExistente.FechaIngreso,
            Activo = socioExistente.Activo
        };
    }

    public async Task<List<SocioResponse>?> ObtenerTodos(bool? activos)
    {
        var consulta = _context.Socios
            .AsNoTracking()
            .AsQueryable();

        if (activos.HasValue)
        {
            consulta = consulta
                .Where(socio => socio.Activo == activos.Value);
        }

        var socios = await consulta
            .OrderBy(socio => socio.Id)
            .Select(socio => new SocioResponse
            {
                Id = socio.Id,
                Nombre = socio.Nombre,
                Apellido = socio.Apellido,
                Email = socio.Email,
                Telefono = socio.Telefono,
                FechaIngreso = socio.FechaIngreso,
                Activo = socio.Activo
            })
            .ToListAsync();

        return socios;
    }
    public async Task<SocioResponse?> DesactivarSocio(int id)
    {
        var socioExistente = await _context.Socios.FindAsync(id);
        if (socioExistente == null)
        {
            return null;
        }

        if (!socioExistente.Activo)
        {
            throw new InvalidOperationException("El socio ya está desactivado.");
        }

        socioExistente.Activo = false;
        await _context.SaveChangesAsync();

        return new SocioResponse
        {
            Id = socioExistente.Id,
            Nombre = socioExistente.Nombre,
            Apellido = socioExistente.Apellido,
            Email = socioExistente.Email,
            Telefono = socioExistente.Telefono,
            FechaIngreso = socioExistente.FechaIngreso,
            Activo = socioExistente.Activo
        };
    }
    public async Task<SocioResponse?> ActivarSocio(int id)
    {
        var socioExistente = await _context.Socios.FindAsync(id);
        if (socioExistente == null)
        {
            return null;
        }

        if (socioExistente.Activo)
        {
            throw new InvalidOperationException("El socio ya está activo.");
        }

        socioExistente.Activo = true;
        await _context.SaveChangesAsync();

        return new SocioResponse
        {
            Id = socioExistente.Id,
            Nombre = socioExistente.Nombre,
            Apellido = socioExistente.Apellido,
            Email = socioExistente.Email,
            Telefono = socioExistente.Telefono,
            FechaIngreso = socioExistente.FechaIngreso,
            Activo = socioExistente.Activo
        };
    }
}
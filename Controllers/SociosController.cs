using Gym.Datos;
using Gym.DTOs;
using Gym.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/socios")]
[Authorize(Roles = "Administrador")]
public class SociosController : ControllerBase
{
    private readonly ApplicationDBContext _context;
    
    public SociosController(ApplicationDBContext context)
    {
        _context = context;
    }
    
    [HttpPost]
    public async Task<ActionResult> Post(CrearSocioRequest peticionCrearSocio)
    {
        var emailSanitizado = peticionCrearSocio.Email?.
            Trim()
            .ToLower();
        
        var emailYaRegistrado = await _context.Socios
            .AnyAsync(socio => socio.Email == emailSanitizado);
        
        if (emailYaRegistrado)
        {
            return Conflict(new { mensaje = "Este correo electrónico ya está registrado." });
        }

        var socio = new Socio
        {
            Nombre = peticionCrearSocio.Nombre.Trim(),
            Apellido = peticionCrearSocio.Apellido.Trim(),
            Email = emailSanitizado,
            Telefono = peticionCrearSocio.Telefono.Trim(),
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
            Activo = socio.Activo
        };
        
        return CreatedAtAction(
            nameof(ObtenerPorId),
            new
            {
                id = socio.Id,
                peticionCrearSocio
            });
    }
    
    [HttpGet("total-registrados")]
    public async Task<ActionResult<IEnumerable<SocioResponse>>> Get()
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
                Activo = socio.Activo
            })
            .ToListAsync();

        return Ok(socios);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<IEnumerable<SocioResponse>>> ObtenerPorId(int id)
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
                Activo = socio.Activo
            })
            .FirstOrDefaultAsync();

        if (socio == null)
        {
            return NotFound(new { mensaje = "Socio no encontrado." });
        }

        return Ok(socio);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SocioResponse>> Put(int id, ActualizarSocioRequest peticionActualizar)
    {
        var socioExistente = await _context.Socios.FindAsync(id);
        if (socioExistente == null)
        {
            return NotFound(new { mensaje = "Socio no encontrado." });
        }
        var emailSanitizado = peticionActualizar.Email?.Trim().ToLower();
        
        var emailYaRegistrado = await _context.Socios
            .AnyAsync(socio => socio.Email == emailSanitizado && socio.Id != id);

        if (emailYaRegistrado)
        {
            return Conflict(new { mensaje = "Este correo electrónico ya está registrado por otro socio." });
        }
        
        
        socioExistente.Nombre = peticionActualizar.Nombre.Trim();
        socioExistente.Apellido = peticionActualizar.Apellido.Trim();
        socioExistente.Email = emailSanitizado;
        socioExistente.Telefono = peticionActualizar.Telefono.Trim();

        await _context.SaveChangesAsync();

        var socioActualizado = new SocioResponse
        {
            Id = socioExistente.Id,
            Nombre = socioExistente.Nombre,
            Apellido = socioExistente.Apellido,
            Email = socioExistente.Email,
            Telefono = socioExistente.Telefono,
            FechaIngreso = socioExistente.FechaIngreso,
            Activo = socioExistente.Activo
        };

        return Ok(socioActualizado);
    }

    [HttpPatch("{id:int}/desactivar")]
    public async Task<ActionResult> DesactivarSocio(int id)
    {
        var socioExistente = await _context.Socios.FindAsync(id);
        if (socioExistente == null)
        {
            return NotFound(new { mensaje = "Socio no encontrado." });
        }
        if(!socioExistente.Activo)
        {
            return BadRequest(new { mensaje = "El socio ya está desactivado." });
        }
        socioExistente.Activo = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPatch("{id:int}/activar")]
    public async Task<ActionResult> ActivarSocio(int id)
    {
        var socioExistente = await _context.Socios.FindAsync(id);
        if (socioExistente == null)
        {
            return NotFound(new { mensaje = "Socio no encontrado." });
        }
        if(socioExistente.Activo)
        {
            return BadRequest(new { mensaje = "El socio ya está activo." });
        }
        socioExistente.Activo = true;
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpGet]
    public async Task<ActionResult<List<SocioResponse>>> ObtenerTodos([FromQuery] bool? activos)
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

        return Ok(socios);
    }
}
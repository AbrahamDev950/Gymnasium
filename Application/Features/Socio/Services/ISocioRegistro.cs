using Gym.DTOs;

namespace Gym.Application.Features.Socio.Services;

public interface ISocioRegistro
{
    Task<SocioResponse?> RegistrarSocio(CrearSocioRequest request);
    Task<IEnumerable<SocioResponse>?> ObtenerSociosRegistrados();
    Task<IEnumerable<SocioResponse>?> BuscarSocios(string termino);
    Task<IEnumerable<SocioResponse>?> ObtenerPorId(int id);
    Task<List<SocioResponse>?> ObtenerTodos(bool? activos);
    Task<SocioResponse?> ActualizarSocio(int id, ActualizarSocioRequest request);
    Task<SocioResponse?> DesactivarSocio(int id);
    Task<SocioResponse?> ActivarSocio(int id);
}
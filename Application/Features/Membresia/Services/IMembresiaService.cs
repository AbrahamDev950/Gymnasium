using Gym.DTOs;

namespace Gym.Controllers.Services;

public interface IMembresiaService
{
    Task<MembresiaResponse> CrearMembresia(CrearMembresiaRequest request);
    Task<MembresiaResponse> ObtenerMembresíaPorId(int id);
    Task<MembresiaResponse> ObtenerMembresíaVigenteDeSocio(int socioId);
    Task<IEnumerable<MembresiaResponse>> ObtenerHistorialMembresiasDeSocio(int socioId);
    Task<MembresiaResponse> RenovarMembresia(int id, RenovarMembresiaRequest request);
    Task<IEnumerable<MembresiaResponse>> ObtenerMembresias(bool proximasAVencer = false);

}
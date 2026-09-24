using Gym.DTOs;

namespace Gym.Application.Features.Asistencia.Services;

public interface IAsistenciaService
{
    Task<AsistenciaResponse?> RegistrarAsistencia(int id);
    Task<AsistenciaResponse?> ObtenerAsistenciaId(AsistenciaRequest request); 
    Task<IEnumerable<AsistenciaResponse>?> ObtenerAsistenciasDelDia();
    Task<IEnumerable<AsistenciaResponse>?> ObtenerHistorialAsistenciasDeSocio(AsistenciaRequest request);
}
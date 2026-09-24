namespace Gym.DTOs;

public class AsistenciaResponse
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public required string NombreSocio { get; set; }
    public DateTime FechaHoraEntrada { get; set; }
    public DateTime FechaCreacion { get; set; }
}
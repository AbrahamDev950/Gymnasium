using System.ComponentModel.DataAnnotations;

namespace Gym.Entidades;

/// <summary>
/// Entidad que representa la asistencia de un socio al gimnasio.
/// </summary>
public class Asistencia
{
    public int Id { get; set; }
    
    [Required]
    public int SocioId { get; set; }
    public Socio? Socio { get; set; }  // Relación N:1 (muchas asistencias, 1 socio)
    
    [Required]
    public DateTime FechaHoraEntrada { get; set; }
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
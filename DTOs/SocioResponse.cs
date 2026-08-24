namespace Gym.DTOs;

/// <summary>
/// Datos que puede devolver la API de un socio.
/// </summary>
public class SocioResponse
{
    public int Id { get; set; }

    public required string Nombre { get; set; }

    public required string Apellido { get; set; }

    public required string Email { get; set; }

    public required string Telefono { get; set; }

    public DateTime FechaIngreso { get; set; }
    
    public bool Activo { get; set; }
}
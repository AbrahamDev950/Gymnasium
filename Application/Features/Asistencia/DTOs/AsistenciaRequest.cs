using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;

public class AsistenciaRequest
{
    [Required(ErrorMessage = "El ID del socio es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del socio debe ser válido.")]
    public required int SocioId { get; set; }
}
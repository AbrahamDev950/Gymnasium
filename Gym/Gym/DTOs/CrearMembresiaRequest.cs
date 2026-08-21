using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;

public class CrearMembresiaRequest
{
    [Required(ErrorMessage = "El ID del socio es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del socio debe ser válido.")]
    public required int SocioId { get; set; }
    
    [Required(ErrorMessage = "El ID del plan es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del plan debe ser válido.")]
    public required int PlanId { get; set; }
}
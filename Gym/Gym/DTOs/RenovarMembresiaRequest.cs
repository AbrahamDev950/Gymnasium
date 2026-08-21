using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;

public class RenovarMembresiaRequest
{
    [Required(ErrorMessage = "El ID del plan es requerido.")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del plan debe ser válido.")]
    public required int PlanId { get; set; }
}
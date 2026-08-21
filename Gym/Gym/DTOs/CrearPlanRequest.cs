using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;
/// <summary>
/// DTO para crear un nuevo plan.
/// Recibe los datos necesarios para crear un plan de entrenamiento o suscripción en el gimnasio.
/// No recibe Id ni fecha de creación, ya que estos se generan automáticamente en el backend.
/// </summary>
public class CrearPlanRequest
{
    [Required(ErrorMessage = "El nombre del plan es requerido.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public required string Nombre { get; set; }
    
    [Required(ErrorMessage = "La duración es requerida.")]
    [Range(1, 365, ErrorMessage = "La duración debe estar entre 1 y 365 días.")]
    public required int Duracion { get; set; }
    
    [Required(ErrorMessage = "El precio es requerido.")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
    public required decimal Precio { get; set; }
}
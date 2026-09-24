using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;
/// <summary>
/// DTO para actualizar un plan existente.
/// Solamente se pueden actualizar los campos Nombre, Duracion y Precio.
/// Los campos Id y FechaCreacion no se pueden modificar.
/// El cliente puede actualizar uno o varios campos, por lo que todos son opcionales.
/// </summary>
public class ActualizarPlanRequest
{
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public string? Nombre { get; set; }
    
    [Range(1, 365, ErrorMessage = "La duración debe estar entre 1 y 365 días.")]
    public int? Duracion { get; set; }
    
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
    public decimal? Precio { get; set; }
}
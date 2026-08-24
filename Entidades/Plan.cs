using System.ComponentModel.DataAnnotations;

namespace Gym.Entidades;

public class Plan
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El campo {0} debe tener entre 3 y 100 caracteres.")]
    public required string Nombre { get; set; }
    
    [Required]
    [Range(1, 365, ErrorMessage = "La duración debe estar entre 1 y 365 días.")]
    public required int Duracion { get; set; }  // En días
    
    [Required]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
    public required decimal Precio { get; set; }
    
    public bool Activo { get; set; } = true;
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
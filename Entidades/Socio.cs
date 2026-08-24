using System.ComponentModel.DataAnnotations;

namespace Gym.Entidades;

public class Socio
{
    public int Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El campo {0} debe tener entre 3 y 50 caracteres.")]
    public required string Nombre { get; set; }
    
    
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El campo {0} debe tener entre 3 y 50 caracteres.")]
    public required string Apellido { get; set; }
    
    
    [Required]
    [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electrónico válido.")]
    [StringLength(100, ErrorMessage = "El campo {0} no puede tener más de 100 caracteres.")]
    public required string? Email { get; set; }
    
    
    [Required]
    [Phone(ErrorMessage = "El campo {0} debe ser un número de teléfono válido.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "El campo {0} debe tener exactamente 10 dígitos.")]
    public required string Telefono { get; set; }
    
    public required DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    
    public bool Activo { get; set; } = true;
}
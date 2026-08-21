using System.ComponentModel.DataAnnotations;

namespace Gym.Entidades;

public class Administrador
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(25,
        MinimumLength = 3,
        ErrorMessage = "El campo {0} debe tener entre 3 y 25 caracteres.")]
    public required string NombreUsuario { get; set; }
    
    [Required]
    public required string PasswordHash { get; set; }
}
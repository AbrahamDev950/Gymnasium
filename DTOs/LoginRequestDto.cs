using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    public required string NombreUsuario { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public required string Password { get; set; }
}
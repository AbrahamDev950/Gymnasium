using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;
/// <summary>
/// DTO para actualizar la información de un socio.
/// </summary>
public class ActualizarSocioRequest
{
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El campo {0} debe tener entre 2 y 50 caracteres."
    )]
    public string? Nombre { get; set; }


    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El campo {0} debe tener entre 2 y 50 caracteres."
    )]
    public string? Apellido { get; set; }
    
    [EmailAddress(
        ErrorMessage = "El campo {0} debe ser un correo electrónico válido."
    )]
    [StringLength(254)]
    public string? Email { get; set; }
    
    [Phone(
        ErrorMessage = "El campo {0} debe ser un teléfono válido."
    )]
    [StringLength(10), MinLength(10, ErrorMessage = "El campo {0} debe tener exactamente 10 dígitos.")]
    public string? Telefono { get; set; }
    
    public bool Activo { get; set; }
    public IFormFile? FotoPerfil { get; set; }
}
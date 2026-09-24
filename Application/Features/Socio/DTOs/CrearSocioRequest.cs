using System.ComponentModel.DataAnnotations;

namespace Gym.DTOs;

// El admin creara un socio pero solo con nombre, apellido, email y telefono.
// La fecha de registro, status del socio y el id del socio se generaran automaticamente en el backend.
public class CrearSocioRequest
{
    // Aplicamos validaciones para los campos de nombre, apellido, email y telefono.
    [Required]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El campo {0} debe tener entre 2 y 50 caracteres."
    )]
    public required string Nombre { get; set; }

    [Required]
    [StringLength(
        50,
        MinimumLength = 2,
        ErrorMessage = "El campo {0} debe tener entre 2 y 50 caracteres."
    )]
    public required string Apellido { get; set; }

    [Required]
    [EmailAddress(
        ErrorMessage = "El campo {0} debe ser un correo electrónico válido."
    )]
    [StringLength(254)]
    public required string Email { get; set; }

    [Required]
    [Phone(
        ErrorMessage = "El campo {0} debe ser un teléfono válido."
    )]
    [StringLength(10), MinLength(10, ErrorMessage = "El campo {0} debe tener exactamente 10 dígitos.")]
    public required string Telefono { get; set; }
}
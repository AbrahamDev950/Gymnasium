using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gym.Entidades;

namespace Gym.Entidades;

public class Membresia
{
    public int Id { get; set; }
    
    [Required]
    // Nos indica a que socio pertenece la membresía.
    public int SocioId { get; set; }
    // Relación 1:N Socio → Membresia
    // Ya que un socio puede tener múltiples membresías a lo largo del tiempo,
    // pero cada membresía pertenece a un solo socio.
    public Socio? Socio { get; set; } 
    
    [Required]
    // Nos indica que plan fue adquirido en la membresía.
    public int PlanId { get; set; }
    // Relación 1:N Plan → Membresia
    // Ya que un plan puede ser adquirido por múltiples socios,
    public Plan? Plan { get; set; }  
    
    [Required]
    public DateTime FechaInicio { get; set; }
    
    [Required]
    // Calculada desde la fecha de inicio + la duración del plan en días.
    public DateTime FechaVencimiento { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    // Precio congelado del plan en el momento
    // de la compra de la membresía, para que no cambie si el plan cambia de precio.
    public decimal PrecioAplicado { get; set; }  
    
    [Required]
    [StringLength(50)]
    // "Activa", "Vencida", "Cancelada"
    public required string Estado { get; set; }  
    // Fecha de creación de la membresía
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
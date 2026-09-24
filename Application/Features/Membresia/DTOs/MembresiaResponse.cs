namespace Gym.DTOs;

public class MembresiaResponse
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public required string NombreSocio { get; set; }
    public int PlanId { get; set; }
    public required string NombrePlan { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public decimal PrecioAplicado { get; set; }
    public required string Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int DiasRestantes { get; set; }  // Para conveniencia del cliente
}
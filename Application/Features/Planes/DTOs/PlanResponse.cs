namespace Gym.DTOs;
/// <summary>
/// Datos que puede devolver la API de un plan.
/// Devuelve todos los campos de la entidad Plan para que el cliente pueda ver la información completa del plan.
/// </summary>
public class PlanResponse
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public required int Duracion { get; set; }
    public required decimal Precio { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
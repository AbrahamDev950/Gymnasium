namespace Gym.DTOs;

public class DashboardResponse
{
    public int AsistenciasHoy { get; set; }
    public int MembresíasVigentes { get; set; }
    public int MembresíasProximasAVencer { get; set; }
    public int SociosActivos { get; set; }
    public int SociosInactivos { get; set; }
    public decimal IngresosDelMes { get; set; }
    public DateTime FechaConsulta { get; set; }
}
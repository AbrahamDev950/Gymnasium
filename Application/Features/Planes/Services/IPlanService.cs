using Gym.DTOs;

namespace Gym.Application.Features.Planes.Services;

public interface IPlanService
{
    Task<PlanResponse> CrearPlan(CrearPlanRequest request);
    Task<IEnumerable<PlanResponse>> ObtenerTodosLosPlanes();
    Task<PlanResponse> ObtenerPlanPorId(int id);
    Task<PlanResponse> ActualizarPlan(int id, ActualizarPlanRequest request);
    Task<PlanResponse> DesactivarPlan(int id);
    Task<PlanResponse> ReactivarPlan(int id);



}
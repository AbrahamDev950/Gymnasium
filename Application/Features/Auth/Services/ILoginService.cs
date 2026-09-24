using Gym.DTOs;

namespace Gym.Servicios;

public interface ILoginService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    
    
}
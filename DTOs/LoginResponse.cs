namespace Gym.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; }
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
}
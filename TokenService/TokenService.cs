using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gym.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace Gym.Servicios;

public class TokenService
{
    private readonly IConfiguration configuration;

    public TokenService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GenerarToken(Administrador administrador)
    {
        var jwtKey = configuration["Jwt:Key"]
                     ?? throw new InvalidOperationException(
                         "No se encontró Jwt:Key."
                     );

        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];

        var expirationMinutes =
            configuration.GetValue<int>("Jwt:ExpirationMinutes");

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                administrador.Id.ToString()
            ),
            new(
                ClaimTypes.Name,
                administrador.NombreUsuario
            ),
            new(
                ClaimTypes.Role,
                "Administrador"
            ),
            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            )
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
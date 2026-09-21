using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gym.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace Gym.Servicios;
/// <summary>
/// Servicio para generar tokens JWT para la autenticación de administradores.
/// </summary>
public class TokenService
{
    private readonly IConfiguration configuration;

    public TokenService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <summary>
    /// Genera un token JWT para el administrador proporcionado.
    /// </summary>
    /// <param name="administrador"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GenerarToken(Administrador administrador)
    {
        // Leemos la clave secreta desde la configuración
        var jwtKey = configuration["Jwt:Key"]
                     ?? throw new InvalidOperationException(
                         "No se encontró Jwt:Key."
                     );
        // Leemos el issuer y el audience desde la configuración
        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];

        // Asignamos el timepo de expiración del token en minutos desde la configuración
        var expirationMinutes =
            configuration.GetValue<int>("Jwt:ExpirationMinutes");

        // Creamos la lista de claims que se incluirán en el token
        var claims = new List<Claim>
        {
            new(
                // El Id del administrador se almacena en el claim "sub" (subject) del token
                JwtRegisteredClaimNames.Sub,
                administrador.Id.ToString()
            ),
            new(
                // El nombre de usuario del administrador se almacena en el claim "name" del token
                ClaimTypes.Name,
                administrador.NombreUsuario
            ),
            new(
                // El rol del administrador se almacena en el claim "role" del token
                ClaimTypes.Role,
                "Administrador"
            ),
            new(
                // El claim "jti" (JWT ID) es un identificador único para el token,
                // que ayuda a prevenir ataques de repetición
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            )
        };

        // Creamos la clave de seguridad simétrica a partir de la clave secreta
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        );
        
        // Creamos las credenciales de firma utilizando la clave y el algoritmo HMAC-SHA256
        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        // Construimos el token JWT con los claims, la fecha de expiración y las credenciales de firma
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        // Devolvemos el token generado como una cadena
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
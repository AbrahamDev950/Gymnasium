using Gym.Datos;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Gym.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Conexion a la base de datos
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection."
    );

// Leer la configuración de JWT desde appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "No se encontró la clave de configuración Jwt:Key."
    );
}
/* Issuer lo usamos para identificar quién emite el token
// Audience para identificar quién es el destinatario del token. 
// Estos valores deben coincidir con los que se configuran en la aplicación cliente que consume la API.*/
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString));
// Agregar el servicio de TokenService para generar tokens JWT
builder.Services.AddScoped<TokenService>();
// Configurar la autenticación JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });






// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await DataSeeder.InicializarAsync(
        app.Services,
        app.Configuration
    );
}


// Area de middlewares

app.UseHttpsRedirection();
// Authentication nos permite identificar al usuario que hace la petición
// mientras que Authorization nos permite determinar si ese usuario tiene
// permisos para realizar la acción solicitada.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
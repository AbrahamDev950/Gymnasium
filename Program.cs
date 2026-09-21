using Gym.Datos;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Gym.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

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

// Validamos nuestro token JWT, para ello necesitamos el issuer y el audience que definimos en appsettings.json
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];


// ------------------ INICIO AREA DE SERVICIOS ------------------
builder.Services.AddControllers();
// Agregamos el servicio de ApplicationDBContext con la cadena de conexión
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString));

// Servicio de TokenService para generar tokens JWT
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
            
            // No dar margen de tiempo para la expiración del token
            ClockSkew = TimeSpan.Zero
        };
    });

// Agregar OpenAPI/Swagger
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    // Configurar Swagger para usar JWT Bearer Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Configurar CORS para permitir solicitudes desde el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
                // Cambiar una vez que se tenga el dominio del frontend
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ------------------ FIN AREA DE SERVICIOS ------------------


// ################### INICIO DEL PIPELINE DE LA APLICACIÓN ###################
var app = builder.Build();

// Usamos archivos estáticos para servir el frontend
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Frontend")
    )
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Frontend")
    )
});

// Esperamos a que se inicialicen los datos en la base de datos,
// Creando un administrador inicial si no existe.
await DataSeeder.InicializarAsync(
    app.Services,
    app.Configuration
);


// app.UseHttpsRedirection();

// Configurar CORS
app.UseCors("Frontend");

// Configuración de OpenAPI y Swagger
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gymnasium API v1");
    });
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
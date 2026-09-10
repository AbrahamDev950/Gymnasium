# Gymnasium - API REST de Gestión de Gimnasio

Gymnasium es una API REST desarrollada con **ASP.NET Core 10**, **Entity Framework Core** y **SQL Server**. El proyecto sirve como aprendizaje y portafolio, implementando conceptos profesionales como autenticación JWT, relaciones de entidades, baja lógica y validaciones de negocio.

## Características

✅ **Autenticación JWT** con rol `Administrador`  
✅ **CRUD de Socios** - Gestión de miembros con baja lógica  
✅ **CRUD de Planes** - Catálogo de membresías  
✅ **CRUD de Membresías** - Suscripciones con historial y renovación  
✅ **Registro de Asistencias** - Control de entrada validando estado y membresía vigente  
✅ **Dashboard** - Resumen de métricas del gimnasio  
✅ **Swagger/OpenAPI** - Documentación interactiva  

---

## Tecnología

- **Backend:** ASP.NET Core 10
- **ORM:** Entity Framework Core 10.0.11
- **Base de Datos:** SQL Server
- **Autenticación:** JWT Bearer
- **Documentación:** Swagger/OpenAPI
- **Lenguaje:** C#
- **Control de versiones:** Git

---

## Instalación

### Requisitos

- .NET 10 SDK
- SQL Server (local o remoto)
- Git

### Pasos

1. **Clonar repositorio:**
```bash
   git clone https://github.com/AbrahamDev950/Gymnasium.git
   cd Gymnasium
```

2. **Restaurar dependencias:**
```bash
   dotnet restore
```

3. **Configurar secretos locales:**
```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=GymnasiumDevDB;User Id=sa;Password=TUCONSTRASENA;TrustServerCertificate=True;"
   dotnet user-secrets set "Jwt:Key" "clave32caracteres"
   dotnet user-secrets set "InitialAdmin:Username" "admin"
   dotnet user-secrets set "InitialAdmin:Password" "admin123"
```

4. **Crear/actualizar base de datos:**
```bash
   dotnet ef database update
```

5. **Ejecutar aplicación:**
```bash
   dotnet run
```


---
Utilizar el puerto asignado como por ejemplo: https://localhost:5001
Una vez ejecutada la aplicación, accede a **Swagger UI**:


Aquí puedes:
- Ver todos los endpoints
- Probar requests directamente
- Copiar ejemplos en cURL/Postman

### Autenticación

1. Usa el endpoint `/api/auth/login` con credenciales:
```json
{
  "email": "admin",
  "password": "admin123",
  "twoFactorCode": "string",
  "twoFactorRecoveryCode": "string"
}
```

2. Copia el token JWT devuelto

3. Haz clic en "Authorize" en Swagger y pega: `Bearer <token>`

---

## Estructura del Proyecto
Gym/
├── Controllers/ # Endpoints REST
│ ├── AuthController.cs
│ ├── SociosController.cs
│ ├── PlanesController.cs
│ ├── MembresíasController.cs
│ ├── AsistenciasController.cs
│ └── DashboardController.cs
├── Datos/ # Acceso a datos
│ ├── ApplicationDBContext.cs
│ └── DataSeeder.cs
├── DTOs/ # Data Transfer Objects
├── Entidades/ # Modelos de negocio
│ ├── Administrador.cs
│ ├── Socio.cs
│ ├── Plan.cs
│ ├── Membresia.cs
│ └── Asistencia.cs
├── Servicios/ # Lógica de negocios
│ └── TokenService.cs
├── Migrations/ # EF Core migrations
├── Program.cs
├── appsettings.json
└── Gym.csproj


---

## Módulos Implementados

### 1. Autenticación (Administrador)

**Login:**

POST /api/auth/login
Content-Type: application/json

{
"nombreUsuario": "admin",
"contraseña": "admin123"
}


**Respuesta:**
```json
{
  "token": "ey...",
  "expirationMinutes": 60,
  "administrador": {
    "mensaje": "Credenciales correctas y token generado.",
    "id": 1,
    "nombreUsuario": "admin"
  }
}
```

---

### 2. Socios

**Crear socio:**
POST /api/socios
Authorization: Bearer <TOKEN>
Content-Type: application/json

**Respuesta:**
```json
{
  "id": 2,
  "nombre": "Lupita",
  "apellido": "Diaz",
  "email": "lupita@example.com",
  "telefono": "0002136547",
  "fechaIngreso": "2026-09-10T19:37:51.1797832Z",
  "activo": true
}
```



**Listar socios:**
GET /api/socios
GET /api/socios?activo=true


---
### 3. Planes

**Crear plan:**

POST /api/planes
Authorization: Bearer <TOKEN>

**Respuesta:**
```json
{
"id": 1,
"nombre": "mensual",
"duracion": 30,
"precio": 200,
"activo": true,
"fechaCreacion": "2026-09-10T19:43:47.5994276Z"
}
```
---

### 4. Membresías

**Crear membresía:**

POST /api/membresias
Authorization: Bearer <TOKEN>

{
"socioId": 1,
"planId": 1
}

**Respuesta:**
```json
{
"id": 1,
"socioId": 1,
"nombreSocio": "Marco Hernandez",
"planId": 1,
"nombrePlan": "mensual",
"fechaInicio": "2026-09-10T19:44:42.5372567Z",
"fechaVencimiento": "2026-10-10T19:44:42.5372567Z",
"precioAplicado": 200,
"estado": "Activa",
"fechaCreacion": "2026-09-10T19:44:42.5376334Z",
"diasRestantes": 29
}
```
---
**Renovar membresía (suma días):**

POST /api/membresias/1/renovar
Authorization: Bearer <TOKEN>

{
"planId": 2
}

**Respuesta:**
```json
{
  "id": 2,
  "socioId": 1,
  "nombreSocio": "Marco Hernandez",
  "planId": 1,
  "nombrePlan": "mensual",
  "fechaInicio": "2026-10-10T19:44:42.5372567",
  "fechaVencimiento": "2026-11-09T19:44:42.5372567",
  "precioAplicado": 200,
  "estado": "Activa",
  "fechaCreacion": "2026-09-10T19:45:54.2755276Z",
  "diasRestantes": 59
}
```

**Ver membresía vigente de socio:**

GET /api/socios/1/membresia-vigente

***Respuesta***
```json
{
  "id": 3,
  "socioId": 2,
  "nombreSocio": "Lupita Diaz",
  "planId": 1,
  "nombrePlan": "mensual",
  "fechaInicio": "2026-09-10T20:22:59.3848152",
  "fechaVencimiento": "2026-10-10T20:22:59.3848152",
  "precioAplicado": 200,
  "estado": "Activa",
  "fechaCreacion": "2026-09-10T20:22:59.3850965",
  "diasRestantes": 29
}
```

**Ver historial:**

GET /api/socios/1/membresias

***Respuesta***
```json
[
  {
    "id": 2,
    "socioId": 1,
    "nombreSocio": "Marco Hernandez",
    "planId": 1,
    "nombrePlan": "mensual",
    "fechaInicio": "2026-10-10T19:44:42.5372567",
    "fechaVencimiento": "2026-11-09T19:44:42.5372567",
    "precioAplicado": 200,
    "estado": "Activa",
    "fechaCreacion": "2026-09-10T19:45:54.2755276",
    "diasRestantes": 59
  },
  {
    "id": 1,
    "socioId": 1,
    "nombreSocio": "Marco Hernandez",
    "planId": 1,
    "nombrePlan": "mensual",
    "fechaInicio": "2026-09-10T19:44:42.5372567",
    "fechaVencimiento": "2026-10-10T19:44:42.5372567",
    "precioAplicado": 200,
    "estado": "Activa",
    "fechaCreacion": "2026-09-10T19:44:42.5376334",
    "diasRestantes": 29
  }
]
```

---

### 5. Asistencias

**Registrar entrada:**

POST /api/asistencias
Authorization: Bearer <TOKEN>

{
"socioId": 1
}

**Respuesta:**
```json
{
"id": 1,
"socioId": 1,
"nombreSocio": "Marco Hernandez",
"fechaHoraEntrada": "2026-09-10T19:46:58.4682852Z",
"fechaCreacion": "2026-09-10T19:46:58.4681722Z"
}
```
---

**Ver asistencias del día:**

GET /api/asistencias/dia/hoy

***Respuesta***
```json
[
  {
    "id": 2,
    "socioId": 2,
    "nombreSocio": "Lupita Diaz",
    "fechaHoraEntrada": "2026-09-10T20:23:10.1989283",
    "fechaCreacion": "2026-09-10T20:23:10.1986025"
  },
  {
    "id": 1,
    "socioId": 1,
    "nombreSocio": "Marco Hernandez",
    "fechaHoraEntrada": "2026-09-10T19:46:58.4682852",
    "fechaCreacion": "2026-09-10T19:46:58.4681722"
  }
]
```

**Ver historial de un socio:**

GET /api/asistencias/socio/1

***Respuesta***
```json
[
  {
    "id": 3,
    "socioId": 1,
    "nombreSocio": "Marco Hernandez",
    "fechaHoraEntrada": "2026-09-10T20:25:42.820245",
    "fechaCreacion": "2026-09-10T20:25:42.8202444"
  },
  {
    "id": 1,
    "socioId": 1,
    "nombreSocio": "Marco Hernandez",
    "fechaHoraEntrada": "2026-09-10T19:46:58.4682852",
    "fechaCreacion": "2026-09-10T19:46:58.4681722"
  }
]
```


---

### 6. Dashboard

**Obtener resumen:**

GET /api/dashboard


**Respuesta:**
```json
{
  "asistenciasHoy": 1,
  "membresíasVigentes": 2,
  "membresíasProximasAVencer": 0,
  "sociosActivos": 2,
  "sociosInactivos": 0,
  "ingresosDelMes": 400,
  "fechaConsulta": "2026-09-10T19:47:27.736972Z"
}
```

---
## Conceptos Implementados

### Baja Lógica
Los socios y planes no se eliminan físicamente. Se desactivan con `Activo = false`, preservando historial.

### Precio Histórico Congelado
Cada membresía almacena `PrecioAplicado` en el momento de compra. Cambios futuros en el plan no afectan membresías antiguas.

### Estado Calculado Dinámicamente
El estado de una membresía ("Activa"/"Vencida") se calcula en tiempo real basado en `FechaVencimiento`, no se almacena estáticamente.

### Renovación sin Pérdida de Días
Al renovar una membresía vigente, la nueva comienza desde el vencimiento de la anterior, sin perder días de cobertura.

### Validación en Múltiples Niveles
- DTO (anotaciones `[Required]`, `[Range]`, etc.)
- Controller (lógica de negocio)
- Base de datos (índices únicos, FK con restricciones)

---

## Pruebas

### Con Postman/Rider HTTP Client

Todos los endpoints están documentados en Swagger. Usa el token JWT obtenido en login para proteger operaciones de escritura.

### Con xUnit (próximo)

Las pruebas automatizadas cubrirán:
- Autenticación y autorización
- CRUD de cada módulo
- Validaciones de negocio
- Casos de error (404, 409, 400)

---

## Autor

Abraham Hernandez - Backend Developer   
Portafolio: [GitHub](https://github.com/AbrahamDev950)

---

MIT

---

## Próximas Mejoras

- [ ] Pruebas automatizadas (xUnit)
- [ ] Endpoint de reportes de ingresos por rango de fechas
- [ ] Notificaciones de membresías próximas a vencer


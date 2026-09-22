# 🏋️ Gymnasium — API REST para Gestión de Gimnasio

**Gymnasium** es una aplicación web para la gestión de un gimnasio, desarrollada como proyecto de aprendizaje y portafolio profesional.

El proyecto implementa una **API REST con ASP.NET Core 10**, **Entity Framework Core** y **SQL Server**, incorporando autenticación mediante JWT, autorización por roles, persistencia de datos, validaciones de negocio y un frontend web para consumir la API.

## ✨ Características

* 🔐 Autenticación y autorización mediante **JWT Bearer**
* 👤 Gestión de **Socios**
* 📋 Gestión de **Planes**
* 💳 Gestión de **Membresías**
* 🔄 Renovación de membresías
* 📅 Historial de membresías
* 🚪 Registro y consulta de **Asistencias**
* 📊 Dashboard con métricas del gimnasio
* 🗄️ Persistencia mediante **SQL Server + Entity Framework Core**
* 📖 Documentación interactiva mediante **Swagger/OpenAPI**
* 🐳 Contenerización mediante **Docker**
* 🌐 Interfaz web para consumir la API

---

# 🛠️ Tecnologías

| Tecnología                   | Uso                               |
| ---------------------------- | --------------------------------- |
| **C#**                       | Lenguaje principal                |
| **ASP.NET Core 10**          | Desarrollo de la API REST         |
| **Entity Framework Core 10** | ORM y acceso a datos              |
| **SQL Server**               | Base de datos relacional          |
| **JWT Bearer**               | Autenticación                     |
| **Swagger / OpenAPI**        | Documentación y pruebas de la API |
| **Docker**                   | Contenerización                   |
| **Git / GitHub**             | Control de versiones              |
| **HTML / CSS / JavaScript**  | Frontend                          |

---

# 📋 Requisitos

Para ejecutar el proyecto mediante Docker necesitas:

* [Docker](https://www.docker.com/)
* Docker Compose
* Git

No es necesario instalar SQL Server directamente en la máquina si se utiliza la configuración proporcionada por Docker Compose.

---

# 🚀 Instalación

## 1. Clonar el repositorio

```bash
git clone https://github.com/AbrahamDev950/Gymnasium.git
cd Gymnasium
```

## 2. Configurar las variables de entorno

Crea un archivo `.env` en la raíz del proyecto:

```bash
touch .env
```

Ejemplo de configuración:

```env
# SQL Server
MSSQL_SA_PASSWORD=123456789MIContrasenaSegura

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production

# Connection String
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=Gymnasium;User Id=sa;Password=123456789MIContrasenaSegura;TrustServerCertificate=True

# JWT
Jwt__Key=MiSuperClaveSeguraParaGymnasium123456789
Jwt__Issuer=https://localhost:5000
Jwt__Audience=https://localhost:5000

# Initial Administrator
InitialAdmin__Username=admin
InitialAdmin__Password=admin123
```


## 3. Ejecutar con Docker

```bash
docker compose up --build
```

Si tu instalación utiliza la versión antigua de Docker Compose:

```bash
sudo docker-compose up --build
```

Docker iniciará los servicios necesarios para la aplicación.

---

# 🌐 Acceso a la aplicación

Una vez que los contenedores estén ejecutándose:

### API

```text
http://localhost:5000
```

### Swagger UI

```text
http://localhost:5000/swagger
```

### Frontend

```text
http://localhost:5000
```

Desde Swagger puedes consultar y probar los endpoints disponibles.

---

# 🔐 Autenticación

La API utiliza **JWT Bearer Tokens** para proteger los endpoints que requieren autenticación.

## Login

```http
POST /api/auth/login
Content-Type: application/json
```

Ejemplo:

```json
{
  "email": "admin",
  "password": "admin123"
}
```

La API devuelve un token JWT:

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

### Utilizar el token

En Swagger:

1. Ejecuta `/api/auth/login`.
2. Copia el JWT obtenido.
3. Selecciona **Authorize**.
4. Introduce:

```text
Bearer <TOKEN>
```

Después de autenticarte podrás ejecutar los endpoints protegidos.

---

# 📚 Endpoints

## 1. 👤 Socios

Los socios representan a las personas registradas en el gimnasio.

### Crear socio

```http
POST /api/socios
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

Ejemplo de respuesta:

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

### Listar socios

```http
GET /api/socios
```

### Filtrar socios activos

```http
GET /api/socios?activo=true
```

Los socios utilizan **baja lógica**, por lo que desactivar un socio no elimina su información de la base de datos.

---

# 2. 📋 Planes

Los planes representan las diferentes opciones de membresía disponibles.

### Crear plan

```http
POST /api/planes
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

Ejemplo de respuesta:

```json
{
  "id": 1,
  "nombre": "Mensual",
  "duracion": 30,
  "precio": 200,
  "activo": true,
  "fechaCreacion": "2026-09-10T19:43:47.5994276Z"
}
```

Los planes también utilizan baja lógica mediante la propiedad `activo`.

---

# 3. 💳 Membresías

Las membresías relacionan un socio con un plan y conservan información histórica de la contratación.

### Crear membresía

```http
POST /api/membresias
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

```json
{
  "socioId": 1,
  "planId": 1
}
```

Ejemplo de respuesta:

```json
{
  "id": 1,
  "socioId": 1,
  "nombreSocio": "Marco Hernandez",
  "planId": 1,
  "nombrePlan": "Mensual",
  "fechaInicio": "2026-09-10T19:44:42.5372567Z",
  "fechaVencimiento": "2026-10-10T19:44:42.5372567Z",
  "precioAplicado": 200,
  "estado": "Activa",
  "fechaCreacion": "2026-09-10T19:44:42.5376334Z",
  "diasRestantes": 29
}
```

### Renovar membresía

```http
POST /api/membresias/1/renovar
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

```json
{
  "planId": 2
}
```

La renovación extiende la fecha de vencimiento de la membresía sin perder los días restantes de una membresía vigente.

### Consultar membresía vigente

```http
GET /api/socios/1/membresia-vigente
Authorization: Bearer <TOKEN>
```

### Consultar historial de membresías

```http
GET /api/socios/1/membresias
Authorization: Bearer <TOKEN>
```

El historial permite conservar las diferentes membresías asociadas a un socio.

---

# 4. 🚪 Asistencias

El módulo de asistencias permite registrar las entradas de los socios al gimnasio.

Antes de registrar una entrada se valida que el socio pueda utilizar el servicio de acuerdo con las reglas de negocio implementadas.

### Registrar entrada

```http
POST /api/asistencias
Authorization: Bearer <TOKEN>
Content-Type: application/json
```

```json
{
  "socioId": 1
}
```

Ejemplo de respuesta:

```json
{
  "id": 1,
  "socioId": 1,
  "nombreSocio": "Marco Hernandez",
  "fechaHoraEntrada": "2026-09-10T19:46:58.4682852Z",
  "fechaCreacion": "2026-09-10T19:46:58.4681722Z"
}
```

### Consultar asistencias del día

```http
GET /api/asistencias/dia/hoy
Authorization: Bearer <TOKEN>
```

### Consultar historial de asistencias de un socio

```http
GET /api/asistencias/socio/1
Authorization: Bearer <TOKEN>
```

---

# 5. 📊 Dashboard

El dashboard proporciona información resumida sobre el estado actual del gimnasio.

### Obtener métricas

```http
GET /api/dashboard
Authorization: Bearer <TOKEN>
```

Ejemplo:

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

# 🧠 Conceptos de negocio implementados

## Baja lógica

Los socios y planes no se eliminan físicamente.

En lugar de borrar los registros, se modifica su estado:

```text
Activo = false
```

Esto permite conservar información histórica.

## Precio histórico

Cada membresía almacena el precio utilizado al momento de realizar la contratación mediante:

```text
PrecioAplicado
```

Por lo tanto, si posteriormente cambia el precio de un plan, las membresías anteriores mantienen el precio con el que fueron contratadas.

## Estado de membresía calculado

El estado de una membresía se determina utilizando su fecha de vencimiento.

Conceptualmente:

```text
FechaVencimiento > FechaActual
        ↓
     Activa

FechaVencimiento <= FechaActual
        ↓
     Vencida
```

Esto evita depender de un estado almacenado que podría quedar desactualizado.

## Renovación sin pérdida de días

Cuando una membresía todavía está vigente, la renovación utiliza su fecha de vencimiento actual como punto de partida.

Por ejemplo:

```text
Membresía actual:
10 septiembre → 10 octubre

Renovación:
10 octubre → 9 noviembre
```

De esta manera no se pierden los días restantes de la membresía anterior.

## Validaciones en diferentes niveles

El proyecto utiliza diferentes mecanismos de validación:

### DTOs

Validaciones mediante Data Annotations:

```csharp
[Required]
[Range(...)]
[StringLength(...)]
```

### Controllers

Validaciones relacionadas con las reglas de negocio.

Por ejemplo:

* Verificar que exista un socio.
* Verificar que exista un plan.
* Verificar que un socio pueda registrar asistencia.
* Verificar que una membresía pueda renovarse.

### Base de datos

Restricciones relacionadas con la integridad de los datos:

* Foreign Keys
* Índices
* Restricciones de unicidad
* Relaciones entre entidades

---

# 🗂️ Estructura del proyecto

```text
Gymnasium/
├── Controllers/
│   ├── AuthController.cs
│   ├── SociosController.cs
│   ├── PlanesController.cs
│   ├── MembresiasController.cs
│   ├── AsistenciasController.cs
│   └── DashboardController.cs
│
├── Datos/
│   ├── ApplicationDBContext.cs
│   └── DataSeeder.cs
│
├── DTOs/
│
├── Entidades/
│   ├── Administrador.cs
│   ├── Socio.cs
│   ├── Plan.cs
│   ├── Membresia.cs
│   └── Asistencia.cs
│
├── Services/
│   └── TokenService.cs
│
├── Migrations/
│
├── Frontend/
│   ├── Dashboard/
│   ├── Index/
│   └── Users/
│
├── Program.cs
├── appsettings.json
├── Dockerfile
├── docker-compose.yml
└── Gym.csproj
```

---

# 🧪 Pruebas

Los endpoints pueden probarse utilizando:

* **Swagger UI**
* **Postman**
* **Rider HTTP Client**

Para los endpoints protegidos es necesario proporcionar el JWT obtenido mediante el endpoint de autenticación.

Entre los escenarios contemplados se encuentran:

* Autenticación.
* Autorización.
* Operaciones CRUD.
* Validaciones de negocio.
* Recursos inexistentes (`404`).
* Solicitudes inválidas (`400`).

---

# 📈 Próximas mejoras

* [ ] Pruebas automatizadas con **xUnit**
* [ ] Mejoras adicionales en el sistema de roles y permisos

### Pruebas automatizadas

Las pruebas automatizadas cubrirán principalmente:

* Autenticación y autorización.
* CRUD de los módulos principales.
* Validaciones de negocio.
* Casos de error.
* Comportamiento de membresías.
* Renovaciones.
* Registro de asistencias.

---

# 👨‍💻 Autor

**Abraham Hernandez**

Backend Developer

GitHub:
https://github.com/AbrahamDev950/Gymnasium

---

# 📄 Licencia

Este proyecto se distribuye bajo la licencia **MIT**.

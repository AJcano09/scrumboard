# ideasgroup-reto
---
## Guía de Inicio y Configuración

### Prerrequisitos

Antes de iniciar el proyecto, asegúrese de contar con las siguientes herramientas instaladas:

- Docker Desktop o Docker Engine + Docker Compose.
- .NET SDK 8 o superior si desea ejecutar el stack sin Docker.
- Herramienta CLI de Entity Framework Core si va a correr la API fuera de contenedores:

```bash
dotnet tool install --global dotnet-ef
```

### Ejecución con Docker

1. Copie el archivo de ejemplo de variables de entorno:

```bash
cp .env.example .env
```

El archivo ya trae valores funcionales por defecto para Postgres, la API y el frontend.

2. Levante el stack en modo desarrollo:

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build
```

Esto levanta:
- PostgreSQL en el puerto 5432.
- La API en http://localhost:5001/swagger.
- La SPA en http://localhost:4201.

3. Levante el stack en modo producción:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml up --build
```

Esto levanta:
- PostgreSQL en el puerto 5432.
- La API en http://localhost:5001/swagger.
- La SPA en http://localhost:8080.

### Variables de entorno

El proyecto usa un único archivo .env en la raíz. Allí pueden ajustarse:
- `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`
- `ConnectionStrings__ScrumBoardConnection`
- `Jwt__JwtSettings__Secret`, `Jwt__JwtSettings__Issuer`, `Jwt__JwtSettings__Audience`, `Jwt__JwtSettings__ExpirationMinutes`
- `Security__Pepper`
- `API_PORT`, `WEB_PORT`, `API_URL`

### Usuarios de prueba

Al iniciar la API por primera vez, el seeder crea automáticamente dos usuarios de prueba:

- `admin@scrumboard.com` / `Login.1234`
- `admin2@scrumboard.com` / `Login.1234`

También se crea un proyecto de ejemplo con columnas iniciales para probar el flujo de la aplicación.

### Ejecución sin Docker (alternativa explícita)

Si prefiere correr la API localmente sin contenedores, exporte las variables de entorno manualmente antes de ejecutar:

```bash
export ConnectionStrings__ScrumBoardConnection="Host=localhost;Port=5432;Database=scrumboard_db;Username=postgres;Password=postgres"
export Jwt__JwtSettings__Secret="change-me-super-secret-key-min-32-chars"
export Jwt__JwtSettings__Issuer="ScrumBoardApi"
export Jwt__JwtSettings__Audience="ScrumBoardClient"
export Jwt__JwtSettings__ExpirationMinutes=60
export Security__Pepper="change-me-in-production"

cd backend
dotnet run --project ScrumBoard.Api
```

### Nota sobre la configuración del frontend en Docker

Para la SPA dockerizada, la URL de la API se resuelve en runtime mediante un archivo `assets/env.js` generado en el arranque del contenedor. Esto evita reconstruir la imagen cada vez que cambie la URL de la API entre entornos.

### Migraciones de Base de Datos

Si ejecuta la API fuera de Docker, desde la carpeta `backend` aplique las migraciones con:

```bash
dotnet ef database update --project ScrumBoard.Infrastructure --startup-project ScrumBoard.Api
```
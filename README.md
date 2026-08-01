# ideasgroup-reto
---
## Guía de Inicio y Configuración

### Prerrequisitos

Antes de iniciar el proyecto, asegúrese de contar con las siguientes herramientas instaladas:

- .NET SDK 8 o superior.
- PostgreSQL 15 o superior, ejecutándose localmente o a través de Docker.
- Herramienta CLI de Entity Framework Core:

```bash
dotnet tool install --global dotnet-ef
```

### Configuración del Entorno

1. Clone el repositorio:

```bash
git clone <url-del-repositorio>
cd scrumboard
```

2. Configure la cadena de conexión de PostgreSQL en el archivo de desarrollo de la API:

```text
backend/ScrumBoard.Api/appsettings.Development.json
```

Agregue o actualice la sección `ConnectionStrings` con un valor similar a este:

```json
{
  "ConnectionStrings": {
    "ScrumBoardConnection": "Host=localhost;Port=5432;Database=ScrumBoardDb;Username=postgres;Password=postgres"
  }
}
```

### Migraciones de Base de Datos

Desde la carpeta `backend`, aplique las migraciones con el siguiente comando:

```bash
dotnet ef database update --project ScrumBoard.Infrastructure --startup-project ScrumBoard.Api
```

Este comando usa `ScrumBoard.Infrastructure` como proyecto de destino y `ScrumBoard.Api` como proyecto de inicio.

### Ejecución del Proyecto

Para levantar la API, ejecute:

```bash
cd backend
dotnet run --project ScrumBoard.Api
```

Una vez iniciada, abra la URL mostrada en la salida de la consola y agregue `/swagger` para acceder a la interfaz de Swagger.
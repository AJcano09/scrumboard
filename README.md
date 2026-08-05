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

---

## Testing rápido en Docker

1. Levante el stack con Docker (development):

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml up --build
```

2. Verifique que la API esté arriba abriendo Swagger en:

   http://localhost:5001/swagger

3. Acceda la SPA en:

   http://localhost:4201

4. Inicie sesión con uno de los usuarios sembrados:

   - `admin@scrumboard.com` / `Login.1234`
   - `admin2@scrumboard.com` / `Login.1234`

5. Pruebe el flujo completo:

   - **Drag-and-drop**: arrastre tarjetas entre columnas y observe la actualización en tiempo real.
   - **CRUD**: cree, edite y elimine tarjetas y columnas; verifique que los cambios se persisten y se replican entre pestañas.
   - **Reportes**: abra la sección de reportes y descargue el PDF y el Excel del proyecto de ejemplo.

---

## Decisiones arquitectónicas

- **Arquitectura hexagonal (Clean Architecture):** El código se separa en 4 capas:
  - **Domain**: entidades y puertos (interfaces) del dominio.
  - **Application**: casos de uso y orquestación. No conoce infraestructura.
  - **Infrastructure**: repositorios, servicios externos, SignalR.
  - **Api**: entry points HTTP, controladores y DTOs.
  
  Justificación: permite testear la lógica de negocio de forma aislada y cambiar proveedores (base de datos, colas, etc.) sin tocar el núcleo.

- **CQRS-lite:** Las consultas de lectura usan DTOs optimizados (p. ej. `BoardViewDto`) separados de las entidades de escritura. Así se evita el acoplamiento entre modelo de escritura y modelo de lectura.

- **Repository pattern con interfaces (ports):** Cada repositorio expone una interfaz en el dominio (`IBoardRepository`, `ITaskRepository`, etc.). Las implementaciones concretas viven en Infrastructure. Facilita el mocking en tests.

- **Inversión de dependencias:** La capa Application depende únicamente de Domain y de los puertos definidos. La inyección de dependencias se resuelve en Api/Infrastructure.

---

## Tiempo real — SignalR

- **Alternativas consideradas:** WebSocket puro, Server-Sent Events (SSE), polling periódico.
- **SignalR elegido por:** reconexión automática, pub/sub basado en grupos, integración nativa con JWT.
- **Grupos:** cada board corresponde a un grupo de SignalR nombrado con el `projectId`. Los clientes se unen al grupo al cargar el board.
- **Eventos publicados:** `taskCreated`, `taskUpdated`, `taskDeleted`, `taskMoved`, `columnCreated`, `columnUpdated`, `columnDeleted`, `columnMoved`.

---

## Estrategia de ordenamiento fraccionario

- **Problema:** renumerar todas las tarjetas en cada drag es O(n) y entra en conflicto con el tiempo real (dos usuarios ordenando al mismo tiempo).
- **Solución:** orden fraccionario — la nueva posición es el punto medio entre los vecinos.
- **Gap por defecto:** 1024, lo que permite ~8-9 inserciones antes de necesitar rebalanceo.
- **Sobre el tipo:** se usa `decimal` (no `float` ni `int`) para precisión en el cálculo del punto medio.
- **Rebalanceo:** cuando el gap se agota, habría que redistribuir las posiciones (no implementado aún).
- Verificaciones en `TaskOrderCalculatorTests`.

---

## Patrón de exportación dual

- **Un solo DTO + Strategy pattern:** `ProjectReportDto` alimenta tanto la exportación a PDF como a Excel desde una única consulta a la base de datos.
- **Interfaz:** `IReportExporter` con implementaciones estratégicas.
- **Exportadores:**
  - `PdfReportExporter` — usa QuestPDF.
  - `ExcelReportExporter` — usa ClosedXML.
- **Extensibilidad:** agregar un nuevo formato solo requiere una nueva clase que implemente `IReportExporter`. No se modifica código existente (principio abierto/cerrado).
- **Resolución en runtime:** inyección con `IEnumerable<IReportExporter>` y factory para resolver el exportador por nombre/mIME.

---

## Declaración de uso de IA

- **IA utilizada para:** scaffold del backend, generación y refactor de código, escritura y revisión de casos de prueba, configuración de Docker y archivos `.env`.
- **IA NO utilizada para:** decisiones arquitectónicas (lideradas por el desarrollador) ni código sensible a seguridad (auth, JWT, hashing de contraseñas).
- Todo código generado por IA fue revisado y validado por el desarrollador humano.

---

## Decisiones menores documentadas

- **`PropertyNameCaseInsensitive = true`:** el serializador JSON del backend debe aceptar `camelCase` proveniente del frontend Angular, evitando errores de mapeo en DTOs.
- **Actualizaciones optimistas con rollback:** el drag-and-drop actualiza el DOM inmediatamente y revierte en caso de error del API.
- **`cdkDropListConnectedTo` con `@ViewChildren(CdkDropList)`:** se usa esta query en lugar de IDs string para type safety y evitar errores de referencia cruzada entre listas.
- **`env.js` de runtime para Docker:** la configuración del frontend se resuelve en runtime (no en build-time), permitiendo cambiar la URL de la API sin reconstruir la imagen.
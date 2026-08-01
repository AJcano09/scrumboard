# ideasgroup-reto

# Hoja de Ruta — Plan de Acción y Tareas (desde cero)

> Proceso IDEASGROUP-REM-LAT-26-2907 · Inicio: jue 30 jul 2026 · Entrega: jue 6 ago 2026

---

## Día 1 — Jue 30 jul (Setup completo: backend, frontend, infraestructura)

**Repositorio y control de versiones**
- [x] `git init`, crear repo remoto público en GitHub, primer commit vacío/README inicial.
- [x] `.gitignore` para .NET (`bin/`, `obj/`) y Node/Angular (`node_modules/`, `dist/`).
- [x] Definir convención de commits atómicos (ej. `feat:`, `fix:`, `chore:`) para mantenerlos descriptivos y distribuidos.

**Backend — solución y arquitectura hexagonal**
- [x] `dotnet new sln -n ScrumBoard`.
- [x] Crear proyectos: `ScrumBoard.Domain` (classlib), `ScrumBoard.Application` (classlib), `ScrumBoard.Infrastructure` (classlib), `ScrumBoard.Api` (webapi).
- [x] Referencias: `Application` → `Domain`; `Infrastructure` → `Application` + `Domain`; `Api` → `Application` + `Infrastructure`.
- [x] **Domain**: entidades `User`, `Project`, `Column`, `Task` con los atributos mínimos del PDF (incluir enums `TaskPriority`, `ProjectStatus`). Sin dependencias externas en este proyecto.
- [x] **Application**: puertos (`IUserRepository`, `IProjectRepository`, `IColumnRepository`, `ITaskRepository`, `IPasswordHasher`, `IRealtimeNotifier`, `IReportExporter`), DTOs base, casos de uso esqueleto (carpetas por feature).
- [x] **Infrastructure**: instalar `Npgsql.EntityFrameworkCore.PostgreSQL`, `ScrumBoardDbContext`, configuraciones Fluent API por entidad, implementaciones de los repositorios definidos como puertos.
- [x] Migración inicial: `dotnet ef migrations add InitialCreate` (proyecto Infrastructure, startup Api).
- [x] Seeder con **2 usuarios precargados** (contraseña con hash + salt/pepper) y datos demo mínimos.
- [x] **Api**: `Program.cs` con DI de las 3 capas, Swagger, `AddControllers()`, configuración por variables de entorno (sin cadenas de conexión ni secretos versionados).

**Frontend — Angular 17 + PrimeNG + Sakai**
- [x] `ng new scrumboard-web --routing --style=scss --standalone`.
- [x] Instalar PrimeNG y adaptar la plantilla Sakai (layout, tema, menú).
- [x] Definir estructura por capas: `core/` (auth, interceptors, guards), `features/` (proyectos, tablero), `shared/` (componentes, modelos, servicios comunes).
- [x] Archivos de entorno (`environment.ts` / `environment.prod.ts`) para la URL de la API — nada de URLs embebidas en componentes.

**Infraestructura**
- [x] `docker-compose.yml` esqueleto: servicio Postgres, placeholders para API y SPA (nginx/httpd).
- [x] `.env.example` con variables explícitas y valores por defecto que permitan levantar el proyecto sin configuración manual.
- [x] `README.md` con secciones esqueleto: instrucciones de ejecución, decisiones arquitectónicas, tecnología de tiempo real, estrategia de ordenamiento, patrón de exportación dual, declaración de uso de IA.

**Cierre del día**
- [x] Verificar que `dotnet build` y `ng build` corren sin errores.
- [x] 3–4 commits atómicos mínimo (scaffold backend, scaffold frontend, docker/env, README inicial).

---

## Día 2 — Vie 31 jul (Autenticación)

- [ ] Implementar hash de contraseñas (salt + pepper) en `Infrastructure` detrás del puerto `IPasswordHasher`.
- [ ] Confirmar que el seeder crea los 2 usuarios con contraseña ya hasheada.
- [ ] Emisión y validación de JWT (endpoint `POST /api/auth/login`), configuración de clave/secret vía variable de entorno.
- [ ] Middleware/atributos de autorización en todos los endpoints de negocio.
- [ ] **Frontend:** pantalla de login (formulario + validaciones básicas con PrimeNG).
- [ ] **Frontend:** guard de ruta que bloquea el tablero sin sesión válida.
- [ ] **Frontend:** interceptor HTTP que adjunta el token y gestiona la respuesta 401 (logout + redirección a login).
- [ ] Commits atómicos por hito (backend auth, frontend login, guard/interceptor).

---

## Día 3 — Sáb 1 ago (Proyectos y Columnas)

- [ ] API RESTful completa de **Proyectos**: crear, listar (con paginación y filtro por nombre por coincidencia parcial resuelto en servidor), editar, eliminar.
- [ ] API RESTful completa de **Columnas**: CRUD + orden dentro del proyecto.
- [ ] Regla de negocio en backend: no permitir eliminar una columna que contenga tareas (validación en la capa Application).
- [ ] **Frontend:** listado de proyectos con paginación/filtro, formularios de alta/edición, confirmación de eliminación.
- [ ] **Frontend:** pantalla de administración de columnas por proyecto, incluida reordenación.
- [ ] Commits atómicos (API proyectos, API columnas, UI proyectos, UI columnas).

---

## Día 4 — Dom 2 ago (Tareas y tablero Kanban)

- [ ] API RESTful completa de **Tareas**: CRUD, asignación de responsable y prioridad.
- [ ] **Frontend:** tablero kanban que renderiza columnas y tareas en el orden establecido.
- [ ] Drag & drop entre columnas y dentro de una misma columna (`cdkDragDrop` de Angular CDK, compatible con PrimeNG).
- [ ] Diseñar e implementar el **algoritmo de posición fraccionaria** para el reordenamiento (evita reescribir todas las filas al mover una tarea).
- [ ] Actualización optimista en la UI con reversión visible si el servidor responde con error.
- [ ] Escribir de inmediato el **test obligatorio** del cálculo de nueva posición al reordenar (mientras el código está fresco).
- [ ] Verificar persistencia del orden al recargar y al iniciar sesión desde otro equipo/navegador.
- [ ] Commits atómicos (API tareas, algoritmo de orden + test, UI tablero, drag & drop).

---

## Día 5 — Lun 3 ago (Tiempo real)

- [ ] Configurar Hub de SignalR, autenticado con el mismo JWT de sesión.
- [ ] Propagar alta, edición, eliminación y movimiento/reordenamiento de tareas a las sesiones suscritas al mismo tablero (< 2 segundos).
- [ ] Confirmar que una sesión **no** recibe eventos de tableros a los que no está suscrita.
- [ ] **Frontend:** cliente SignalR, suscripción al tablero activo, actualización de UI en tiempo real.
- [ ] Cierre limpio de conexión y suscripciones en `ngOnDestroy` (sin conexiones huérfanas).
- [ ] Prueba manual con dos sesiones/dos usuarios distintos moviendo tareas simultáneamente.
- [ ] Documentar en README la tecnología elegida y las alternativas descartadas.
- [ ] Commits atómicos (hub backend, cliente frontend, pruebas de sincronización).

---

## Día 6 — Mar 4 ago (Reportes + pruebas)

- [ ] Definir **un único DTO** de reporte (`ProjectReportDto`) y **una sola consulta** a la base de datos que alimente ambos formatos.
- [ ] Implementar `IReportExporter` (puerto) con `PdfReportExporter` (QuestPDF, obligatorio) y `ExcelReportExporter` (librería a elección, declarada en README).
- [ ] PDF: encabezado con datos del proyecto y fecha de generación, tabla de tareas con columna/responsable/prioridad.
- [ ] Excel: mismos datos, encabezados legibles, anchos de columna adecuados.
- [ ] Validar extensibilidad: agregar un tercer formato de prueba no debería exigir tocar las clases exportadoras existentes.
- [ ] **Frontend:** botones de descarga con nombre de archivo y tipo de contenido correctos.
- [ ] Completar el resto de pruebas unitarias hasta llegar a **5 backend + 5 frontend** sobre lógica de dominio/aplicación.
- [ ] Commits atómicos (DTO + consulta, exportador PDF, exportador Excel, UI descargas, tests).

---

## Día 7 — Mié 5 ago (Cierre, validación y opcionales)

- [ ] Probar `docker-compose` completo desde cero: clonar el repo en una carpeta limpia y levantar todo únicamente con las instrucciones del README.
- [ ] Verificar que las migraciones construyen la base de datos desde cero en orden correcto.
- [ ] Generar e incrustar el **diagrama del modelo de base de datos** (imagen, visible directamente en el README) con notación correcta.
- [ ] Completar todas las secciones del README: decisiones arquitectónicas y su justificación, tecnología de tiempo real y alternativas descartadas, estrategia de índices/orden, patrón de exportación dual, declaración de uso de IA (qué herramientas y en qué partes).
- [ ] *(Opcional, solo si el resto ya está sólido)* filtros por responsable/prioridad, indicador de usuarios conectados, búsqueda de tareas por texto. Recuerda: solo suman 5%, no arriesgar lo obligatorio por completarlos.
- [ ] Revisar el historial de commits: deben verse distribuidos a lo largo de los días, no concentrados en uno o dos.
- [ ] Commits atómicos (docker validado, diagrama + README final, opcionales si aplica).

---

## Jue 6 ago — Margen / Entrega

- [ ] *(Opcional)* Grabar video de 5–10 min mostrando el flujo completo (happy path): login, CRUD de proyecto/columnas/tareas, drag & drop, sincronización en tiempo real con dos sesiones, descarga de ambos reportes. Subir a Drive/OneDrive/YouTube privado con enlace público.
- [ ] Revisión final del repositorio en modo "evaluador": clonar en limpio, seguir solo el README, confirmar que todo levanta.
- [ ] Confirmar que no hay secretos ni cadenas de conexión versionadas.
- [ ] Enviar correo con la URL del repositorio (y enlace al video si aplica) al correo donde se recibió el reto, antes de la hora límite. Nada después de la fecha/hora de corte se considera en la calificación.

---

## Notas de seguimiento

- Cualquier decisión no especificada en el documento del reto debe documentarse en el README con su justificación — el evaluador la usará también en la sustentación técnica.
- Prepárate para explicar y proponer alternativas sobre 2–3 fragmentos de código al azar en la entrevista posterior; prioriza código que entiendas a fondo sobre código "que funciona pero no sabrías defender".

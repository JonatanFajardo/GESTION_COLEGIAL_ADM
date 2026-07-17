# TODO — Pendientes por pantalla

Este archivo agrupa tareas que surgen al revisar una pantalla pero pertenecen a **otra pantalla dedicada** (regla §7 de las convenciones del proyecto: no sobrecargar listados con columnas por rol; usar/crear pantalla dedicada). Al llegar a la pantalla destino, atender los ítems y borrarlos de aquí.

---

## Origen: Revisión de `Alumnos/Index` (2026-07-03)

### `Encargados` / `Directorio` (contacto de encargados) — ✅ FUSIONADO (2026-07-03)
Nueva pantalla `DirectorioContacto` con tabs ① Por Alumno · ② Por Encargado. Las viejas quedan como fallback vía permission strings ("Listado de encargados", "Directorio de Alumnos").

Pendientes que quedaron abiertos:
- [ ] **API — endpoint de conteo de alumnos por encargado.** El `EncargadoViewModel` no tiene `Enc_CantidadAlumnos` ni `Enc_AlumnosACargo`. Cuando el API los devuelva: (a) mostrar badge "👥 N alumnos" en las cards de tab ②, (b) agregar columna "Alumno(s) a cargo" en la tabla de tab ②, (c) agregar KPI card "Encargados con múltiples alumnos".
- [ ] **API — paginación server-side en `AlumnosDirectorio/ListAsync`.** Hoy retorna los 836 alumnos completos; tab ① exige seleccionar curso o sección para no golpear la red. Agregar `page` + `pageSize` (o al menos aceptar `Niv_Id` y `Mda_Id` como filtros para reducir el set). También endpoint `AlumnosDirectorio/CountAsync` para KPI "Alumnos en directorio" sin traer la lista completa.
- [ ] Agregar la permission `"Directorio de Contacto"` a la matriz roles/pantallas y migrarla desde "Listado de encargados"/"Directorio de Alumnos".
- [ ] Cuando la nueva pantalla esté validada, eliminar del sidebar y del proyecto los archivos viejos: `Views/Encargados/Index.cshtml`, `Views/AlumnosDirectorio/`, `Content/js/pages/alumnosdirectorio.js` (dejar `Encargados` Detail/Edit/Create/Delete porque el fusionado los reutiliza para CRUD).
- [ ] Desde `Alumnos/Detail`, agregar link directo a "Ver encargado" que abra `DirectorioContacto` con filtro por `Alu_Id`.

### `CuentasCobrar/EstadoCuenta` (estado de cuenta del alumno)
- [ ] Desde `Alumnos/Detail`, agregar link "Ver estado de cuenta" que abra EstadoCuenta con el `Alu_Id` precargado.
- [ ] Validar que muestre badge Al día / Con mora arriba, saldo total, y últimos movimientos.

### `CuadernoNotasAdm` / `BoletinAdm` (rendimiento del alumno)
- [ ] Desde `Alumnos/Detail`, agregar link "Ver boletín" que abra el hub `NotasAdm` (tab Boletín) con `Alu_Id`.
- [x] ~~Revisar duplicación entre CuadernoNotasAdm y BoletinAdm~~ — ✅ FUSIONADOS en hub "Notas y Boletín" (`NotasAdm/Index`, 2026-07-08).

### `PaseDeLista` (asistencia del alumno)
- [ ] Desde `Alumnos/Detail`, agregar link "Ver asistencia" que muestre historial de asistencia del alumno (no del grupo).
- [ ] Confirmar si existe una vista por-alumno o solo por-sección; si no existe, evaluar crearla.

### `Alumnos/Detail` — convertir en ficha 360 real
- [ ] Sección de links salientes con íconos: Encargados, Documentos, Estado de cuenta, Boletín, Asistencia, Historial de matrícula.
- [ ] No mezclar los datos de esas pantallas dentro del Detail; solo enlazar.

---

## Pantallas deshabilitadas — decisión de producto pendiente

### `Mensajes a Encargados` (Controller `Mensajes`) — DESHABILITADA en sidebar (2026-07-03)
Motivo: hoy la pantalla solo hace `INSERT` en `app.tbMensajes` de la API externa. No envía correo, SMS ni notificación; no hay portal de encargados que lea la tabla. El nombre promete comunicación pero nadie lee el mensaje.

Decidir entre:
- **A. SMTP real** — integrar SendGrid/SES/Mailgun; usar `Enc_CorreoElectronico`; badge Enviado/Entregado/Fallido. (Recomendada.)
- **B. Portal del encargado** — login para encargados que lea la tabla. Módulo grande.
- **C. Renombrar a "Notas internas del alumno"** — aceptar el buzón interno como registro no-comunicación.

Al retomar: habilitar el link en `_sidebar.cshtml:241` y aplicar refactor según opción elegida (filtros, destinatario visible, envío masivo por sección/nivel, plantillas, adjuntos).

---

## Refactor sección Académico — hubs consolidados (2026-07-08)

Test de utilidad §7 aplicado a las 15 pantallas de Académico. Resultado: 15 entradas → 5 (Notas y Boletín · Docencia Diaria · Analítica Académica · Graduación · Centro de Anuncios).

### Hubs creados
- **Notas y Boletín** (`NotasAdm/Index`): tabs Cuaderno + Boletín. Boletín ahora renderiza parciales dinámicos (antes 3 hardcoded), decimales consistentes, umbral rojo desde nota mínima configurable.
- **Docencia Diaria** (`AsistenciaAdm/Index`): tabs Pase de Lista + Plan de Clases + Tareas, selector de horario compartido.
- **Analítica Académica** (`KPIsAcademicos/Index`): tabs Resumen + Rendimiento por sección; clasificación de riesgo unificada desde nota mínima (antes 70/60 vs 70/50); tabla de riesgo enlaza a ficha 360.
- **Graduación** (`GraduacionAlumnos/Index`): tabs Candidatos + Directorio (con constancia PDF) + Backfill histórico (dry-run).

### Pantallas deshabilitadas del sidebar (test §7)
- `MisAlumnosAdm` — galería sin acciones ni enlace a ficha; `Alumnos/Index` la supera. Reactivar solo si se le da tarea propia (ej. acceso rápido docente con link a ficha).
- `MapaAulasAdm` — sin capacidad ni sección (`AulaViewModel` no las tiene); nadie consume su data. Reactivar solo integrada a Horarios (choques/capacidad).
- `AlumnosEnRiesgo` — filtros rotos (JS no enviaba parámetros); la tabla de riesgo vive en Analítica Académica con enlace a ficha 360.
- `AlumnosBusqueda` — filtro roto (Termino nunca se enviaba); pendiente: darle a `Alumnos/Index` filtros por estado/nivel/sección (§4 top-3).

### Pendientes del refactor
- [ ] **Matriz roles/pantallas**: unificar permisos por hub. Hoy cada tab conserva su guard viejo — un usuario con "Cuaderno de notas" pero sin "Boletín de calificaciones" ve el tab pero sus AJAX se rechazan (ídem Docencia: "Plan de clases"/"Tareas y entregas"; ídem Graduación: "Directorio de Graduados").
- [ ] **Limpieza de huérfanos** (commit aparte, tocar csproj): `Views/BoletinAdm/`, `Views/PlanClasesAdm/`, `Views/TareasAdm/`, `Views/GraduacionAlumnos/BackfillIndex.cshtml`, `Views/AlumnosGraduados/Index.cshtml`, JS sin uso (`boletinadm.js`, `planclasesadm.js`, `tareasadm.js`, `rendimientoporseccion.js`, `graduacionalumnos.backfill.js`) y las 4 pantallas deshabilitadas completas si se confirma la decisión.
- [ ] **Fase 3 pre-golive — cierre de parcial**: campo `Pac_Cerrado`/fecha en Parciales (API + DB) y bloqueo de edición en el Cuaderno; protege la integridad del boletín.
- [ ] `GraduacionService.cs:19` hardcodea `BaseUrl` — unificar con `SendHttpClient`.

### Backlog post-golive (cortado a propósito)
- Entregas de tareas con calificación por alumno (hoy "Tareas" es solo el catálogo).
- Flujo de aprobación de Plan de Clases por director.
- Acuse de lectura en Centro de Anuncios + destinatarios segmentados.
- Capacidad/sección en aulas + detección de choques (rehabilitar Mapa de Aulas).
- Historial visible de cambios de nota (auditoría) y ponderación de parciales.
- % asistencia agregado por sección + alertas de inasistencia (hoy solo en ficha individual).

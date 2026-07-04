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
- [ ] Desde `Alumnos/Detail`, agregar link "Ver boletín" que abra BoletinAdm con `Alu_Id`.
- [ ] Revisar duplicación entre CuadernoNotasAdm y BoletinAdm (§4 de las convenciones del proyecto); definir cuál es para docente vs. cuál para admin/director.

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

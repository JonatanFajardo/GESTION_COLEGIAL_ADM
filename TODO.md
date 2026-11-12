# TODO — Pendientes por pantalla

Este archivo agrupa tareas que surgen al revisar una pantalla pero pertenecen a **otra pantalla dedicada** (regla §7 de las convenciones del proyecto: no sobrecargar listados con columnas por rol; usar/crear pantalla dedicada). Al llegar a la pantalla destino, atender los ítems y borrarlos de aquí.

---

## Cola de trabajo (2026-11-12)

1. **Inscripción inicial del año** — cierra el hueco #1 (Año Lectivo): inscribir en el año activo a los alumnos que hoy solo existen en `tbAlumnos`, y que la Matrícula nueva también inscriba en el año. La API ya tiene `MatriculaAnio/EnrollAsync`; falta la pantalla en el ADM.
2. **Calendario escolar** — espejo de Skuul (`/dashboard/calendar-events`): eventos por tipo (feriado, cierre, exámenes, reunión de padres, acto, actividad…), para toda la escuela o una sección, en borrador o publicados. Conectarlo con el pase de lista (no pedir asistencia en feriados ni cierres) y con el % de asistencia. Punto de partida: `EventosService` (la API no tiene esa parte) y FullCalendar (incluido, sin uso).
3. Disciplina · 4. Reportes de asistencia de alumnos · 5. Constancias · 6. Escala de notas + cuadro de honor · 7. Portal de padres.

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

### `EstadoCuentaAdm` → consolidar en `CuentasCobrar/EstadoCuenta` (2026-09-19)
Dos "Estado de cuenta" del alumno hacían lo mismo. Se dejó **`CuentasCobrar/EstadoCuenta`** como canónica (accionable: cobra, descuentos, mora, histórico con recibos) y se le **agregó Imprimir/PDF** (membrete + `@media print`) que solo tenía la otra. `EstadoCuentaAdm` se **ocultó solo para cajero** (`!esCajero` en sidebar) y se corrigió su símbolo "Q"→"L".
- [ ] Verificar que director/contador/consulta que usan `EstadoCuentaAdm` tengan acceso a `CuentasCobrar/EstadoCuenta` (permiso "Listado de cuentas por cobrar") antes de ocultarla globalmente.
- [ ] Luego: comentar globalmente el `<li>` de `EstadoCuentaAdm` y retirar `estadocuentaadm.js` / vista.
- [ ] Opcional: cargar nombre real del instituto en el membrete de impresión (hoy es texto fijo "Instituto San Francisco"; `EstadoCuentaAdm` lo traía de `InstitutoConfig/GetAsync`).

### Vistas de deuda → consolidadas en hub `Cartera` (2026-09-19)
El hub `Cartera` (`/Cartera`, permiso "Cartera", Pan 9068) reemplaza las vistas de deuda dispersas con pestañas: **Por Nivel** (drill Curso→Sección→Alumno), **Antigüedad de saldos**, **Top Deudores** y **Pendientes**. Todas se calculan en cliente desde `Cartera/PendientesAsync` (endpoint propio, no depende de permisos de las pantallas viejas). Botones por alumno: 📄 Estado de cuenta y 💵 Registrar pago (precarga por identidad). Además "Solicitar cobro" = correo simulado a encargados (no hay SMTP; ver [[feedback]]).

Estado del retiro:
- [x] Permiso "Cartera" concedido al rol **cajero** (1005).
- [x] **Solo para cajero** (reversible, `bool esCajero` en `_sidebar.cshtml`): ocultos **Deudores** y **Cargos pendientes** del bloque Cuentas por Cobrar. Las otras 5 (Aging, Top, Morosidad, Morosos, Pendientes por Cobrar) el cajero **no las tenía concedidas**, así que ya no le aparecían.
- [ ] **Conceder "Cartera" a contador (1006) y director (1002)** antes de retirar globalmente sus vistas de deuda (hoy las usan y no tienen el reemplazo).
- [ ] Una vez todos los roles tengan Cartera: comentar globalmente los `<li>` de Deudores, Cargos pendientes, Pendientes por Cobrar, Aging de Cuentas, Top Alumnos con Deuda, Morosidad por Nivel y Morosos; dejar controllers vivos (endpoints reutilizados).
- [ ] Revisar **"Intereses por mora"** (`CuentasCobrar/Moratorias`): si solo lista, fusionar en Cartera; si calcula/aplica mora, mantener.
- [ ] Drill-down: el tope es Curso (`Cur_Nombre`), no Nivel; y falta % morosidad por sección (requiere total de matriculados, dato de API).

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

---

## Refactor catálogos — hubs consolidados de Configuración (2026-09-12)

Test de utilidad §7 aplicado a las ~18 pantallas sueltas de catálogos. Todas comparten rol dueño (Admin académico / de sistema) y tarea (mantener un catálogo que rara vez cambia); todas **duplican** un hub → veredicto **Fusionar → deshabilitar link suelto**.

### Hubs y qué consolida cada uno
- **Estructura Académica** (`CatalogosInstitucion`): Modalidades · Niveles Educativos · Secciones · Semestres · Parciales.
- **Programa Académico** (`ProgramaAcademico`): Cursos · Grados Académicos (CursosNiveles) · Materias · Duraciones.
- **Recursos de Horario** (`RecursosHorario`): Aulas · Días · Horas.
- **Config. de Cobranza** (`ConfiguracionCobranza`): Conceptos de Pago · Tarifas · Descuentos · Formas de Pago · Estados de Pago.
- **Catálogos Comunes** (`CatalogosComunes`): Estados · Parentescos · Títulos · Cargos.

### Links sueltos deshabilitados en `_sidebar.cshtml` (comentados con Razor `@* *@`, 2026-09-12)
- Menú "Cursos" completo (Cursos · Grados Académicos · Duraciones · Materias) → Programa Académico.
- Menú "Configuración → Legacy · pantallas sueltas" (Modalidades · Niveles · Secciones · Semestres · Parciales) → Estructura Académica.
- Horarios: Horas · Días · Aulas → Recursos de Horario (se conservan Crear Horario Semanal, Horario Semanal Nuevo y Visor: NO son catálogos, siguen su propia duplicación §3/§4).
- Financiero → Configuración (Conceptos · Formas de Pago · Descuentos · Estados de Pago · Tarifas) → Config. de Cobranza.
- Alumnos → Parentescos y RRHH → Cargos → Catálogos Comunes.

### IMPORTANTE — no borrar los controllers legacy
Los 5 hubs **reutilizan los endpoints de los controllers originales** (`/Modalidades/ListAsync`, `/Cargos/CreateAsync`, `/Tarifas/DeleteAsync`, etc.). Borrar los controllers rompe los hubs. Los links del sidebar están solo **comentados** — reversibles quitando el `@* *@`.

### Pendientes de este refactor
- [ ] **Validar en navegador** que cada hub hace CRUD real de todos sus catálogos (crear/editar/eliminar/detalle) antes de considerar el borrado definitivo.
- [ ] **Matriz roles/pantallas**: dar de alta las permission strings de los 5 hubs ("Estructura Académica", "Programa Académico", "Recursos de Horario", "Configuración de Cobranza", "Catálogos Comunes") y migrar los permisos de los catálogos sueltos hacia ellas (hoy los hubs se ven vía `esAdmin`; los CRUD internos aún dependen del guard viejo de cada controller).
- [ ] **Limpieza post-validación** (commit aparte, tocar csproj): eliminar las vistas `Index.cshtml` sueltas de los catálogos ya consolidados (Modalidades, NivelesEducativos, Secciones, Semestres, Parciales, Cursos, CursosNiveles, Duraciones, Materias, Aulas, Dias, Horas, ConceptosPago, FormasPago, Descuentos, EstadosPago, Tarifas, Estados, Parentescos, Titulos, Cargos) — **dejando** cada controller y sus vistas Create/Edit/Delete/Detail que el hub reutiliza. Recién ahí quitar los bloques comentados del sidebar.

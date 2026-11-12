# TODO — Pendientes por pantalla

Este archivo agrupa tareas que surgen al revisar una pantalla pero pertenecen a **otra pantalla dedicada** (regla §7 de las convenciones del proyecto: no sobrecargar listados con columnas por rol; usar/crear pantalla dedicada). Al llegar a la pantalla destino, atender los ítems y borrarlos de aquí.

---

## Paridad con la app guía (Skuul) — qué falta copiar (2026-11-12)

Regla: si un módulo existe en Skuul, se copia con su misma profundidad, sus mismas pantallas y su mismo aspecto (no una versión mínima). Si Skuul lo resuelve en varias pantallas, aquí también.

### Ya copiado ✅
- Año Lectivo + asistente de 5 pasos (fechas y períodos · enfoque de enseñanza · clases y profesores · asignaturas · revisar y publicar).
- Períodos del año, enfoque de enseñanza, secciones y ofertas de materia por año.
- Rollover (copiar la estructura al año nuevo) · Inscripción del año y promociones · Expediente por año.
- Calendario escolar (mes, editor del día, para quién, feriados de Honduras) · Pase de lista conectado al calendario.
- Graduación · Centro de anuncios · Importador.

### Falta copiar ❌ (en este orden)
| # | Módulo de Skuul | Qué resuelve | Rol dueño | Esfuerzo | Estado |
|---|---|---|---|---|---|
| 1 | **Disciplina** (casos / incidentes) | Registrar qué pasó, quiénes participaron, qué hizo el colegio y cómo avanzó el caso | Dirección | medio | ✅ hecho (2026-11-12): *Académico › Disciplina*, script 062 de la API; revisado en navegador |
| 2 | **Reporte de asistencia de alumnos** | % por sección y por alumno descontando feriados; alertas de inasistencia | Dirección, docente | poco | pendiente |
| 3 | **Escala de notas + cuadro de honor** | Notas en letra/concepto y ranking de los mejores | Dirección | poco | pendiente |
| 4 | **Constancias y expediente** (boletín y transcript congelados) | Imprimir el historial oficial de varios años | Secretaría | medio | pendiente |
| 5 | **Portal de padres** | El encargado ve notas, cuenta y calendario y hace solicitudes | Encargado | mucho | pendiente |
| 6 | Notas ponderadas (categorías y plantillas de evaluación) | Hoy los parciales no tienen peso | Docente | medio | pendiente |
| 7 | Sustitución de docente | Cubrir la clase de un profesor ausente | Dirección | medio | pendiente |
| 8 | Plan de apoyo al alumno | Seguimiento a un alumno en riesgo, con acciones y notas | Dirección | medio | pendiente |
| 9 | Ficha de salud | Alergias, condición médica, contacto de emergencia | Secretaría | poco | pendiente |
| 10 | Lista de espera de admisión | Aspirantes antes de matricular | Secretaría | poco | pendiente |

No aplican: biblioteca, internado/dormitorios, varias sedes, presupuesto contable (el ADM ya va adelante en finanzas y nómina).

### 1 · Disciplina — cómo lo tiene Skuul (se copia igual, en 3 pantallas)
- **Casos** (listado): filtros por estado y tipo, "solo los que necesitan trabajo", contador de abiertos; tabla Caso (resumen + referencia + n.º de personas) · Tipo (con candado si es reservado) · Estado · Ocurrió · Lo atiende · Abrir.
- **Registrar un caso**: resumen, tipo (Conducta / Protección del menor = reservado), cuándo (no futuro), dónde, quién lo atiende, qué pasó; filas de personas (alumno + papel + nota, hasta 20). Papeles: implicado, afectado, testigo, quien reporta, quien atiende.
- **Ficha del caso**: referencia `CASO-AA-XXXXXX`, datos clave (estado, cuándo/dónde, quién lo atiende, pendientes), mover de estado con motivo, personas, acciones (tipo, qué hay que hacer, responsable, fecha, marcar hecha), notas que no se editan (privadas o no) e historial de estados.
- Estados: Reportado → En revisión → Derivado → Con medidas → Resuelto → Cerrado (con transiciones permitidas; cerrado = solo lectura; resuelto se puede reabrir).
- Un caso de protección del menor solo lo ven quien tiene el permiso de casos reservados, quien lo reportó y quien lo atiende.
- Hecho así en el ADM: `DisciplinaController` (Index · Registrar · Caso), tablas `app.tbCasosDisciplina*`, API `CasosDisciplina/*`. Permisos: "Casos de disciplina", "Registrar caso de disciplina", "Atender casos de disciplina", "Ver casos reservados" (admin y dirección, como Skuul).
- Pendiente: decidir si docentes pueden **registrar** casos (en Skuul no; en un colegio suele hacerlo el docente que ve la falta). Mostrar los casos de un alumno en su ficha 360 (enlace, no copia). Dato con codificación rota en `tbPersonas` ("Ã“scar").

## Cola de trabajo (2026-11-12)

1. ✅ **Inscripción inicial del año** (hecho 2026-11-12) — pestaña "Inscripción del año" en *Inscripción y promociones* (crea las secciones del año que faltan e inscribe en lote; los inscritos que figuraban como Graduado pasan a Activo) y la Matrícula inscribe sola al alumno nuevo en el año activo.
2. ✅ **Calendario escolar** (hecho 2026-11-12) — *Académico › Calendario escolar*, espejo de Skuul: vista de mes + "Qué hay este mes", 9 tipos de día, todo el día o con hora, borrador/publicado, "para quién" (secciones del año, roles, personas), carga de feriados de Honduras (Semana Santa calculada; Morazánico y asueto quedan en borrador para confirmar fecha). Un feriado o cierre publicado bloquea el pase de lista de esa sección (aviso en pantalla + rechazo en el servidor). Tarjeta "Próximos 7 días" en los 8 dashboards. Permisos: "Calendario escolar" (ver, todos) y "Editar calendario escolar" (admin, dirección, secretaría). API: `EventosCalendario/*`, script 060. Se borró `EventosService` (llamaba a una API que no existía).
   - Queda para el **portal de padres (7)**: la API ya tiene `EventosCalendario/ParaAlumnoAsync` (todo el colegio + su sección del año + él con nombre); falta la pantalla y, en las citas, el botón "Solicitar este horario" como en Skuul.
   - Queda para los **reportes de asistencia (4)**: el % de asistencia debe contar solo días con clase (descontar feriados y cierres publicados de la sección).
   - Las "plantillas de calendario" de Skuul son la estructura del año para varias sedes (períodos, apertura automática): en el ADM eso lo cubre Año Lectivo; no aplica.
   - `wwwroot/Content/plugins/fullcalendar` quedó sin uso (el calendario usa su propia cuadrícula, como Skuul): se puede borrar.
   - De paso se arregló el **pase de lista**, que nunca había funcionado: la asistencia apuntaba a la tabla de horarios vieja (`tbHorario`, 1 fila) y no se podía guardar; la lista solo traía asistencias ya guardadas (nunca alumnos); el estado se guardaba como letra pero se mostraba siempre "Presente"; la API respondía "error" aunque guardara. Script 061 de la API.
   - Decisión tomada: quien no edita el calendario ve lo de todo el colegio y lo que es para él (su rol, su nombre, sus secciones), igual en el mes, el detalle y la tarjeta de dashboards. Quien edita ve todo. Feriado y cierre: siempre de día completo y solo para todo el colegio o secciones.

## Hallazgos generales (fuera del calendario, 2026-11-12)

- **API sin autenticación:** ningún controlador de la API exige login y docker-compose publica el puerto 5076. Cualquiera que llegue a ese puerto puede leer o cambiar datos sin pasar por el ADM. Antes del go-live: exigir token o no publicar el puerto fuera del servidor.
- **Modo oscuro:** el menú lateral y la barra de migas siguen claros; en *Docencia diaria* la etiqueta "Horario" desaparece; `text-muted` tiene poco contraste en toda la app.
- **Pase de lista para dirección:** el selector de horarios muestra solo los del empleado ligado al usuario; un director (o el admin "jona", ligado a un empleado sin horario) no ve ninguno. Decidir si dirección debe ver todos.
- **"Sidebar compacto" en escritorio:** la preferencia está rota desde antes (el sidebar tapa el contenido). Arreglarla exige mostrar la hamburguesa también en escritorio.
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

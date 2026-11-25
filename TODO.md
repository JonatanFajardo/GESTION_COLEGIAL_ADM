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
| 2 | **Reporte de asistencia de alumnos** | % por sección y por alumno descontando feriados; alertas de inasistencia | Dirección, docente | poco | ✅ hecho (2026-11-12): *Académico › Reporte de asistencia* (resumen por sección + planilla alumno × día), script 063 de la API. Skuul no lo tiene como pantalla; se tomó de RosarioSIS (Average Daily Attendance + Attendance Chart) |
| 3 | **Escala de notas + cuadro de honor** | Notas en letra/concepto y ranking de los mejores | Dirección | poco | ✅ hecho (rama `feature/escala-notas`, sobre cork; revisado en navegador). **Escala de notas** (*Académico › Escala de notas*): rangos concepto-por-promedio contiguos con alta/edición/baja, aviso de cobertura 0-100, validación de solape; umbrales del cuadro de honor; script 065 con `FN_EscalaConcepto`. **Cuadro de honor** (*Académico › Cuadro de honor*): ranking por año con filtros nivel/sección, dos niveles (Honor ≥umbral / Honor Superior), medallas, imprimir lista e imprimir diplomas (ventana nueva, uno por alumno); SP `PR_CuadroHonor` (script 066). Umbrales demo en la DB = 85/87 (los datos promedian 80); ajustables en Escala de notas. **Concepto en el boletín** ✅ (columna "Concepto" en *Notas y boletín › Boletín*, mapea el promedio por materia con la escala). De paso se **arregló el boletín, que no mostraba notas** (el SP `PR_Boletin_Generate` devuelve 3 result sets y el repo leía solo el primero = datos del alumno; nuevo SP `PR_Boletin_Notas`, script 067). El boletín muestra una columna por semestre × parcial (selector de semestre, marca "Provisional" por período; script 088) |
| 4 | **Constancias y expediente** (boletín y transcript congelados) | Imprimir el historial oficial de varios años | Secretaría | medio | ✅ hecho (2026-11-12): *Académico › Constancias y expediente*, script 064 de la API. Listado con filtros (tipo, año, alumno/referencia) · Emitir (tipo → alumno → año/parcial → vista previa → emitir) · documento imprimible tamaño carta con historial de revisiones; constancia de estudio, boletín (semestre × parcial) y expediente (todos los años). Enlace "Constancias" en la ficha del alumno |
| 5 | **Portal de padres** | El encargado ve notas, cuenta y calendario y hace solicitudes | Encargado | mucho | ✅ hecho (2026-11-23, rama `feature/portal-padres`): **Portal** en `/portal` con diseño propio para el celular (sin el menú administrativo): Inicio con una tarjeta por hijo (promedio publicado, faltas del año, saldo pendiente con aviso de vencido, solicitudes abiertas), Notas (solo parciales **publicados** en *Cierre de parciales*, con el concepto de la escala), Asistencia (indicadores y faltas/tardanzas con botón "Justificar"), Estado de cuenta (cargos del año, pagado, pendiente, vencido), Calendario por mes (`ParaAlumno`), Anuncios vigentes y Solicitudes (justificar una falta, cita, constancia u otro; ver estado y respuesta; retirar mientras esté abierta; no se repite una igual abierta). Con varios hijos se cambia de hijo con un botón. **Invitaciones al portal** (*Alumnos › Invitaciones al portal*): indicadores, búsqueda, enlace de un solo uso que vence a las 72 h (se guarda solo su huella), copiar enlace, detener invitación, suspender/reactivar la cuenta; con cuenta ya creada el enlace sirve para poner contraseña nueva. **Activación** pública en `/invitacion/{código}`: el encargado elige usuario y contraseña (mismo hash que el login; mínimo 8 caracteres con letras y números) y entra directo a su portal. **Solicitudes del portal** (*Alumnos › Solicitudes del portal*): indicadores, filtros por estado y tipo, búsqueda, atender (en proceso → atendida / rechazada con respuesta obligatoria) e historial; la solicitud no cambia registros por sí sola (como en Skuul). Seguridad: rol **Encargado** (1008) con la pantalla "Portal de padres"; vínculo usuario ↔ encargado en `seguridad.tbUsuariosEncargados`; la API saca al encargado del usuario firmado y toda lectura se limita a sus hijos (otro alumno por URL o por la API = vacío); un Encargado no puede abrir pantallas administrativas (filtro global en el ADM, 403 en llamadas AJAX). Permisos: "Invitaciones al portal" (admin, secretaria), "Solicitudes del portal" (admin, secretaria, director). Script 082 de la API (`tbInvitacionesPortal`, `tbSolicitudesPortal` + historial). Pendientes: no hay envío de correo ni WhatsApp (el enlace se copia); los anuncios no tienen público destino (el portal muestra todos los vigentes); las fechas que guarda la base están en hora UTC del contenedor (se ven con +6 h); preferencias de avisos, documentos descargables (boletín/constancias) y horario del alumno de Skuul quedan para después; `tbEncargados_tbAlumnos` casi vacía: sin ese vínculo el encargado no tiene hijos que ver |
| 6 | **Notas ponderadas** (categorías y plantillas de evaluación) | Hoy los parciales no tienen peso | Docente, Dirección | medio | ✅ hecho (2026-11-24, rama `feature/notas-ponderadas`): **Plantillas de evaluación** (*Académico › Plantillas de evaluación*, Dirección): por año, tipos de evaluación con peso en % que suman 100 y cómo se juntan las actividades de cada tipo (suma de puntos como RosarioSIS; promedio o la mejor como Skuul); alcance por nivel y excepción por materia, se aplica la más específica (materia > nivel > por defecto del año); tabla "qué plantilla se aplica" por nivel, copiar, quitar solo si no se usa; un cambio de pesos se aplica a los parciales abiertos (y recalcula sus notas) y los cerrados conservan los suyos. Pestaña **Uso en los cuadernos**: por materia y sección del parcial, actividades, % calificado y notas al día, Excel/PDF y "Abrir" el cuaderno; las notas faltantes siguen en Cierre de parciales. **Cuaderno de notas** (*Notas y boletín*): sin actividades sigue la nota directa; con la primera actividad (nombre, tipo, puntos máximos, fecha, descripción, "no cuenta" y opción de crearla en otras secciones de la misma materia y grado) la nota sale de una planilla alumno × actividad (puntos, E = excusado, NE = no entregó = 0, vacío = sin calificar y no cuenta), con la nota calculada en vivo; si un tipo no tiene nada calificado para un alumno su peso se reparte entre los demás (como RosarioSIS); la nota se redondea a 2 decimales con ,5 hacia arriba y el entero sube desde ,50 (igual en pantalla y en la base); al guardar puntos, actividades o pesos la nota entera se escribe en las notas del parcial en la misma transacción (boletín, constancias y cierre sin cambios) y la nota directa queda bloqueada en ese parcial (crear y editar). Plantilla por defecto 2026: «Plantilla general 2026» (acumulativo 70 % + examen 30 %, suma de puntos). Control de versión por celda y por actividad (dos pestañas no se pisan; los guardados y el recálculo por cambio de pesos van en fila con un candado), aviso de cambios sin guardar, llenar vacíos con NE o el máximo, enlace directo desde Uso; parcial cerrado = solo lectura. El docente solo trabaja en los grupos de su horario. Script 089 de la API (prueba con ROLLBACK en `Scripts/pruebas/089_NotasPonderadas_prueba.sql`). Permisos: "Plantillas de evaluación" (admin, director); el docente usa "Cuaderno de notas". Pendientes: comentario por calificación y puntos extra (RosarioSIS/Skuul); un parcial que ya tiene actividades no se pasa a una plantilla creada después (solo los parciales nuevos); un alumno sin nada calificado conserva la nota que ya tenía; la nota del parcial se guarda entera (la columna es entera) |
| 7 | Sustitución de docente | Cubrir la clase de un profesor ausente | Dirección | medio | ✅ hecho (2026-11-23, rama `feature/sustitucion-docente`): *Académico › Sustituciones*, script 073 de la API (`app.tbSustituciones`, `app.tbAusenciasDocentes`). **Día a día**: selector de fecha (hoy, anterior/siguiente), indicadores (docentes ausentes, clases sin cubrir, cubiertas, horas cubiertas en el mes), una tarjeta por docente ausente con el origen (vacaciones / permiso / incapacidad aprobados, asistencia "Ausente"/"Permiso", o marcada a mano) y sus clases del día según día de la semana, año y semestre vigentes; "Asignar suplente" sugiere solo docentes libres y presentes (primero quien sabe la materia, luego mismo grado/grupo, luego menos sustituciones en el mes, con la razón); cancelar; marcar y quitar ausencias. Reglas en el servidor como Skuul: suplente sin clase ni cobertura que se cruce, no ausente, una sustitución activa por clase y día (índice único + bloqueo por clase y suplente), sin suplente en feriados/cierres, de 7 días atrás a 90 adelante; una clase pasada no se cancela (queda para el pago). Aviso al suplente en notificaciones. **Historial**: rango, suplente, incluir canceladas, horas por suplente, Excel. **Docente**: solo lo suyo (clases que cubre, próximas y sus ausencias), sin acciones. Permisos: "Sustituciones de docentes" (admin, director, secretaría, RRHH, docente), "Ver todas las sustituciones" (admin, director, secretaría, RRHH), "Asignar sustituciones" (admin, director). El suplente ya pasa lista de la clase cubierta ese día (script 080); no hay clave de semestre activo (se deduce de la fecha y del horario cargado) |
| 8 | **Plan de apoyo al alumno** | Seguimiento a un alumno en riesgo, con acciones y notas | Dirección | medio | ✅ hecho (2026-11-21, rama `feature/plan-apoyo`): *Académico › Planes de apoyo*. **Directorio** con indicadores (activos, revisión vencida, pasos atrasados, completados del año), filtros estado / categoría / Nivel → Grado → Sección / búsqueda / "solo revisión vencida" / "solo donde soy responsable", tabla GcTabla y Excel · **Nuevo plan** en pantalla completa (buscador de alumnos activos, aviso si ya tiene un plan abierto, validación en vivo, acepta `?aluId=` y `?categoria=`, guardar como borrador o activar) · **El plan**: datos editables con control de versión, transiciones de Skuul (borrador → activo/cancelado; activo → en pausa/completado/cancelado; en pausa → activo/completado/cancelado; completado → reabrir) con motivo obligatorio al pausar, cancelar y reabrir, pasos con responsable y fecha límite (atrasados resaltados), notas de seguimiento, historial y hoja carta para imprimir. Categorías: refuerzo académico, orientación, conducta, salud, adecuación curricular (salud y orientación se marcan confidenciales). Responsables = usuarios (admin, director, Docente); aviso en Notificaciones al asignar y al cerrar. **Analítica académica**: botones "Crear plan" / "Ver plan" en la tabla de riesgo; **Disciplina**: enlace "Abrir plan de apoyo" por alumno del caso. Script 072 de la API (prefijo `Apo_`; `Pla_` ya era de `tbPlanClases`). Permisos: "Planes de apoyo" (admin, director, Docente: el docente solo ve los planes donde es responsable o tiene un paso, marca sus pasos y escribe notas), "Gestionar planes de apoyo" y "Ver planes confidenciales" (admin, director). De paso se arregló la tabla de riesgo de Analítica, que siempre salía vacía (mandaba el mínimo en NULL al procedimiento). Pendientes: la tabla de riesgo solo usa el promedio anual (no faltas, conducta ni salud); en la DB de prueba ningún alumno tenía promedio bajo 60 y se bajaron 10 para probar; un plan confidencial no admite pasos de docentes que no lo pueden ver (por diseño); los usuarios demo comparten Emp_Id 1 y se muestran con su usuario entre paréntesis |
| 9 | Ficha de salud | Alergias, condición médica, contacto de emergencia | Secretaría | poco | ✅ hecho (2026-11-18, rama `feature/ficha-salud`): *Alumnos › Fichas de salud* (directorio con contadores con/sin ficha, alerta crítica y sin teléfono de emergencia; filtros Nivel → Grado → Sección, búsqueda, "solo sin ficha / con alerta / sin teléfono", exportar a Excel) · ficha del alumno con bloque "En caso de emergencia", formulario por secciones con validación en vivo, historial de cambios (solo nombres de campos, sin valores), control de versión y hoja carta para imprimir · pestaña "Salud" en la ficha 360 · ícono con la alerta en el pase de lista. Script 070 de la API. Permisos: "Fichas de salud" (admin, director, secretaria) y "Editar ficha de salud" (admin, secretaria). Pendientes: `tbEncargados_tbAlumnos` está vacía y ningún flujo la llena (y el catálogo de parentescos trae "Primer/Segundo Grado"), así que "a quién llamar" hoy solo muestra el contacto adicional de la ficha; los usuarios demo comparten Emp_Id 1 y el historial muestra el mismo nombre para todos |
| 10 | **Lista de espera de admisión** | Aspirantes antes de matricular | Secretaría | poco | ✅ hecho (rama `feature/lista-espera`, revisado en navegador). *Alumnos › Lista de espera*: selector de año, indicadores (en espera, ofertas abiertas, matriculados desde la lista, cupos libres), resumen de cupo y demanda por grado para Dirección y la cola por grado (prioridad y orden de llegada) con filtros Nivel → Grado → Modalidad, estado, búsqueda y Excel. **Registro de aspirante** en pantalla completa (validación en vivo, aviso de duplicado y de identidad ya matriculada, muestra su lugar en la cola) y **detalle** con línea de tiempo. Ofrecer cupo solo al siguiente y con cupo libre (una oferta reserva el asiento; sin cupo definido se ofrece con advertencia), rechazar / desistió / reactivar con motivo. **Matricular** abre el wizard de Matrícula con los datos precargados y al guardar marca al aspirante como matriculado. Permisos "Lista de espera" (admin, dirección, secretaría) y "Gestionar lista de espera" (admin, secretaría). Script 071. De paso se arregló la cascada de grados/modalidades/cursos/secciones del wizard y del alta de alumnos (pedía `?id=` dos veces y la API respondía 400). Pendiente: definir `Sea_Cupo` en las secciones del año (hoy todos los grados salen "sin definir"); el alta de alumno rechaza identidades que empiezan con 0 y exige segundo nombre aunque el wizard lo muestra opcional; el vínculo encargado-alumno no se guarda al matricular (la tabla de vínculos está vacía); matricular solo en el año activo; el catálogo de parentescos solo tiene "Primer/Segundo Grado" |
| 11 | **Temario y avance del programa** | El docente arma el programa de su materia por semana y marca qué tema dio en cada sección; Dirección ve qué secciones van atrasadas | Docente, Dirección | medio | ✅ hecho (2026-11-23, rama `feature/temario`): *Académico › Temario y avance*, script 074 (`tbTemarios`, `tbTemarioTemas`, `tbTemarioAvance`). **Biblioteca** con filtros Año, Semestre, Nivel → Grado → Curso → Materia, indicadores y "Nuevo temario" / "Copiar temario" (a otro semestre o año). **Temario**: temas por semana (agregar, editar, subir/bajar, quitar, traer temas de otro temario), imprimir en hoja carta y matriz temas × secciones (dado / en parte / omitido, fecha y nota de clase) con semáforo al día / leve / atrasada contra la semana esperada (desde el inicio de clases del temario). **Avance por sección** (Dirección): todas las secciones, la más atrasada primero, y las materias del horario sin temario con "Crear temario". Permisos: *Temario y avance* (admin, director, Docente), *Editar temarios* (admin, director, Docente: solo sus materias), *Supervisar temarios* (admin, director); alcance del docente validado en los procedimientos con `tbHorarios`. Plan de clases ofrece los temas del temario de la semana. Pendiente: flujo borrador → revisión → publicado de Skuul, notas de clase semanales revisadas por coordinación, períodos de semestre en Año Lectivo (hoy la fecha de inicio vive en el temario), faltan las materias "Ciencias Naturales" y "Estudios Sociales" en el catálogo (los temarios de prueba usan Biología I y Sociología) |
| 12 | **Cierre y publicación de parciales** | Que nadie cambie notas de un parcial cerrado y que el boletín distinga lo provisional de lo publicado | Dirección | medio | ✅ hecho (2026-11-23, rama `feature/cierre-parciales`, revisado en navegador): *Académico › Cierre de parciales*. Selector de año, indicadores (notas cargadas, notas por exigir en parciales abiertos, parciales cerrados, resultados publicados), una tarjeta por semestre × parcial con avance (notas esperadas = alumnos activos de cada sección × materias de su horario del año; si la sección no tiene horario en ese semestre se usan las del año), estado Abierto / Cerrado / Publicado, fechas de carga opcionales (inicio y límite, con aviso de vencida) y acciones. **Ver faltantes**: tabla por sección, materia y docente con cuántas faltan, agrupable por docente y exportable a Excel/PDF. **Cerrar** (si faltan notas pide confirmación explícita y motivo; guarda la foto del avance), **publicar** (solo cerrado), **retirar publicación** y **reabrir** (motivo obligatorio; quita la publicación) con historial. Un parcial cerrado es solo lectura en todos los caminos: `PR_tbNotas_Insert`, `PR_tbNotas_Update` y `PR_tbNotas_Delete` (misma firma) rechazan con un mensaje claro; la API y el ADM lo revisan antes; el cuaderno muestra el aviso "Solo lectura" y bloquea guardar. **Boletín** (*Notas y boletín › Boletín* y `/BoletinAdm`): aviso "Resultados provisionales" con los parciales no publicados y marca en su columna. Script 075 (`tbCierresParcial`, `tbCierresParcialHistorial`; sin fila = abierto). Permisos: "Cierre de parciales" (admin, director, secretaria: ver estado, faltantes e historial) y "Cerrar y publicar parciales" (admin, director). De paso se arregló `Notas/EditAsync` de la API, que mandaba parámetros que el procedimiento ya no tenía y siempre fallaba. Pendientes: el cuaderno de notas sigue sin listar alumnos ni crear notas nuevas (lee `tbHorario`, que tiene 1 fila, y `Notas/CreateAsync` manda parámetros de otra versión del procedimiento): queda para el rehecho del cuaderno; el boletín junta los dos semestres por parcial, así que la marca "Provisional" de una columna cubre ambos; la ficha del alumno (pestaña Académico) y las constancias no avisan si un parcial no está publicado; no hay horarios del segundo semestre (se usan los del año); años anteriores se calculan con la ubicación actual de los alumnos; `spGraduarAlumno` y las herramientas de datos de prueba no revisan el cierre (a propósito) |
| 13 | **Cierre del año escolar** (resultados finales, promociones, archivado) | Cerrar el año con resultados finales, decidir quién pasa de grado, repite o se gradúa, y dejar el año en solo lectura | Dirección | mucho | ✅ hecho (2026-11-25, rama `feature/cierre-anio`): *Académico › Cierre del año* (y botón de bandera en *Años lectivos*). Asistente de 4 pasos con el ciclo de Skuul (Abierto → Cerrando → Cerrado → Archivado): **1 Revisión** con semáforo (parciales sin cerrar o sin publicar, notas faltantes, secciones sin docente guía, alumnos sin inscripción del año o sin sección, año siguiente) y enlaces a la pantalla que lo resuelve; "Iniciar el cierre" pasa el año a Cerrando. **2 Resultados finales**: por alumno y materia, promedio de los parciales publicados y nota final redondeada al entero; reglas por año y nivel (nota mínima, valor inicial 70 tomado de la Escala de notas; hasta 2 materias en recuperación); registro de notas de recuperación (si aprueba, su nota final es la de la recuperación); filtros Nivel → Grado → Sección. **3 Decisiones de promoción**: propuesta automática (promovido / repite / se gradúa en el último grado del nivel / pendiente de recuperación) que Dirección cambia con motivo; al confirmar una sección la inscripción del año queda Promovido / Repite / Graduado, promovidos y repitentes quedan inscritos en el año siguiente (sección equivalente o sin sección) con el mismo procedimiento de *Inscripción y promociones*, y los que se gradúan aparecen listos en *Graduación*; se puede deshacer con motivo; los resultados de una sección confirmada quedan congelados (foto con nombre, sección y materia). **4 Cerrar el año**: si algo bloquea solo se cierra forzando con motivo; la revisión queda guardada en el historial; opcional pasar el año de trabajo al siguiente (con la activación normal de *Años lectivos*). Año cerrado = solo lectura en el servidor: notas y actividades del cuaderno, acciones de Cierre de parciales, pase de lista, tareas y entregas. **Reabrir** (vuelve a Cerrando) solo con el permiso "Reabrir el año escolar" y motivo; **Archivar** = solo lectura y fuera de los selectores de año. Script 090 de la API (prueba con ROLLBACK en `Scripts/pruebas/090_CierreAnio_prueba.sql`). Permisos: "Cierre del año" (admin, director, secretaria: ver), "Cerrar el año" (admin, director), "Reabrir el año escolar" (admin). De paso `spGraduarAlumno` archiva la matrícula por año en vez de la tabla vacía `tbMatriculas` (antes no podía graduar a ningún alumno inscrito por año). Pendientes: Graduación sigue fallando para alumnos con ficha de salud, casos de disciplina, planes de apoyo, puntos de actividades, entregas o constancias (borra al alumno y esas tablas lo referencian); noveno grado se propone "se gradúa" igual que en Graduación (si sigue a décimo en el colegio, Dirección elige Promovido); la clave `nota_minima_aprobacion` (60) que usa Graduación no coincide con la Escala de notas (70); *Inscripción y promociones* todavía puede promover desde un año cerrado; al guardar una plantilla de evaluación los cuadernos de un año cerrado reciben los pesos nuevos (sus notas no cambian); una tarea rechazada por año cerrado muestra un error general; las recuperaciones las registra Dirección (no hay pantalla para el docente); Constancias y expediente todavía no leen los resultados congelados |

No aplican: biblioteca, internado/dormitorios, varias sedes, presupuesto contable (el ADM ya va adelante en finanzas y nómina).

### 1 · Disciplina — cómo lo tiene Skuul (se copia igual, en 3 pantallas)
- **Casos** (listado): filtros por estado y tipo, "solo los que necesitan trabajo", contador de abiertos; tabla Caso (resumen + referencia + n.º de personas) · Tipo (con candado si es reservado) · Estado · Ocurrió · Lo atiende · Abrir.
- **Registrar un caso**: resumen, tipo (Conducta / Protección del menor = reservado), cuándo (no futuro), dónde, quién lo atiende, qué pasó; filas de personas (alumno + papel + nota, hasta 20). Papeles: implicado, afectado, testigo, quien reporta, quien atiende.
- **Ficha del caso**: referencia `CASO-AA-XXXXXX`, datos clave (estado, cuándo/dónde, quién lo atiende, pendientes), mover de estado con motivo, personas, acciones (tipo, qué hay que hacer, responsable, fecha, marcar hecha), notas que no se editan (privadas o no) e historial de estados.
- Estados: Reportado → En revisión → Derivado → Con medidas → Resuelto → Cerrado (con transiciones permitidas; cerrado = solo lectura; resuelto se puede reabrir).
- Un caso de protección del menor solo lo ven quien tiene el permiso de casos reservados, quien lo reportó y quien lo atiende.
- Hecho así en el ADM: `DisciplinaController` (Index · Registrar · Caso), tablas `app.tbCasosDisciplina*`, API `CasosDisciplina/*`. Permisos: "Casos de disciplina", "Registrar caso de disciplina", "Atender casos de disciplina", "Ver casos reservados" (admin y dirección, como Skuul).
- Pendiente: decidir si docentes pueden **registrar** casos (en Skuul no; en un colegio suele hacerlo el docente que ve la falta). Mostrar los casos de un alumno en su ficha 360 (enlace, no copia). Dato con codificación rota en `tbPersonas` ("Ã“scar").

### 4 · Constancias y expediente — pendientes
- Hecho así en el ADM: `ConstanciasController` (Index · Emitir · Ver), API `DocumentosEmitidos/*`. Permisos: "Constancias y expediente" (ver: admin, dirección, secretaría) y "Emitir constancias" (admin, secretaría). Un documento emitido no se edita ni se borra; una corrección es la revisión siguiente y exige motivo.
- El membrete toma el nombre y la dirección de *Ajustes › Institución* (`App_Data/instituto-config.json`). Hoy no está configurado y sale el valor por defecto "Instituto Nova Sula", mientras el menú dice "Inst. San Francisco": dejar un solo nombre del colegio.
- No hay logo del colegio en el documento (se usa un ícono). Agregar el logo a la configuración institucional.
- La lista de alumnos de *Emitir* se reutiliza de Disciplina (`CasosDisciplina/AlumnosAsync`, todos los alumnos). Si crece, cambiar a búsqueda en el servidor.
- El listado filtra por texto en el ADM después de traer todo; si hay muchos documentos, pasar la búsqueda y la paginación a la API.
- Falta "Emitir boletines de toda una sección" de una vez (Skuul lo hace por sección y período).

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
- ~~**Modo oscuro:** menú lateral, barra de migas y `text-muted`~~ (hecho). Pendiente: en *Docencia diaria* la etiqueta "Horario" desaparece.
- ✅ **Pase de lista para dirección** (hecho 2026-11-23, rama `feature/pase-lista`, script 080 de la API): admin y director ven todos los horarios del año con filtros Nivel → Grado → Sección y pueden pasar lista; el docente sigue viendo solo los suyos y, en la fecha de una sustitución, también la clase que cubre (marcada "Sustitución de ..."). El servidor valida titular, suplente en esa fecha o dirección al listar, guardar y pedir alertas de salud.
- ~~**"Sidebar compacto" en escritorio**~~ (hecho): la hamburguesa se muestra también en escritorio y oculta o muestra el menú.
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

### Reportes sueltos → `Centro de reportes` (2026-11-23)
Un solo lugar (`/CentroReportes`) para pedir cualquier reporte con sus filtros, verlo y bajarlo en Excel o PDF, con "Mis reportes recientes" para repetirlo (tabla `app.tbReportesGenerados`, script API 081). Cada reporte solo aparece y solo se ejecuta si el usuario tiene el permiso de su pantalla de origen (validado en el servidor). Los `<li>` se comentaron; controllers y vistas siguen vivos.

| Pantalla | Rol que la usa | Tarea concreta | ¿Duplica? | Veredicto |
|---|---|---|---|---|
| Panel financiero (`ReportesFinancieros/DashboardFinanciero`) | Contador | Ver de un vistazo cómo van las finanzas | No (agregado) | Mantener |
| Ingresos del mes | Contador | Ver lo cobrado en un mes | La API no está conectada (servicio comentado, procedimientos inexistentes): la pantalla no carga | Deshabilitar → "Ingresos por concepto" del Centro |
| Comparativa anual | Contador | Comparar cobros entre años | Misma falla de API | Deshabilitar → "Flujo de caja por mes" por año |
| Proyección de cobros | Contador | Ver lo que falta cobrar del mes | Misma falla de API | Deshabilitar → "Cargos pendientes de cobro" |
| Pendientes por cobrar | Contador, cajero | Lista de cargos con saldo | Sí, con Cuentas por cobrar (filtro vencidas) | Pasar al centro ("Cargos pendientes de cobro") |
| Historial de transacciones | Cajero, contador | Pagos de un rango de fechas | Sí, con Movimientos de caja | Pasar al centro ("Pagos recibidos") |
| Conciliación rápida | Cajero | Cuadrar pagos del día | No (operativa) | Mantener |
| Flujo de caja | Contador | Cobrado vs pendiente por mes (año) | No | Pasar al centro (mismo filtro: año) |
| Estado de resultados | Contador | Resumen de un mes | No | Pasar al centro (año + mes) |
| Análisis de ingresos | Contador | Cobrado y pendiente por concepto (año) | No | Pasar al centro ("Cobrado y pendiente por concepto") |
| Comparativo por año | Director | Matrícula por programa entre dos años | No | Pasar al centro ("Matrícula: comparativo entre dos años") |
| Indicadores financieros | Contador, director | Indicadores del mes | Revisar si duplica el Panel financiero | Mantener |
| Antigüedad de saldos | Contador | Deuda por días de atraso | No | Pasar al centro |
| Mayores deudores | Contador | A quién llamar primero | No | Pasar al centro (con "cuántos alumnos") |
| Morosidad por nivel | Contador, director | % de morosidad por programa | No | Pasar al centro ("Morosidad por programa") |
| Gráficos (`VisorGraficos`) | — (solo admin: su permiso no existe en la DB) | Gráficos resumen | Sí, con los dashboards | Deshabilitar |
| Reportes pre-generados | Director, consulta | Índice de enlaces a reportes | Sí, lo reemplaza el Centro | Deshabilitar |
| Pirámide de matrícula | Director, secretaria | Alumnos por programa y sexo | No | Pasar al centro ("Matrícula por programa y sexo") |
| Antigüedad (RRHH) | RRHH | Años de cada empleado | No | Pasar al centro ("Antigüedad del personal") |
| Analítica académica | Director | Indicadores y alumnos en riesgo | No (agregado) | Mantener |
| Reporte de asistencia | Director, docente | Asistencia por sección con planilla día a día | No | Mantener (el Centro solo agrega la exportación "Asistencia de alumnos por sección") |
| Cuadro de honor | Director | Mejores promedios con su configuración | No | Mantener |
| Cumpleaños | RRHH | Tarjetas de cumpleaños del mes | No | Mantener |
| Asistencia del personal | RRHH, director | Registrar asistencia diaria | No (operativa) | Mantener (el Centro agrega el resumen por empleado) |
| Cumplimiento documental | RRHH, director | Marcar documentos entregados | No (operativa) | Mantener (el Centro agrega "Documentos pendientes del personal") |

Pendientes:
- [ ] Los nombres de permiso de varios controllers no coinciden con `seguridad.tbPantallas` (ej. `[SessionManager("Flujo de Caja")]` vs "Flujo de caja proyectado"; igual Estado de resultados, Análisis de ingresos, Comparativo de años, Pendientes por cobrar). Por eso solo el admin podía abrir esas pantallas. El Centro acepta ambos nombres.
- [ ] `EmpleadosAntiguedad` devuelve siempre 0 años (la columna del procedimiento se llama "Años" y la API espera "Anos"); el Centro calcula años y meses. La fecha usada es la de registro de la persona, no una fecha de contratación real.
- [ ] `RendimientoPorSeccion` devuelve el mismo total de alumnos para secciones de programas distintos: revisar antes de sumarlo al Centro.
- [ ] Decidir si se borran los controllers y vistas de las pantallas que pasaron al Centro.

### `EstadoCuentaAdm` → consolidar en `CuentasCobrar/EstadoCuenta` (2026-09-19)
Dos "Estado de cuenta" del alumno hacían lo mismo. Se dejó **`CuentasCobrar/EstadoCuenta`** como canónica (accionable: cobra, descuentos, mora, histórico con recibos) y se le **agregó Imprimir/PDF** (membrete + `@media print`) que solo tenía la otra. `EstadoCuentaAdm` se **ocultó solo para cajero** (`!esCajero` en sidebar) y se corrigió su símbolo "Q"→"L".
- [ ] Verificar que director/contador/consulta que usan `EstadoCuentaAdm` tengan acceso a `CuentasCobrar/EstadoCuenta` (permiso "Listado de cuentas por cobrar") antes de ocultarla globalmente.
- [ ] Luego: comentar globalmente el `<li>` de `EstadoCuentaAdm` y retirar `estadocuentaadm.js` / vista.
- [ ] Opcional: cargar nombre real del instituto en el membrete de impresión (hoy es texto fijo "Instituto San Francisco"; `EstadoCuentaAdm` lo traía de `InstitutoConfig/GetAsync`).

### Caja: de 7 pantallas a 3 (2026-11-15)
**Cuentas por cobrar** (`CuentasCobrar/Index`, filtro Pendientes/Vencidas/Pagadas/Todas + antigüedad + mora), **Movimientos de caja** (`Pagos/Index`, hoy/semana/mes/año o rango; anulados tachados y fuera de los totales) y **Cuenta del alumno** (`CuentasCobrar/EstadoCuenta`, con anular pago, perdonar cargo, devolver dinero y usar saldo a favor; permiso "Corregir cuentas").
Retiradas (la ruta vieja redirige): Deudores, Cargos pendientes, Intereses por mora y Morosos → Cuentas por cobrar (`?estado=`); Historial de pagos y Pagos del día → Movimientos de caja. "Aplicar descuento" del estado de cuenta lo reemplaza "Perdonar".
- [ ] Director (1002) tiene "Corregir cuentas" pero no "Listado de cuentas por cobrar": no puede abrir la Cuenta del alumno. Decidir si se le concede.
- [ ] La API fecha los pagos y correcciones con el reloj del contenedor (UTC): un cobro de la noche cae en el día siguiente y no aparece en "Hoy".
- [ ] `ObtenerResumenFinancieroAsync` devuelve `totalPendiente = 0` con cargos vencidos; la pantalla calcula el pendiente con la lista de cargos.
- [ ] Hub `Cartera` y `EstadoCuentaAdm` siguen duplicando parte de Cuentas por cobrar / Cuenta del alumno: decidir si se retiran.

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

### Alumnos y matrícula
- [ ] Cupo de las secciones del año: hoy se fijó por script (35 o los matriculados); falta una pantalla para editarlo (Año Lectivo está congelado).
- [ ] Lista de espera → Matricular: si la modalidad guardada del aspirante no corresponde al grado, el combo queda para elegir a mano (revisar datos de aspirantes).

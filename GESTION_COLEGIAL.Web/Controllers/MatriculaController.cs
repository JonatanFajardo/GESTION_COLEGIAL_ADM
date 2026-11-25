using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
	/// <summary>
	/// Wizard de Matrícula nueva. Consolida en un flujo guiado de 4 pasos lo que hoy
	/// es un formulario plano (Alumnos/Nuevo): datos del alumno + cascada académica
	/// (Nivel → Grado → Modalidad → Curso → Sección → Estado), encargado(s) con
	/// parentesco, checklist de documentos y resumen de confirmación.
	///
	/// NO reemplaza Alumnos/Nuevo (se deja intacta). Reutiliza únicamente los
	/// SERVICIOS de negocio ya existentes (AlumnosService, EncargadosService,
	/// ParentescosService, DocumentosAlumnoAdmService) — no se tocó ningún
	/// controlador ni vista de Alumnos/Encargados.
	///
	/// El encargado (nuevo o ya registrado) se liga al alumno con su parentesco en
	/// app.tbEncargados_tbAlumnos al confirmar la matrícula.
	///
	/// Hueco documentado:
	/// 2) No existe endpoint para generar ni consultar una plantilla de "documentos
	///    requeridos" para un alumno que aún no existe (DocumentosAlumno/ListAsync
	///    exige un Alu_Id ya creado, y no hay acción de alta de documento, sólo
	///    UpdateAsync de estado). Por eso el Paso 3 muestra un checklist de
	///    referencia fijo en el cliente (no persistido) y, tras guardar el alumno,
	///    enlaza a la pantalla dedicada DocumentosAlumnoAdm para el seguimiento real.
	/// </summary>
	[SessionManager("Matrícula nueva")]
	public class MatriculaController : BaseController
	{
		private readonly AlumnosService alumnosService = new AlumnosService();
		private readonly EncargadosService encargadosService = new EncargadosService();
		private readonly ParentescosService parentescosService = new ParentescosService();
		private readonly MatriculaAnioService matriculaAnioService = new MatriculaAnioService();
		private readonly AnioConexionService anioConexionService = new AnioConexionService();
		private readonly DocumentosAlumnoAdmService documentosService = new DocumentosAlumnoAdmService();
		private readonly ListaEsperaService listaEsperaService = new ListaEsperaService();

		/// <summary>Matricular desde la lista de espera pide además "Gestionar lista de espera".</summary>
		private bool PuedeDesdeListaEspera() =>
			(Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase)
			|| TienePantalla(ListaEsperaController.PantallaGestionar);

		/// <summary>
		/// Vista principal del wizard. Precarga Niveles Educativos y Estados
		/// (mismo Dropdown que usa Alumnos/Nuevo) para el Paso 1.
		/// </summary>
		public async Task<ActionResult> Index(int? aspiranteId)
		{
			var model = new AlumnoViewModel();
			model = await alumnosService.Dropdown(model);
			var anios = await anioConexionService.AniosInscripcionAsync();
			ViewBag.AniosInscripcion = anios;
			ViewBag.AnioSeleccionado = (anios.FirstOrDefault(a => a.EsActivo) ?? anios.FirstOrDefault())?.Ani_Id ?? 0;
			ViewBag.AnioFijo = false;
			if (aspiranteId.HasValue && aspiranteId.Value > 0)
				await PrepararAspiranteAsync(aspiranteId.Value, anios);
			return View(model);
		}

		/// <summary>
		/// Desde la lista de espera (?aspiranteId=): precarga lo que el wizard ya captura (niño, encargado,
		/// nivel/grado/modalidad/curso). Solo un aspirante con cupo ofrecido en un año que admita inscripciones (el activo o uno siguiente); si no, un aviso.
		/// </summary>
		private async Task PrepararAspiranteAsync(int aspiranteId, System.Collections.Generic.List<AnioInscripcionViewModel> anios)
		{
			if (!PuedeDesdeListaEspera())
			{
				ViewBag.AspiranteAviso = "No tiene permiso para matricular desde la lista de espera. Puede registrar la matrícula normal.";
				return;
			}
			AspiranteViewModel a = await listaEsperaService.FindAsync(aspiranteId);
			if (a == null || a.Asp_Id == 0)
				ViewBag.AspiranteAviso = "El aspirante ya no existe en la lista de espera.";
			else if (a.Asp_Estado != "OF")
				ViewBag.AspiranteAviso = $"{a.Aspirante} está «{CatalogoAspirantes.Estado(a.Asp_Estado)}»: solo se matricula un aspirante con cupo ofrecido.";
			else if (!anios.Any(x => x.Ani_Id == a.Ani_Id))
				ViewBag.AspiranteAviso = $"{a.Aspirante} aspira al año {a.Ani_Nombre}, que ya no admite matrículas o todavía no tiene secciones creadas.";
			else
			{
				ViewBag.AnioSeleccionado = a.Ani_Id;
				ViewBag.AnioFijo = true;
				ViewBag.Aspirante = a;
				ViewBag.AspiranteJson = JsonConvert.SerializeObject(new
				{
					id = a.Asp_Id,
					nombres = a.Asp_Nombres,
					apellidos = a.Asp_Apellidos,
					identidad = a.Asp_Identidad,
					nacimiento = a.Asp_FechaNacimiento.ToString("yyyy-MM-dd"),
					sexo = a.Asp_Sexo,
					niv = a.Niv_Id,
					cun = a.Cun_Id,
					mda = a.Mda_Id,
					cur = a.Cur_Id,
					encargadoNombre = a.Asp_EncargadoNombre,
					encargadoIdentidad = a.Asp_EncargadoIdentidad,
					parId = a.Par_Id,
					telefono = a.Asp_EncargadoTelefono,
					correo = a.Asp_EncargadoCorreo
				}, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
			}
		}

		// ============================================================
		// PASO 1 — Cascada académica (Nivel → Grado → Modalidad → Curso → Sección)
		// Proxy 1:1 a los métodos ya existentes en AlumnosService, para no duplicar
		// lógica de negocio ni depender de permisos de AlumnosController.
		// ============================================================

		[HttpGet]
		public async Task<ActionResult> GetGradosAsync(int id)
		{
			var result = await alumnosService.CursoNivelesDropdown(id);
			var items = (result ?? Enumerable.Empty<CursoNivelDropViewModel>())
				.Select(x => new { Value = x.Cun_Id, Text = x.Cun_Descripcion });
			return AjaxResult(items);
		}

		[HttpGet]
		public async Task<ActionResult> GetModalidadesAsync(int id)
		{
			var result = await alumnosService.ModalidadesDropdown(id);
			var items = (result ?? Enumerable.Empty<ModalidadViewModel>())
				.Select(x => new { Value = x.Mda_Id, Text = x.Mda_Descripcion });
			return AjaxResult(items);
		}

		[HttpGet]
		public async Task<ActionResult> GetCursosAsync(int id)
		{
			var result = await alumnosService.CursosDropdown(id);
			var items = (result ?? Enumerable.Empty<CursoViewModel>())
				.Select(x => new { Value = x.Cur_Id, Text = x.Cur_Nombre });
			return AjaxResult(items);
		}

		[HttpGet]
		public async Task<ActionResult> GetSeccionesAsync(int id)
		{
			var result = await alumnosService.SeccionesDropdown(id);
			var items = (result ?? Enumerable.Empty<SeccionViewModel>())
				.Select(x => new { Value = x.Sec_Id, Text = x.Sec_Descripcion });
			return AjaxResult(items);
		}

		/// <summary>Evita alta duplicada: reutiliza Alumnos/FindByIdentidadAsync.</summary>
		[HttpGet]
		public async Task<ActionResult> BuscarAlumnoPorIdentidadAsync(string identidad)
		{
			var result = await alumnosService.FindByIdentidad(identidad);
			return AjaxResult(result);
		}

		// ============================================================
		// PASO 2 — Encargado(s) + Parentesco
		// ============================================================

		/// <summary>Lista de encargados existentes para "vincular" uno ya registrado.</summary>
		[HttpGet]
		public async Task<ActionResult> ListarEncargadosAsync()
		{
			var result = await encargadosService.ListAsync();
			var items = (result ?? Enumerable.Empty<EncargadoViewModel>())
				.Select(x => new { Value = x.Enc_Id, Text = x.Enc_Nombre, Identidad = x.Per_Identidad, Telefono = x.Per_Telefono });
			return AjaxResult(items);
		}

		/// <summary>Catálogo de parentescos (Padre, Madre, Tutor legal, etc.).</summary>
		[HttpGet]
		public async Task<ActionResult> ListarParentescosAsync()
		{
			var result = await parentescosService.ListAsync();
			var items = (result ?? Enumerable.Empty<ParentescoViewModel>())
				.Select(x => new { Value = x.Par_Id, Text = x.Par_Descripcion });
			return AjaxResult(items);
		}

		/// <summary>
		/// Registra al encargado nuevo (Persona + Encargado) y lo liga al alumno recién creado con su parentesco.
		/// Si la identidad ya pertenece a un encargado, no lo duplica: solo lo liga.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> GuardarEncargadoAsync(EncargadoViewModel model, int aluId, int parId)
		{
			if (aluId <= 0 || parId <= 0)
				return Json(new { success = false, message = "Falta el alumno o el parentesco del encargado." });
			if (!ModelState.IsValid)
			{
				var primero = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault(m => !string.IsNullOrWhiteSpace(m));
				return Json(new { success = false, message = primero ?? "Revise los datos del encargado." });
			}

			var existente = await BuscarEncargadoPorIdentidadAsync(model.Per_Identidad);
			if (existente == null)
			{
				bool huboError = await encargadosService.Create(model);
				if (huboError)
					return Json(new { success = false, message = "No se pudo registrar al encargado." });
				existente = await BuscarEncargadoPorIdentidadAsync(model.Per_Identidad);
				if (existente == null)
					return Json(new { success = false, message = "El encargado no quedó registrado. Revise sus datos." });
			}
			var vinculo = await encargadosService.VincularAsync(existente.Enc_Id, aluId, parId);
			return vinculo.Error
				? Json(new { success = false, message = vinculo.Mensaje })
				: Json(new { success = true, enc_Id = existente.Enc_Id });
		}

		/// <summary>Liga un encargado ya registrado al alumno recién creado, con su parentesco.</summary>
		[HttpPost]
		public async Task<ActionResult> VincularEncargadoAsync(int aluId, int encId, int parId)
		{
			if (aluId <= 0 || encId <= 0 || parId <= 0)
				return Json(new { success = false, message = "Falta el alumno, el encargado o el parentesco." });
			var vinculo = await encargadosService.VincularAsync(encId, aluId, parId);
			return vinculo.Error
				? Json(new { success = false, message = vinculo.Mensaje })
				: Json(new { success = true });
		}

		private async Task<EncargadoViewModel> BuscarEncargadoPorIdentidadAsync(string identidad)
		{
			if (string.IsNullOrWhiteSpace(identidad)) return null;
			var lista = await encargadosService.ListAsync();
			return (lista ?? Enumerable.Empty<EncargadoViewModel>()).FirstOrDefault(e => e.Per_Identidad == identidad);
		}

		// ============================================================
		// PASO 4 — Confirmación final: crea el Alumno.
		// ============================================================

		[HttpPost]
		public async Task<ActionResult> GuardarAlumnoAsync(AlumnoViewModel model, int? aspiranteId, int? aniId)
		{
			bool desdeLista = aspiranteId.HasValue && aspiranteId.Value > 0;
			if (desdeLista && !PuedeDesdeListaEspera())
			{
				return Json(new { success = false, message = "No tiene permiso para matricular desde la lista de espera." });
			}

			if (!ModelState.IsValid)
			{
				var primero = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault(m => !string.IsNullOrWhiteSpace(m));
				return Json(new { success = false, message = primero ?? "Revise los datos del alumno." });
			}

			// Una identidad no se repite entre alumnos.
			var yaExiste = await alumnosService.FindByIdentidad(model.Per_Identidad);
			if (yaExiste != null && yaExiste.Alu_Id > 0)
				return Json(new { success = false, message = "Ya existe un alumno con esa identidad." });

			// Desde la lista de espera se revisa ANTES de crear al alumno: si el aspirante ya no tiene la oferta
			// (otra pestaña lo matriculó, lo rechazaron) o la identidad ya es de un alumno, no se crea nada.
			if (desdeLista)
			{
				AspiranteViewModel asp = await listaEsperaService.FindAsync(aspiranteId.Value);
				if (asp == null || asp.Asp_Id == 0)
					return Json(new { success = false, message = "El aspirante ya no existe en la lista de espera." });
				if (asp.Asp_Estado != "OF")
					return Json(new { success = false, message = $"{asp.Aspirante} está «{CatalogoAspirantes.Estado(asp.Asp_Estado)}»: solo se matricula un aspirante con cupo ofrecido." });
				aniId = asp.Ani_Id; // el año lo fija el aspirante
				var aniosOk = await anioConexionService.AniosInscripcionAsync();
				if (!aniosOk.Any(x => x.Ani_Id == asp.Ani_Id))
					return Json(new { success = false, message = $"{asp.Aspirante} aspira al año {asp.Ani_Nombre}, que ya no admite matrículas o todavía no tiene secciones creadas." });
				var existente = string.IsNullOrWhiteSpace(model.Per_Identidad) ? null : await alumnosService.FindByIdentidad(model.Per_Identidad);
				if (existente != null && existente.Alu_Id > 0)
					return Json(new { success = false, message = "Ya existe un alumno con esa identidad. Revise la identidad del aspirante." });
			}

			// El año y el cupo de su sección se revisan ANTES de crear al alumno (una vez creado ya no se puede deshacer).
			{
				var aniosOk = await anioConexionService.AniosInscripcionAsync();
				if (aniId.HasValue && aniId.Value > 0 && !aniosOk.Any(x => x.Ani_Id == aniId.Value))
					return Json(new { success = false, message = "Ese año no admite matrículas: elija el año activo o uno siguiente con secciones." });
				var cupo = await anioConexionService.VerificarCupoAsync(aniId, model.Niv_Id, model.Cun_Id, model.Mda_Id, model.Sec_Id);
				if (cupo != null && !cupo.Permitido)
					return Json(new { success = false, message = cupo.Mensaje });
			}

			bool huboError = await alumnosService.Create(model);
			if (huboError)
			{
				return Json(new { success = false, message = "No se pudo crear al alumno. Revise los datos e intente de nuevo." });
			}

			// El API no regresa el Alu_Id recién creado desde CreateAsync (sólo boolean invertido,
			// ver SendHttpClient). Se recupera por identidad para poder enlazar a Documentos/Detalle.
			var creado = await alumnosService.FindByIdentidad(model.Per_Identidad);
			if (creado == null || creado.Alu_Id <= 0)
				return Json(new { success = false, message = "El alumno no quedó registrado. Revise los datos e intente de nuevo." });
			// Lo inscribe en el año activo, en la sección del año de su ubicación (si falta, la crea).
			// Si falla, la matrícula igual queda hecha: se puede inscribir después en Inscripción y promociones.
			bool inscritoEnAnio = false;
			string avisoInscripcion = null, nombreAnio = null;
			if (creado != null && creado.Alu_Id > 0)
			{
				var inscripcion = await anioConexionService.InscribirAlumnoAsync(creado.Alu_Id, aniId, GetUsuarioId());
				inscritoEnAnio = inscripcion != null && inscripcion.Ok;
				nombreAnio = inscripcion?.Ani_Nombre;
				if (!inscritoEnAnio) avisoInscripcion = inscripcion?.Mensaje;
			}
			if (!desdeLista)
			{
				return AjaxResult(new { alu_Id = creado?.Alu_Id ?? 0, alu_Nombre = creado?.Alu_Nombre, inscrito_anio = inscritoEnAnio, anio_nombre = nombreAnio, aviso_inscripcion = avisoInscripcion }, true);
			}

			// Desde la lista de espera: el aspirante (con cupo ofrecido) pasa a Matriculado con su Alu_Id.
			AspiranteResultadoViewModel marca = creado != null && creado.Alu_Id > 0
				? await listaEsperaService.MatricularAsync(aspiranteId.Value, creado.Alu_Id, GetUsuarioId())
				: null;
			var aspirante = new
			{
				ok = marca?.Ok == true,
				mensaje = marca?.Ok == true
					? "El aspirante quedó como matriculado en la lista de espera."
					: "El alumno se creó, pero el aspirante no se pudo marcar como matriculado: " + (marca?.Mensaje ?? "no se encontró el alumno recién creado."),
				urlAspirante = Url.Action("Aspirante", "ListaEspera", new { id = aspiranteId.Value, aviso = marca?.Ok == true ? "matriculado" : null }),
				urlLista = Url.Action("Index", "ListaEspera")
			};
			return AjaxResult(new { alu_Id = creado?.Alu_Id ?? 0, alu_Nombre = creado?.Alu_Nombre, inscrito_anio = inscritoEnAnio, anio_nombre = nombreAnio, aviso_inscripcion = avisoInscripcion, aspirante }, true);
		}
	}
}

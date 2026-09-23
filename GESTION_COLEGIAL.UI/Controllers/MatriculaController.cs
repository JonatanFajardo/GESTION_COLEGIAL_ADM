using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

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
	/// Huecos de API documentados (ver también TODO.md):
	/// 1) No existe endpoint para vincular Alumno-Encargado-Parentesco al momento
	///    del alta. EncargadoViewModel no tiene Alu_Id ni Par_Id. Encargados/ByAlumnoAsync
	///    (lectura) existe en el API externo, pero no hay contraparte de escritura.
	///    Por eso GuardarEncargadoAsync crea al encargado (Persona + Encargado) pero
	///    el parentesco elegido en el wizard NO se persiste en ningún lado; sólo viaja
	///    en el resumen visual de Paso 4 para que el usuario lo capture manualmente.
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
		private readonly DocumentosAlumnoAdmService documentosService = new DocumentosAlumnoAdmService();

		/// <summary>
		/// Vista principal del wizard. Precarga Niveles Educativos y Estados
		/// (mismo Dropdown que usa Alumnos/Nuevo) para el Paso 1.
		/// </summary>
		public async Task<ActionResult> Index()
		{
			var model = new AlumnoViewModel();
			model = await alumnosService.Dropdown(model);
			return View(model);
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
		/// Crea un encargado nuevo (Persona + Encargado) reutilizando EncargadosService.Create.
		/// HUECO DOCUMENTADO: el Par_Id (parentesco) recibido aquí NO se envía a la API porque
		/// EncargadoViewModel no tiene campo de parentesco ni de Alu_Id — no hay dónde persistirlo.
		/// Se recibe solo para poder mostrarlo en el resumen de Paso 4.
		/// </summary>
		[HttpPost]
		public async Task<ActionResult> GuardarEncargadoAsync(EncargadoViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
			}

			bool huboError = await encargadosService.Create(model);
			return huboError
				? AjaxResult(false, AlertMessage.AlertMessageCustomType.Error)
				: AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
		}

		// ============================================================
		// PASO 4 — Confirmación final: crea el Alumno.
		// ============================================================

		[HttpPost]
		public async Task<ActionResult> GuardarAlumnoAsync(AlumnoViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
			}

			bool huboError = await alumnosService.Create(model);
			if (huboError)
			{
				return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
			}

			// El API no regresa el Alu_Id recién creado desde CreateAsync (sólo boolean invertido,
			// ver SendHttpClient). Se recupera por identidad para poder enlazar a Documentos/Detalle.
			var creado = await alumnosService.FindByIdentidad(model.Per_Identidad);
			return AjaxResult(new { alu_Id = creado?.Alu_Id ?? 0, alu_Nombre = creado?.Alu_Nombre }, true);
		}
	}
}

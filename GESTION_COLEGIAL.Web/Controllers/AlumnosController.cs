using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
	/// <summary>
	/// Controlador para gestionar la entidad Alumnos.
	/// </summary>
	[SessionManager("Listado de alumnos")]
	public class AlumnosController : BaseController
	{
		private readonly AlumnosService alumnosService = new AlumnosService();
		private readonly BoletinAdmService boletinService = new BoletinAdmService();
		private readonly EstadoCuentaAdmService estadoCuentaService = new EstadoCuentaAdmService();
		private readonly DocumentosAlumnoAdmService documentosService = new DocumentosAlumnoAdmService();
		private readonly AsistenciaAdmService asistenciaService = new AsistenciaAdmService();
		private readonly Ficha360AlumnoAdmService ficha360Service = new Ficha360AlumnoAdmService();
		private readonly BitacoraService bitacoraService = new BitacoraService();
		private readonly EncargadosService encargadosService = new EncargadosService();
		private readonly ParentescosService parentescosService = new ParentescosService();

		/// <summary>
		/// Muestra la vista del índice.
		/// </summary>
		/// <returns>La vista del índice.</returns>
		public ActionResult Index()
		{
			ViewBag.PuedeCrear = TienePantalla("Crear alumnos");
			ViewBag.PuedeEditar = TienePantalla("Editar alumnos");
			ViewBag.PuedeEliminar = TienePantalla("Eliminar alumnos");
			return View();
		}

		/// <summary>
		/// Muestra la vista de creación de forma asíncrona.
		/// </summary>
		/// <returns>La vista de creación.</returns>
		[SessionManager("Crear alumnos")]
		public async Task<ActionResult> Nuevo()
		{
			return await FormularioAsync(new AlumnoViewModel());
		}

		/// <summary>
		/// Obtiene una lista de alumnos de forma asíncrona.
		/// </summary>
		/// <returns>El resultado en formato Ajax.</returns>
		public async Task<ActionResult> ListAsync()
		{
			var result = await alumnosService.ListAsync();
			return AjaxResult(result);
		}

		/// <summary>
		/// Busca un alumno por su ID de forma asíncrona.
		/// </summary>
		/// <param name="id">ID del alumno.</param>
		/// <returns>La vista de creación con el alumno encontrado.</returns>
		[SessionManager("Editar alumnos")]
		public async Task<ActionResult> Editar(int id)
		{
			var result = await alumnosService.Find(id);
			if (result == null || result.Alu_Id == 0)
			{
				AlertMessage.Show(AlertMessage.AlertMessageType.Error, "No se encontró el alumno");
				return RedirectToAction("Index");
			}
			return await FormularioAsync(result);
		}

		/// <summary>
		/// Arma el formulario con sus listas: niveles y estados, más las opciones de grado, modalidad, curso y
		/// sección que corresponden a lo que el alumno ya tiene elegido (cada una filtrada por su padre).
		/// </summary>
		private async Task<ActionResult> FormularioAsync(AlumnoViewModel model)
		{
			var drop = await Dropdown(model);
			ViewBag.Niveles = model.Niv_Id > 0 ? Items(await alumnosService.CursoNivelesDropdown(model.Niv_Id), x => x.Cun_Id, x => x.Cun_Descripcion) : new List<SelectListItem>();
			ViewBag.Modalidades = model.Cun_Id > 0 ? Items(await alumnosService.ModalidadesDropdown(model.Cun_Id), x => x.Mda_Id, x => x.Mda_Descripcion) : new List<SelectListItem>();
			ViewBag.Cursos = model.Mda_Id > 0 ? Items(await alumnosService.CursosDropdown(model.Mda_Id), x => x.Cur_Id, x => x.Cur_Nombre) : new List<SelectListItem>();
			ViewBag.Secciones = model.Cur_Id > 0 ? Items(await alumnosService.SeccionesDropdown(model.Cur_Id), x => x.Sec_Id, x => x.Sec_Descripcion) : new List<SelectListItem>();
			return View("CreateAsync", drop);
		}

		private static List<SelectListItem> Items<T>(IEnumerable<T> origen, System.Func<T, int> valor, System.Func<T, string> texto) =>
			(origen ?? Enumerable.Empty<T>()).Select(x => new SelectListItem { Value = valor(x).ToString(), Text = texto(x) }).ToList();

		/// <summary>
		/// Muestra la vista de detalle de un alumno de forma asíncrona.
		/// </summary>
		/// <param name="id">ID del alumno.</param>
		/// <returns>La vista de detalle del alumno.</returns>
		public async Task<ActionResult> Detalle(int id)
		{
			var result = await alumnosService.Find(id);
			if (result == null)
			{
				AlertMessage.Show(AlertMessage.AlertMessageType.Error, "No se encontró el alumno");
				return RedirectToAction("Index");
			}
			return View("Detail", result);
		}

		/// <summary>
		/// Busca un alumno por su número de identidad de forma asíncrona.
		/// </summary>
		/// <param name="identidad">Número de identidad del alumno.</param>
		/// <returns>El resultado en formato Ajax con los datos del alumno.</returns>
		[HttpGet]
		public async Task<ActionResult> FindByIdentidadAsync(string identidad)
		{
			var result = await alumnosService.FindByIdentidad(identidad);
			return AjaxResult(result);
		}

		/// <summary>
		/// Busca un alumno por su ID y devuelve sus datos en formato Ajax (JSON).
		/// </summary>
		/// <param name="id">ID del alumno.</param>
		/// <returns>El resultado en formato Ajax con los datos del alumno.</returns>
		[HttpGet]
		public async Task<ActionResult> FindAsync(int id)
		{
			var result = await alumnosService.Find(id);
			return AjaxResult(result);
		}

		/// <summary>
		/// Guarda los cambios realizados en un alumno.
		/// </summary>
		/// <param name="model">El modelo del alumno.</param>
		/// <returns>El resultado de la operación.</returns>
		[HttpPost]
		public async Task<ActionResult> Guardar(AlumnoViewModel model)
		{
			// Crear pide "Crear alumnos"; editar pide "Editar alumnos".
			if (!TienePantalla(model.Alu_Id == 0 ? "Crear alumnos" : "Editar alumnos"))
			{
				AlertMessage.Show(AlertMessage.AlertMessageType.Error, model.Alu_Id == 0 ? "No tiene permiso para crear alumnos" : "No tiene permiso para editar alumnos");
				return RedirectToAction("Index");
			}
			// La ubicación académica se elige completa (nivel, grado, modalidad, curso, sección y estado).
			if (model.Niv_Id <= 0) ModelState.AddModelError("Niv_Id", "Seleccione el nivel educativo");
			if (model.Cun_Id <= 0) ModelState.AddModelError("Cun_Id", "Seleccione el grado");
			if (model.Mda_Id <= 0) ModelState.AddModelError("Mda_Id", "Seleccione la modalidad");
			if (model.Cur_Id <= 0) ModelState.AddModelError("Cur_Id", "Seleccione el curso");
			if (model.Sec_Id <= 0) ModelState.AddModelError("Sec_Id", "Seleccione la sección");
			if (model.Est_Id <= 0) ModelState.AddModelError("Est_Id", "Seleccione el estado");
			if (model.Alu_Id == 0 && !string.IsNullOrEmpty(model.Per_Identidad))
			{
				var repetido = await alumnosService.FindByIdentidad(model.Per_Identidad);
				if (repetido != null && repetido.Alu_Id > 0) ModelState.AddModelError("Per_Identidad", "Ya existe un alumno con esa identidad");
			}
			if (!ModelState.IsValid)
			{
				AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Revise los campos marcados en rojo");
				return await FormularioAsync(model);
			}

			if (model.Alu_Id == 0)
			{
				bool result = await alumnosService.Create(model);

				// Validamos error
				if (result)
				{
					AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
					return RedirectToAction("Index");
				}
				AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Insertado exitosamente");
				return RedirectToAction("Index");
			}
			else
			{
				bool result = await alumnosService.Edit(model);

				// Validamos error
				if (result)
				{
					AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error");
					return RedirectToAction("Index");
				}
				AlertMessage.Show(AlertMessage.AlertMessageType.Success, "Editado exitosamente");
				return RedirectToAction("Index");
			}
		}

		/// <summary>
		/// Obtiene los datos para llenar un dropdown de opciones relacionadas con los alumnos.
		/// </summary>
		/// <param name="model">El modelo del alumno.</param>
		/// <returns>El modelo del alumno actualizado con los datos del dropdown.</returns>
		public async Task<AlumnoViewModel> Dropdown(AlumnoViewModel model)
		{
			return await alumnosService.Dropdown(model);
		}

		/// <summary>
		/// Obtiene los cursos y niveles relacionados con un alumno.
		/// </summary>
		/// <param name="id">ID del alumno.</param>
		/// <returns>El resultado en formato Ajax.</returns>
		[HttpGet]
		public async Task<ActionResult> GetCursosNiveles(int id)
		{
			var result = await alumnosService.CursoNivelesDropdown(id);
			IList<SelectListItem> resultToSelectListItem = (result ?? Enumerable.Empty<CursoNivelDropViewModel>()).Select(x => new SelectListItem()
			{
				Value = x.Cun_Id.ToString(),
				Text = x.Cun_Descripcion
			}).ToList();
			return AjaxResult(resultToSelectListItem);
		}

		/// <summary>
		/// Obtiene las modalidades relacionadas con un alumno.
		/// </summary>
		/// <param name="id">ID del alumno.</param>
		/// <returns>El resultado en formato Ajax.</returns>
		public async Task<ActionResult> GetModalidades(int id)
		{
			var result = await alumnosService.ModalidadesDropdown(id);
			IList<SelectListItem> resultToSelectListItem = (result ?? Enumerable.Empty<ModalidadViewModel>()).Select(x => new SelectListItem()
			{
				Value = x.Mda_Id.ToString(),
				Text = x.Mda_Descripcion
			}).ToList();
			return AjaxResult(resultToSelectListItem);
		}

		/// <summary>
		/// Obtiene los cursos relacionados con un alumno.
		/// </summary>
		/// <param name="id">ID del alumno.</param>
		/// <returns>El resultado en formato Ajax.</returns>
		public async Task<ActionResult> GetCursos(int id)
		{
			var result = await alumnosService.CursosDropdown(id);
			IList<SelectListItem> resultToSelectListItem = (result ?? Enumerable.Empty<CursoViewModel>()).Select(x => new SelectListItem()
			{
				Value = x.Cur_Id.ToString(),
				Text = x.Cur_Nombre
			}).ToList();
			return AjaxResult(resultToSelectListItem);
		}

		/// <summary>
		/// Obtiene las secciones relacionadas con un alumno.
		/// </summary>
		/// <param name="id">ID del alumno.</param>
		/// <returns>El resultado en formato Ajax.</returns>
		public async Task<ActionResult> GetSecciones(int id)
		{
			var result = await alumnosService.SeccionesDropdown(id);
			// Validamos error
			if (result == null)
			{
				AlertMessage.Show(AlertMessage.AlertMessageType.Error, "Ha ocurrido un error al procesar la solicitud");
				return AjaxResult(new List<SelectListItem>());
			}
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
			{
				Value = x.Sec_Id.ToString(),
				Text = x.Sec_Descripcion
			}).ToList();
			return AjaxResult(resultToSelectListItem);
		}

		/// <summary>
		/// Elimina un alumno de forma asíncrona.
		/// </summary>
		/// <param name="model">El modelo del alumno.</param>
		/// <returns>El resultado de la operación.</returns>
		[HttpPost]
		[SessionManager("Eliminar alumnos")]
		public async Task<ActionResult> Eliminar(AlumnoViewModel model)
		{
			bool result = await alumnosService.Delete(model.Alu_Id);

			// Validamos error
			if (result)
			{
				return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
			}
			return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
		}

		// ============================================================
		// FICHA DEL ALUMNO — endpoints AJAX que alimentan los 7 tabs.
		// Por ahora devuelven partial views con placeholder data;
		// cuando estén los agregadores reales, se pasa el DTO como model.
		// ============================================================

		/// <summary>KPIs del hero (Promedio, Asistencia, Cuenta, Tareas).</summary>
		public async Task<ActionResult> GetKPIsAsync(int id)
		{
			double promedio = 0;
			int asistencia = 0;
			string cuentaEstado = "—";
			string proximoCargo = "—";

			try
			{
				dynamic resumen = await ficha360Service.GetResumenAsync(id);
				if (resumen != null)
				{
					// Promedio anual (viene en DatosPersonales.PromedioAnual)
					try
					{
						var promRaw = resumen.DatosPersonales?.PromedioAnual;
						if (promRaw != null) promedio = (double)promRaw;
					}
					catch { }

					// % asistencia: se calcula desde AsistenciaReciente
					try
					{
						var asistenciaList = resumen.AsistenciaReciente;
						if (asistenciaList != null)
						{
							int total = 0, presentes = 0;
							foreach (var a in asistenciaList)
							{
								total++;
								string estado = (string)(a.Asi_Estado ?? "");
								if (estado.Equals("Presente", System.StringComparison.OrdinalIgnoreCase) ||
									estado.Equals("P", System.StringComparison.OrdinalIgnoreCase))
								{
									presentes++;
								}
							}
							if (total > 0) asistencia = (int)System.Math.Round((presentes * 100.0) / total);
						}
					}
					catch { }

					// Estado de cuenta y próximo cargo desde CuentasPorCobrar
					try
					{
						var cuentas = resumen.CuentasPorCobrar;
						if (cuentas != null)
						{
							decimal totalPend = 0m;
							System.DateTime? proxFecha = null;
							foreach (var c in cuentas)
							{
								decimal pend = 0m;
								try { pend = (decimal)(c.Cco_MontoPendiente ?? 0m); } catch { }
								totalPend += pend;
								if (pend > 0)
								{
									try
									{
										System.DateTime f = (System.DateTime)c.Cco_FechaVencimiento;
										if (proxFecha == null || f < proxFecha.Value) proxFecha = f;
									}
									catch { }
								}
							}
							cuentaEstado = totalPend > 0 ? $"L. {totalPend:N2} pendiente" : "Al día";
							proximoCargo = proxFecha.HasValue ? proxFecha.Value.ToString("dd/MM/yyyy") : "—";
						}
					}
					catch { }
				}
			}
			catch { }

			return Json(new
			{
				promedio = promedio,
				promedioTrend = 0,
				asistencia = asistencia,
				asistenciaTrend = 0,
				cuentaEstado = cuentaEstado,
				proximoCargo = proximoCargo,
				tareasEntregadas = 0,
				tareasTotal = 0
			});
		}

		public async Task<ActionResult> GetResumenAsync(int id)
		{
			try
			{
				dynamic data = await ficha360Service.GetResumenAsync(id);
				return PartialView("_TabResumen", (object)data);
			}
			catch
			{
				return PartialView("_TabResumen", (object)null);
			}
		}

		// Las notas solo las ve quien ya puede verlas en otra pantalla: "Boletín de calificaciones" (Notas y boletín) o "Constancias y expediente" (Secretaría).
		private bool PuedeVerNotas() => (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", System.StringComparison.OrdinalIgnoreCase) || TienePantalla("Boletín de calificaciones") || TienePantalla("Constancias y expediente");

		public ActionResult GetAcademicoAsync(int id)
		{
			ViewBag.AlumnoId = id;
			ViewBag.PuedeVerNotas = PuedeVerNotas();
			return PartialView("_TabAcademico");
		}

		// Datos del tab Académico (mismo boletín que "Notas y boletín").
		[HttpGet]
		public async Task<ActionResult> BoletinAsync(int Alu_Id, int Anio)
			=> !PuedeVerNotas() ? new StatusCodeResult(403) : AjaxResult(await boletinService.GetAsync(Alu_Id, Anio));

		[HttpGet]
		public async Task<ActionResult> PublicacionAsync(int anio)
			=> !PuedeVerNotas() ? new StatusCodeResult(403) : AjaxResult(await new CierresParcialService().PeriodosAsync(anio));

		[HttpGet]
		public async Task<ActionResult> EscalaAsync()
			=> !PuedeVerNotas() ? new StatusCodeResult(403) : AjaxResult(await new EscalaNotasService().ListAsync());

		[HttpGet]
		public async Task<ActionResult> NotaMinimaAsync()
			=> !PuedeVerNotas() ? new StatusCodeResult(403) : Json(new { valor = await new ConfiguracionService().GetNotaMinimaAprobacionAsync() });

		public async Task<ActionResult> GetAsistenciaAsync(int id)
		{
			try
			{
				var data = await asistenciaService.ByAlumnoAsync(id, System.DateTime.Now.Year);
				return PartialView("_TabAsistencia", (object)data);
			}
			catch
			{
				return PartialView("_TabAsistencia", (object)null);
			}
		}

		public async Task<ActionResult> GetPagosAsync(int id)
		{
			try
			{
				var data = await estadoCuentaService.GetAsync(id, System.DateTime.Now.Year);
				return PartialView("_TabPagos", (object)data);
			}
			catch
			{
				return PartialView("_TabPagos", (object)null);
			}
		}

		public async Task<ActionResult> GetFamiliaAsync(int id)
		{
			try
			{
				var data = await encargadosService.ByAlumnoAsync(id);
				ViewBag.AluId = id;
				ViewBag.PuedeEditar = TienePantalla("Editar alumnos");
				return PartialView("_TabFamilia", data);
			}
			catch
			{
				ViewBag.AluId = id;
				ViewBag.PuedeEditar = TienePantalla("Editar alumnos");
				return PartialView("_TabFamilia", (object)null);
			}
		}

		/// <summary>Encargados ya registrados y parentescos, para ligar uno a este alumno desde la pestaña Familia.</summary>
		[HttpGet]
		[SessionManager("Editar alumnos")]
		public async Task<ActionResult> OpcionesFamiliaAsync()
		{
			var encargados = (await encargadosService.ListAsync() ?? Enumerable.Empty<EncargadoViewModel>())
				.Select(x => new { Value = x.Enc_Id, Text = x.Enc_Nombre, Identidad = x.Per_Identidad });
			var parentescos = (await parentescosService.ListAsync() ?? Enumerable.Empty<ParentescoViewModel>())
				.Select(x => new { Value = x.Par_Id, Text = x.Par_Descripcion });
			return AjaxResult(new { encargados, parentescos });
		}

		/// <summary>Liga un encargado existente al alumno con su parentesco (o cambia el parentesco si ya estaba ligado).</summary>
		[HttpPost]
		[SessionManager("Editar alumnos")]
		public async Task<ActionResult> VincularEncargadoAsync(int aluId, int encId, int parId)
		{
			if (aluId <= 0 || encId <= 0 || parId <= 0)
				return Json(new { success = false, message = "Elija el encargado y el parentesco." });
			var r = await encargadosService.VincularAsync(encId, aluId, parId);
			return Json(new { success = !r.Error, message = r.Error ? r.Mensaje : "Encargado ligado al alumno." });
		}

		/// <summary>Quita el vínculo del encargado con el alumno (el encargado sigue registrado).</summary>
		[HttpPost]
		[SessionManager("Editar alumnos")]
		public async Task<ActionResult> DesvincularEncargadoAsync(int aluId, int encId)
		{
			var r = await encargadosService.DesvincularAsync(encId, aluId);
			return Json(new { success = !r.Error, message = r.Error ? r.Mensaje : "Encargado desligado del alumno." });
		}

		public async Task<ActionResult> GetDocumentosAsync(int id)
		{
			var data = await documentosService.ListAsync(id);
			return PartialView("_TabDocumentos", data);
		}

		public async Task<ActionResult> GetHistorialAsync(int id)
		{
			try
			{
				var data = await bitacoraService.ByAlumnoAsync(id);
				return PartialView("_TabHistorial", data);
			}
			catch
			{
				return PartialView("_TabHistorial", (object)null);
			}
		}
	}
}

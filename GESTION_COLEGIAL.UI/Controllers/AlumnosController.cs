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
			var model = new AlumnoViewModel();
			var drop = await Dropdown(model);
			return View("CreateAsync", drop);
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
			var drop = await Dropdown(result);

			//Esto se hace para llenar los drowpdown al editar los registros.

			// Llenamos el ViewBag con los niveles de curso disponibles, basados en el identificador de nivel (Niv_Id) del resultado obtenido.
			// Se usa ContinueWith para procesar la lista devuelta por el método CursoNivelesDropdown y convertirla en una lista de SelectListItem.
			ViewBag.Niveles = await alumnosService.CursoNivelesDropdown(result.Niv_Id)
			  .ContinueWith(task => (task.Result ?? Enumerable.Empty<CursoNivelDropViewModel>()).Select(x => new SelectListItem
			  {
				  Value = x.Cun_Id.ToString(), // Asignamos el ID del curso como valor del dropdown.
				  Text = x.Cun_Descripcion // Asignamos la descripción del curso como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con las modalidades disponibles según el curso seleccionado (Cun_Id).
			ViewBag.Modalidades = await alumnosService.ModalidadesDropdown(result.Cun_Id)
			  .ContinueWith(task => (task.Result ?? Enumerable.Empty<ModalidadViewModel>()).Select(x => new SelectListItem
			  {
				  Value = x.Mda_Id.ToString(), // Asignamos el ID de la modalidad como valor del dropdown.
				  Text = x.Mda_Descripcion // Asignamos la descripción de la modalidad como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con los cursos disponibles según la modalidad seleccionada (Mda_Id).
			ViewBag.Cursos = await alumnosService.CursosDropdown(result.Mda_Id)
			  .ContinueWith(task => (task.Result ?? Enumerable.Empty<CursoViewModel>()).Select(x => new SelectListItem
			  {
				  Value = x.Cur_Id.ToString(), // Asignamos el ID del curso como valor del dropdown.
				  Text = x.Cur_Nombre // Asignamos el nombre del curso como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con las secciones disponibles según el curso seleccionado (Cur_Id).
			ViewBag.Secciones = await alumnosService.SeccionesDropdown(result.Cur_Id)
			  .ContinueWith(task => (task.Result ?? Enumerable.Empty<SeccionViewModel>()).Select(x => new SelectListItem
			  {
				  Value = x.Sec_Id.ToString(), // Asignamos el ID de la sección como valor del dropdown.
				  Text = x.Sec_Descripcion // Asignamos la descripción de la sección como texto visible en el dropdown.
			  }).ToList());



			return View("CreateAsync", drop);
		}

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
		[SessionManager("Crear alumnos")]
		public async Task<ActionResult> Guardar(AlumnoViewModel model)
		{
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
			}, JsonRequestBehavior.AllowGet);
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

		public async Task<ActionResult> GetAcademicoAsync(int id)
		{
			var data = await boletinService.GetAsync(id, System.DateTime.Now.Year);
			return PartialView("_TabAcademico", data);
		}

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
				return PartialView("_TabFamilia", data);
			}
			catch
			{
				return PartialView("_TabFamilia", (object)null);
			}
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

using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
	/// <summary>
	/// Controlador para gestionar la entidad Alumnos.
	/// </summary>
	public class AlumnosController : BaseController
	{
		private readonly AlumnosService alumnosService = new AlumnosService();

		/// <summary>
		/// Muestra la vista del índice.
		/// </summary>
		/// <returns>La vista del índice.</returns>
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Muestra la vista de creación de forma asíncrona.
		/// </summary>
		/// <returns>La vista de creación.</returns>
		public async Task<ActionResult> CreateAsync()
		{
			var model = new AlumnoViewModel();
			var drop = await Dropdown(model);
			return View(drop);
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
		public async Task<ActionResult> FindAsync(int id)
		{
			var result = await alumnosService.Find(id);
			var drop = await Dropdown(result);

			//Esto se hace para llenar los drowpdown al editar los registros.

			// Llenamos el ViewBag con los niveles de curso disponibles, basados en el identificador de nivel (NivelId) del resultado obtenido.
			// Se usa ContinueWith para procesar la lista devuelta por el método CursoNivelesDropdown y convertirla en una lista de SelectListItem.
			ViewBag.Niveles = await alumnosService.CursoNivelesDropdown(result.NivelId)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.CursoNivelId.ToString(), // Asignamos el ID del curso como valor del dropdown.
				  Text = x.DescripcionCursoNivel // Asignamos la descripción del curso como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con las modalidades disponibles según el curso seleccionado (CursoNivelId).
			ViewBag.Modalidades = await alumnosService.ModalidadesDropdown(result.CursoNivelId)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.ModalidadId.ToString(), // Asignamos el ID de la modalidad como valor del dropdown.
				  Text = x.DescripcionModalidad // Asignamos la descripción de la modalidad como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con los cursos disponibles según la modalidad seleccionada (ModalidadId).
			ViewBag.Cursos = await alumnosService.CursosDropdown(result.ModalidadId)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.CursoId.ToString(), // Asignamos el ID del curso como valor del dropdown.
				  Text = x.NombreCurso // Asignamos el nombre del curso como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con las secciones disponibles según el curso seleccionado (CursoId).
			ViewBag.Secciones = await alumnosService.SeccionesDropdown(result.CursoId)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.SeccionId.ToString(), // Asignamos el ID de la sección como valor del dropdown.
				  Text = x.DescripcionSeccion // Asignamos la descripción de la sección como texto visible en el dropdown.
			  }).ToList());



			return View("CreateAsync", drop);
		}

		/// <summary>
		/// Guarda los cambios realizados en un alumno.
		/// </summary>
		/// <param name="model">El modelo del alumno.</param>
		/// <returns>El resultado de la operación.</returns>
		[HttpPost]
		public async Task<ActionResult> Save(AlumnoViewModel model)
		{
			if (model.AlumnoId == 0)
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
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
			{
				Value = x.CursoNivelId.ToString(),
				Text = x.DescripcionCursoNivel
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
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
			{
				Value = x.ModalidadId.ToString(),
				Text = x.DescripcionModalidad
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
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
			{
				Value = x.CursoId.ToString(),
				Text = x.NombreCurso
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
			}
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
			{
				Value = x.SeccionId.ToString(),
				Text = x.DescripcionSeccion
			}).ToList();
			return AjaxResult(resultToSelectListItem);
		}

		/// <summary>
		/// Elimina un alumno de forma asíncrona.
		/// </summary>
		/// <param name="model">El modelo del alumno.</param>
		/// <returns>El resultado de la operación.</returns>
		[HttpPost]
		public async Task<ActionResult> DeleteAsync(AlumnoViewModel model)
		{
			bool result = await alumnosService.Delete(model.AlumnoId);

			// Validamos error
			if (result)
			{
				return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
			}
			return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
		}
	}
}


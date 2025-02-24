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

			// Llenamos el ViewBag con los niveles de curso disponibles, basados en el identificador de nivel (Niv_Id) del resultado obtenido.
			// Se usa ContinueWith para procesar la lista devuelta por el método CursoNivelesDropdown y convertirla en una lista de SelectListItem.
			ViewBag.Niveles = await alumnosService.CursoNivelesDropdown(result.Niv_Id)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.Cun_Id.ToString(), // Asignamos el ID del curso como valor del dropdown.
				  Text = x.Cun_Descripcion // Asignamos la descripción del curso como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con las modalidades disponibles según el curso seleccionado (Cun_Id).
			ViewBag.Modalidades = await alumnosService.ModalidadesDropdown(result.Cun_Id)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.Mda_Id.ToString(), // Asignamos el ID de la modalidad como valor del dropdown.
				  Text = x.Mda_Descripcion // Asignamos la descripción de la modalidad como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con los cursos disponibles según la modalidad seleccionada (Mda_Id).
			ViewBag.Cursos = await alumnosService.CursosDropdown(result.Mda_Id)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.Cur_Id.ToString(), // Asignamos el ID del curso como valor del dropdown.
				  Text = x.Cur_Nombre // Asignamos el nombre del curso como texto visible en el dropdown.
			  }).ToList());

			// Llenamos el ViewBag con las secciones disponibles según el curso seleccionado (Cur_Id).
			ViewBag.Secciones = await alumnosService.SeccionesDropdown(result.Cur_Id)
			  .ContinueWith(task => task.Result.Select(x => new SelectListItem
			  {
				  Value = x.Sec_Id.ToString(), // Asignamos el ID de la sección como valor del dropdown.
				  Text = x.Sec_Descripcion // Asignamos la descripción de la sección como texto visible en el dropdown.
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
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
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
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
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
			IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
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
		public async Task<ActionResult> DeleteAsync(AlumnoViewModel model)
		{
			bool result = await alumnosService.Delete(model.Alu_Id);

			// Validamos error
			if (result)
			{
				return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
			}
			return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
		}
	}
}

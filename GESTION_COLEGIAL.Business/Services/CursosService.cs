using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Services
{
	/// <summary>
	/// Servicio para gestionar los cursos.
	/// </summary>
	public class CursosService
	{
		/// <summary>
		/// Obtiene una lista de los cursos de forma asincrónica.
		/// </summary>
		/// <returns>Una colección de objetos CursoViewModel.</returns>
		public async Task<IEnumerable<CursoViewModel>> ListAsync()
		{
			string url = "Cursos/ListAsync";
			IEnumerable<CursoViewModel> apiUrl = await ApiRequests.ListAsync<CursoViewModel>(url);
			return apiUrl;
		}

		/// <summary>
		/// Busca un curso por su identificador de forma asincrónica.
		/// </summary>
		/// <param name="id">El identificador del curso.</param>
		/// <returns>El objeto CursoViewModel encontrado.</returns>
		public async Task<CursoViewModel> Find(int id)
		{
			// Url de petición
			string url = "Cursos/FindAsync";
			string urlModalidades = "Cursos/CursosModalidadesFind";
			string urlMaterias = "Cursos/CursosMateriasFind";
			string urlCursosNiveles = "Cursos/CursosNivelesFind";
			string urlSecciones = "Cursos/CursosSeccionesFind";
			// Envío y recepción de la petición
			var result = await ApiRequests.FindAsync<CursoViewModel>(url, id);
			var modalidades = await ApiRequests.FindAllAsync<ModalidadViewModel>(urlModalidades, id);
			var secciones = await ApiRequests.FindAllAsync<SeccionViewModel>(urlSecciones, id);
			var cursosniveles = await ApiRequests.FindAllAsync<CursoNivelViewModel>(urlCursosNiveles, id);
			var materias = await ApiRequests.FindAllAsync<MateriaViewModel>(urlMaterias, id);

			var load = await Load(result);

			load.ModalidadesCheckList = load.ModalidadesCheckList.Select(x => new SelectListItem()
			{
				Text = x.Text,
				Value = x.Value.ToString(),
				Selected = modalidades.Any(y => y.Mda_Id.ToString() == x.Value.ToString()) ? true : false
			}).ToList();

			load.SeccionesCheckList = load.SeccionesCheckList.Select(x => new SelectListItem()
			{
				Text = x.Text,
				Value = x.Value.ToString(),
				Selected = secciones.Any(y => y.Sec_Id.ToString() == x.Value.ToString()) ? true : false
			}).ToList();

			load.CursoNivelesCheckList = load.CursoNivelesCheckList.Select(x => new SelectListItem()
			{
				Text = x.Text,
				Value = x.Value.ToString(),
				Selected = cursosniveles.Any(y => y.Cun_Id.ToString() == x.Value.ToString()) ? true : false
			}).ToList();

			load.MateriasCheckList = load.MateriasCheckList.Select(x => new SelectListItem()
			{
				Text = x.Text,
				Value = x.Value.ToString(),
				Selected = materias.Any(y => y.Mat_Id.ToString() == x.Value.ToString()) ? true : false
			}).ToList();
			return load;
		}

		/// <summary>
		/// Crea un nuevo curso de forma asincrónica.
		/// </summary>
		/// <param name="model">El objeto CursoViewModel a crear.</param>
		/// <returns>true si la creación fue exitosa, de lo contrario false.</returns>
		public async Task<Boolean> Create(CursoViewModel model)
		{
			model.Modalidades = model.ModalidadesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
			model.CursoNiveles = model.CursoNivelesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
			model.Materias = model.MateriasCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
			model.Secciones = model.SeccionesCheckList.Where(x => x.Selected == true).Select(x => Convert.ToInt32(x.Value)).ToArray();
			model.SeccionesCheckList = null;
			model.ModalidadesCheckList = null;
			model.CursoNivelesCheckList = null;
			model.MateriasCheckList = null;
			// Direcciones.
			string url = "Cursos/CreateAsync";
			string urlModalidades = "Cursos/CursosModalidadesCreate";
			string urlCursosNiveles = "Cursos/CursosNivelesCreate";
			string urlMaterias = "Cursos/CursosMateriasCreate";
			string urlSecciones = "Cursos/CursosSeccionesCreate";
			// Envío y recepción de la petición
			bool result = await ApiRequests.CreateAsync(url, model);
			bool resultModalidades = await ApiRequests.CreateAsync(urlModalidades, model);
			bool resultCursosNiveles = await ApiRequests.CreateAsync(urlCursosNiveles, model);
			bool resultMaterias = await ApiRequests.CreateAsync(urlMaterias, model);
			bool resultSecciones = await ApiRequests.CreateAsync(urlSecciones, model);
			// Comprobando el resultado de las peticiones.
			Boolean[] results = { result, resultModalidades, resultCursosNiveles, resultMaterias, resultSecciones };
			return ValidationResults.BooleanListIsTrue(results);
		}

		/// <summary>
		/// Edita un curso existente de forma asincrónica.
		/// </summary>
		/// <param name="model">El objeto CursoViewModel a editar.</param>
		/// <returns>true si la edición fue exitosa, de lo contrario false.</returns>
		public async Task<Boolean> Edit(CursoViewModel model)
		{
			// Direcciones.
			string url = "Cursos/EditAsync";
			string urlModalidades = "Cursos/CursosModalidadesEdit";
			string urlCursosNiveles = "Cursos/CursosNivelesEdit";
			string urlMaterias = "Cursos/CursosMateriasEdit";
			string urlSecciones = "Cursos/CursosSeccionesEdit";
			// Envío y recepción de la petición
			bool result = await ApiRequests.EditAsync(url, model);
			bool resultModalidades = await ApiRequests.EditAsync(urlModalidades, model);
			bool resultCursosNiveles = await ApiRequests.EditAsync(urlCursosNiveles, model);
			bool resultMaterias = await ApiRequests.EditAsync(urlMaterias, model);
			bool resultSecciones = await ApiRequests.EditAsync(urlSecciones, model);
			// Comprobando el resultado de las peticiones.
			Boolean[] results = { result, resultModalidades, resultCursosNiveles, resultMaterias, resultSecciones };
			return ValidationResults.BooleanListIsTrue(results);
		}

		/// <summary>
		/// Verifica si un curso existe de forma asincrónica.
		/// </summary>
		/// <param name="value">El valor a verificar.</param>
		/// <returns>El objeto CursoViewModel si existe, de lo contrario null.</returns>
		public async Task<CursoViewModel> Exist(string value)
		{
			string url = "Cursos/ExistAsync";
			return await ApiRequests.ExistAsync<CursoViewModel>(url, value);
		}

		/// <summary>
		/// Elimina un curso por su identificador de forma asincrónica.
		/// </summary>
		/// <param name="id">El identificador del curso a eliminar.</param>
		/// <returns>true si la eliminación fue exitosa, de lo contrario false.</returns>
		public async Task<Boolean> Delete(int id)
		{
			string url = "Cursos/RemoveAsync";
			return await ApiRequests.DeleteAsync(url, id);
		}

		/// <summary>
		/// Carga datos adicionales en un objeto CursoViewModel de forma asincrónica.
		/// </summary>
		/// <param name="model">El objeto CursoViewModel a cargar.</param>
		/// <returns>El objeto CursoViewModel con los datos adicionales cargados.</returns>
		public async Task<CursoViewModel> Load(CursoViewModel model)
		{
			// Direcciones.
			string urlNivelesEducativos = "Cursos/NivelesEducativosDropdown";
			string urlModalidades = "Cursos/ModalidadesList";
			string urlMaterias = "Cursos/MateriasList";
			string urlCursosNiveles = "Cursos/CursosNivelesList";
			string urlSecciones = "Cursos/SeccionesList";
			// Instancias.
			var nivelesEducativosDropdown = await ApiRequests.DropdownAsync<NivelEducativoViewModel>(urlNivelesEducativos);
			var modalidadesList = await ApiRequests.CheckListAsync<ModalidadViewModel>(urlModalidades);
			var materiasList = await ApiRequests.CheckListAsync<MateriaViewModel>(urlMaterias);
			var cursosNivelesList = await ApiRequests.CheckListAsync<CursoNivelViewModel>(urlCursosNiveles);
			var seccionesList = await ApiRequests.CheckListAsync<SeccionViewModel>(urlSecciones);
			// Cargando en el modelo.
			model.LoadDropDownList(nivelesEducativosDropdown);
			model.LoadCheckList(modalidadesList, seccionesList, cursosNivelesList, materiasList);
			return model;
		}
	}
}
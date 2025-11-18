using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que representa el servicio de alumnos.
    /// </summary>
    public class AlumnosService
    {
        /// <summary>
        /// Obtiene una lista de alumnos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de alumnos.</returns>
        public async Task<IEnumerable<AlumnoViewModel>> ListAsync()
        {
            string url = "Alumnos/ListAsync";
            IEnumerable<AlumnoViewModel> apiUrl = await ApiRequests.ListAsync<AlumnoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un alumno por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del alumno.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el alumno encontrado.</returns>
        public async Task<AlumnoViewModel> Find(int id)
        {
            string url = "Alumnos/FindAsync";
            AlumnoViewModel apiUrl = await ApiRequests.FindAsync<AlumnoViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Busca un alumno por su número de identidad de forma asíncrona.
        /// </summary>
        /// <param name="identidad">El número de identidad del alumno.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el alumno encontrado.</returns>
        public async Task<AlumnoViewModel> FindByIdentidad(string identidad)
        {
            string url = "Alumnos/FindByIdentidadAsync";
            AlumnoViewModel result = await ApiRequests.ExistAsync<AlumnoViewModel>(url, identidad);
            return result;
        }

        /// <summary>
        /// Crea un nuevo alumno de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de alumno a crear.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se creó el alumno correctamente.</returns>
        public async Task<Boolean> Create(AlumnoViewModel model)
        {
            string url = "Alumnos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un alumno existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de alumno a editar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se editó el alumno correctamente.</returns>
        public async Task<Boolean> Edit(AlumnoViewModel model)
        {
            string url = "Alumnos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si existe un alumno con el valor especificado de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el alumno encontrado.</returns>
        public async Task<AlumnoViewModel> Exist(string value)
        {
            string url = "Alumnos/ExistAsync";
            return await ApiRequests.ExistAsync<AlumnoViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un alumno por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del alumno a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado indica si se eliminó el alumno correctamente.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Alumnos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        /// <summary>
        /// Obtiene un modelo de alumno con las listas desplegables cargadas de forma asíncrona.
        /// </summary>
        /// <param name="model">El modelo de alumno.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el modelo de alumno con las listas desplegables cargadas.</returns>
        public async Task<AlumnoViewModel> Dropdown(AlumnoViewModel model)
        {
            string urlNivelesEducativos = "Alumnos/NivelesEducativosDropdown";
            string urlEstados = "Alumnos/EstadosDropdown";
            var nivelesEducativosDropdown = await ApiRequests.DropdownAsync<NivelEducativoViewModel>(urlNivelesEducativos);
            var estadosDropdown = await ApiRequests.DropdownAsync<EstadoViewModel>(urlEstados);
            model.LoadDropDownList(nivelesEducativosDropdown, estadosDropdown);
            return model;
        }

        /// <summary>
        /// Obtiene una lista de cursos y niveles desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de cursos y niveles desplegables.</returns>
        public async Task<IEnumerable<CursoNivelDropViewModel>> CursoNivelesDropdown(int id)
        {
            string urlCursosNiveles = "Alumnos/CursosNivelesDropdown";
            var cursosNivelesDropdown = await ApiRequests.DropdownAsync<CursoNivelDropViewModel>(urlCursosNiveles, id);
            return cursosNivelesDropdown;
        }

        /// <summary>
        /// Obtiene una lista de modalidades desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de modalidades desplegables.</returns>
        public async Task<IEnumerable<ModalidadViewModel>> ModalidadesDropdown(int id)
        {
            string urlModalidades = "Alumnos/ModalidadesDropdown";
            var modalidadesDropdown = await ApiRequests.DropdownAsync<ModalidadViewModel>(urlModalidades, id);
            return modalidadesDropdown;
        }

        /// <summary>
        /// Obtiene una lista de cursos desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de cursos desplegables.</returns>
        public async Task<IEnumerable<CursoViewModel>> CursosDropdown(int id)
        {
            string urlCursos = "Alumnos/CursosDropdown";
            var cursosDropdown = await ApiRequests.DropdownAsync<CursoViewModel>(urlCursos, id);
            return cursosDropdown;
        }

        /// <summary>
        /// Obtiene una lista de secciones desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de secciones desplegables.</returns>
        public async Task<IEnumerable<SeccionViewModel>> SeccionesDropdown(int id)
        {
            string urlSecciones = "Alumnos/SeccionesDropdown";
            var seccionesDropdown = await ApiRequests.DropdownAsync<SeccionViewModel>(urlSecciones, id);
            return seccionesDropdown;
        }
    }
}
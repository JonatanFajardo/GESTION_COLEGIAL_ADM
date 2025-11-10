using GESTION_COLEGIAL.Business.DTOs;
using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que representa el servicio de alumnos.
    /// </summary>
    public class AlumnosService : BaseService
    {
        /// <summary>
        /// Obtiene una lista de alumnos de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de alumnos.</returns>
        public async Task<IEnumerable<AlumnoViewModel>> ListAsync()
        {
            const string url = "Alumnos/ListAsync";
            var dtos = await ApiRequests.ListAsync<AlumnoListDto>(url);
            return Map<IEnumerable<AlumnoListDto>, IEnumerable<AlumnoViewModel>>(dtos);
        }

        /// <summary>
        /// Busca un alumno por su ID de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID del alumno.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene el alumno encontrado.</returns>
        public async Task<AlumnoViewModel> Find(int id)
        {
            const string url = "Alumnos/FindAsync";
            var dto = await ApiRequests.FindAsync<AlumnoFindDto>(url, id);
            return Map<AlumnoFindDto, AlumnoViewModel>(dto);
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
            const string url = "Alumnos/ExistAsync";
            var dto = await ApiRequests.ExistAsync<AlumnoFindDto>(url, value);
            return Map<AlumnoFindDto, AlumnoViewModel>(dto);
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
            const string urlNivelesEducativos = "Alumnos/NivelesEducativosDropdown";
            const string urlEstados = "Alumnos/EstadosDropdown";
            var nivelesEducativosDropdown = await ApiRequests.DropdownAsync<NivelEducativoDropdownDto>(urlNivelesEducativos);
            var estadosDropdown = await ApiRequests.DropdownAsync<EstadoDropdownDto>(urlEstados);
            var niveles = Map<IEnumerable<NivelEducativoDropdownDto>, IEnumerable<NivelEducativoViewModel>>(nivelesEducativosDropdown);
            var estados = Map<IEnumerable<EstadoDropdownDto>, IEnumerable<EstadoViewModel>>(estadosDropdown);
            model.LoadDropDownList(niveles, estados);
            return model;
        }

        /// <summary>
        /// Obtiene una lista de cursos y niveles desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de cursos y niveles desplegables.</returns>
        public async Task<IEnumerable<CursoNivelDropViewModel>> CursoNivelesDropdown(int id)
        {
            const string urlCursosNiveles = "Alumnos/CursosNivelesDropdown";
            var cursosNivelesDropdown = await ApiRequests.DropdownAsync<CursoNivelDropdownDto>(urlCursosNiveles, id);
            return Map<IEnumerable<CursoNivelDropdownDto>, IEnumerable<CursoNivelDropViewModel>>(cursosNivelesDropdown);
        }

        /// <summary>
        /// Obtiene una lista de modalidades desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de modalidades desplegables.</returns>
        public async Task<IEnumerable<ModalidadViewModel>> ModalidadesDropdown(int id)
        {
            const string urlModalidades = "Alumnos/ModalidadesDropdown";
            var modalidadesDropdown = await ApiRequests.DropdownAsync<ModalidadDropdownDto>(urlModalidades, id);
            return Map<IEnumerable<ModalidadDropdownDto>, IEnumerable<ModalidadViewModel>>(modalidadesDropdown);
        }

        /// <summary>
        /// Obtiene una lista de cursos desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de cursos desplegables.</returns>
        public async Task<IEnumerable<CursoViewModel>> CursosDropdown(int id)
        {
            const string urlCursos = "Alumnos/CursosDropdown";
            var cursosDropdown = await ApiRequests.DropdownAsync<CursoDropdownDto>(urlCursos, id);
            return Map<IEnumerable<CursoDropdownDto>, IEnumerable<CursoViewModel>>(cursosDropdown);
        }

        /// <summary>
        /// Obtiene una lista de secciones desplegables para un ID específico de forma asíncrona.
        /// </summary>
        /// <param name="id">El ID de referencia.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de secciones desplegables.</returns>
        public async Task<IEnumerable<SeccionViewModel>> SeccionesDropdown(int id)
        {
            const string urlSecciones = "Alumnos/SeccionesDropdown";
            var seccionesDropdown = await ApiRequests.DropdownAsync<SeccionDropdownDto>(urlSecciones, id);
            return Map<IEnumerable<SeccionDropdownDto>, IEnumerable<SeccionViewModel>>(seccionesDropdown);
        }
    }
}
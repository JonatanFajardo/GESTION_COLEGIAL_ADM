using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con el horario de los alumnos.
    /// </summary>
    public class HorarioAlumnosService
    {
        /// <summary>
        /// Obtiene una lista de horarios de los alumnos de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos HorarioAlumnoViewModel.</returns>
        public async Task<IEnumerable<HorarioAlumnoViewModel>> ListAsync()
        {
            string url = "HorarioAlumnos/ListAsync";
            IEnumerable<HorarioAlumnoViewModel> apiUrl = await ApiRequests.ListAsync<HorarioAlumnoViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un horario de alumno por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del horario de alumno.</param>
        /// <returns>El objeto HorarioAlumnoViewModel encontrado.</returns>
        public async Task<HorarioAlumnoViewModel> Find(int id)
        {
            string url = "HorarioAlumnos/FindAsync";
            HorarioAlumnoViewModel apiUrl = await ApiRequests.FindAsync<HorarioAlumnoViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo horario de alumno de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto HorarioAlumnoViewModel a crear.</param>
        /// <returns>True si el horario de alumno se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(HorarioAlumnoViewModel model)
        {
            string url = "HorarioAlumnos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un horario de alumno existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto HorarioAlumnoViewModel a editar.</param>
        /// <returns>True si el horario de alumno se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(HorarioAlumnoViewModel model)
        {
            string url = "HorarioAlumnos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de horario de alumno existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de horario de alumno a verificar.</param>
        /// <returns>El objeto HorarioAlumnoViewModel si existe, de lo contrario null.</returns>
        public async Task<HorarioAlumnoViewModel> Exist(string value)
        {
            string url = "HorarioAlumnos/ExistAsync";
            return await ApiRequests.ExistAsync<HorarioAlumnoViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un horario de alumno por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del horario de alumno a eliminar.</param>
        /// <returns>True si el horario de alumno se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "HorarioAlumnos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
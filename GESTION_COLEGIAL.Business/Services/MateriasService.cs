using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con las materias.
    /// </summary>
    public class MateriasService
    {
        /// <summary>
        /// Obtiene una lista de materias de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos MateriaViewModel.</returns>
        public async Task<IEnumerable<MateriaViewModel>> ListAsync()
        {
            string url = "Materias/ListAsync";
            IEnumerable<MateriaViewModel> apiUrl = await ApiRequests.ListAsync<MateriaViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca una materia por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la materia.</param>
        /// <returns>El objeto MateriaViewModel encontrado.</returns>
        public async Task<MateriaViewModel> Find(int id)
        {
            string url = "Materias/FindAsync";
            MateriaViewModel apiUrl = await ApiRequests.FindAsync<MateriaViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva materia de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto MateriaViewModel a crear.</param>
        /// <returns>True si la materia se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(MateriaViewModel model)
        {
            string url = "Materias/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una materia existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto MateriaViewModel a editar.</param>
        /// <returns>True si la materia se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(MateriaViewModel model)
        {
            string url = "Materias/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de materia existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de materia a verificar.</param>
        /// <returns>El objeto MateriaViewModel si existe, de lo contrario null.</returns>
        public async Task<MateriaViewModel> Exist(string value)
        {
            string url = "Materias/ExistAsync";
            return await ApiRequests.ExistAsync<MateriaViewModel>(url, value);
        }

        /// <summary>
        /// Elimina una materia por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la materia a eliminar.</param>
        /// <returns>True si la materia se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Materias/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
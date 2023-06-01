using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio para gestionar los cursos y niveles.
    /// </summary>
    public class CursosNivelesService
    {
        /// <summary>
        /// Obtiene una lista de los cursos y niveles de forma asincrónica.
        /// </summary>
        /// <returns>Una colección de objetos CursoNivelViewModel.</returns>
        public async Task<IEnumerable<CursoNivelViewModel>> ListAsync()
        {
            string url = "CursosNiveles/ListAsync";
            IEnumerable<CursoNivelViewModel> apiUrl = await ApiRequests.ListAsync<CursoNivelViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un curso y nivel por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del curso y nivel.</param>
        /// <returns>El objeto CursoNivelViewModel encontrado.</returns>
        public async Task<CursoNivelViewModel> Find(int id)
        {
            string url = "CursosNiveles/FindAsync";
            CursoNivelViewModel apiUrl = await ApiRequests.FindAsync<CursoNivelViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo curso y nivel de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto CursoNivelViewModel a crear.</param>
        /// <returns>True si se creó correctamente, False en caso contrario.</returns>
        public async Task<bool> Create(CursoNivelViewModel model)
        {
            string url = "CursosNiveles/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un curso y nivel existente de forma asincrónica.
        /// </summary>
        /// <param name="model">El objeto CursoNivelViewModel a editar.</param>
        /// <returns>True si se editó correctamente, False en caso contrario.</returns>
        public async Task<bool> Edit(CursoNivelViewModel model)
        {
            string url = "CursosNiveles/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si existe un curso y nivel con el valor especificado de forma asincrónica.
        /// </summary>
        /// <param name="value">El valor a verificar.</param>
        /// <returns>El objeto CursoNivelViewModel si existe, o null si no existe.</returns>
        public async Task<CursoNivelViewModel> Exist(string value)
        {
            string url = "CursosNiveles/ExistAsync";
            return await ApiRequests.ExistAsync<CursoNivelViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un curso y nivel por su identificador de forma asincrónica.
        /// </summary>
        /// <param name="id">El identificador del curso y nivel a eliminar.</param>
        /// <returns>True si se eliminó correctamente, False en caso contrario.</returns>
        public async Task<bool> Delete(int id)
        {
            string url = "CursosNiveles/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }
    }
}
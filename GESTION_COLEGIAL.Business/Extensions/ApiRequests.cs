using GESTION_COLEGIAL.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Extensions
{
    /// <summary>
    /// Clase que proporciona métodos para realizar solicitudes a una API de forma sincrónica.
    /// </summary>
    internal class ApiRequests
    {
        #region Sincrono

        /// <summary>
        /// Obtiene una lista de elementos de tipo T desde la API.
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a obtener.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <returns>Una colección de elementos de tipo T.</returns>
        public static async Task<IEnumerable<T>> ListAsync<T>(string url)
        {
            // Crea la petición a la API
            return await SendHttpClient.GetAsync<T>(url);
        }

        /// <summary>
        /// Crea un elemento en la API.
        /// </summary>
        /// <param name="url">La URL de la API.</param>
        /// <param name="model">El modelo de datos del elemento a crear.</param>
        /// <returns>true si el elemento se creó correctamente, false en caso contrario.</returns>
        public static async Task<Boolean> CreateAsync(string url, dynamic model)
        {
            // Crea la petición a la API
            return await SendHttpClient.PostAsync(url, model);
        }

        /// <summary>
        /// Envía datos a la API mediante POST y retorna una respuesta tipada.
        /// </summary>
        /// <typeparam name="T">El tipo de respuesta esperada.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <param name="model">El modelo de datos a enviar.</param>
        /// <returns>La respuesta de tipo T de la API.</returns>
        public static async Task<T> PostAsyncWithResponse<T>(string url, dynamic model)
        {
            return await SendHttpClient.PostAsyncWithResponse<T>(url, model);
        }

        /// <summary>
        /// Envía datos a la API mediante POST.
        /// </summary>
        /// <param name="url">La URL de la API.</param>
        /// <param name="model">El modelo de datos a enviar.</param>
        /// <returns>true si hubo error, false si fue exitoso.</returns>
        public static async Task<Boolean> PostAsync(string url, dynamic model)
        {
            return await SendHttpClient.PostAsync(url, model);
        }

        /// <summary>
        /// Actualiza un elemento en la API.
        /// </summary>
        /// <param name="url">La URL de la API.</param>
        /// <param name="model">El modelo de datos del elemento a actualizar.</param>
        /// <returns>true si el elemento se actualizó correctamente, false en caso contrario.</returns>
        public static async Task<Boolean> EditAsync(string url, dynamic model)
        {
            // Crea la petición a la API
            return await SendHttpClient.PutAsync(url, model);
        }

        /// <summary>
        /// Busca un elemento de tipo T en la API por su identificador.
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a buscar.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <param name="id">El identificador del elemento.</param>
        /// <returns>El elemento de tipo T encontrado, o null si no se encontró.</returns>
        public static async Task<T> FindAsync<T>(string url, int id)
        {
            return await SendHttpClient.FindAsync<T>(url, id);
        }

        /// <summary>
        /// Obtiene un único elemento de tipo T desde la API usando la URL completa con query params.
        /// Deserializa directamente como objeto único (no como lista).
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a obtener.</typeparam>
        /// <param name="url">La URL completa de la API con query params (ej: "Controller/Action?param=value").</param>
        /// <returns>El objeto de tipo T encontrado, o null si no se encontró.</returns>
        public static async Task<T> GetSingleAsync<T>(string url)
        {
            return await SendHttpClient.GetSingleAsync<T>(url);
        }

        /// <summary>
        /// Busca todos los elementos de tipo T relacionados a un identificador en la API.
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a buscar.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <param name="id">El identificador relacionado a los elementos.</param>
        /// <returns>Una colección de elementos de tipo T.</returns>
        public static async Task<IEnumerable<T>> FindAllAsync<T>(string url, int id)
        {
            // Crea la petición a la API
            return await SendHttpClient.FindAllAsync<T>(url, id);
        }

        /// <summary>
        /// Comprueba si un elemento de tipo T existe en la API según una evaluación específica.
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a evaluar.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <param name="evaluar">La evaluación específica para comprobar la existencia del elemento.</param>
        /// <returns>El elemento de tipo T si existe, o el valor predeterminado de T si no existe.</returns>
        public static async Task<T> ExistAsync<T>(string url, string evaluar)
        {
            // Crea la petición a la API
            var result = await SendHttpClient.ExistAsync<T>(url, evaluar);

            // Comprueba el estado de la solicitud
            //if: Indica que el resultado no contiene nada.
            //else: Retorna los valores solicitados.
            if (result == null)
            {
                return default;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Obtiene un listado de registros desde la API.
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a obtener.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <returns>Una colección de elementos de tipo T.</returns>
        public static async Task<IEnumerable<T>> DropdownAsync<T>(string url)
        {
            // Crea la petición a la API
            return await SendHttpClient.GetAsync<T>(url);
        }

        /// <summary>
        /// Obtiene un listado de registros desde la API relacionados a un identificador.
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a obtener.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <param name="id">El identificador relacionado a los elementos.</param>
        /// <returns>Una colección de elementos de tipo T.</returns>
        public static async Task<IEnumerable<T>> DropdownAsync<T>(string url, int id)
        {
            // Crea la petición a la API
            return await SendHttpClient.DropdownAsync<T>(url, id);
        }

        /// <summary>
        /// Obtiene un listado de registros desde la API y los devuelve como una lista.
        /// </summary>
        /// <typeparam name="T">El tipo de elemento a obtener.</typeparam>
        /// <param name="url">La URL de la API.</param>
        /// <returns>Una lista de elementos de tipo T.</returns>
        public static async Task<List<T>> CheckListAsync<T>(string url)
        {
            // Crea la petición a la API
            return await SendHttpClient.GetAsync<T>(url);
        }

        /// <summary>
        /// Elimina un elemento de la API por su identificador.
        /// </summary>
        /// <param name="url">La URL de la API.</param>
        /// <param name="id">El identificador del elemento a eliminar.</param>
        /// <returns>true si el elemento se eliminó correctamente, false en caso contrario.</returns>
        public static async Task<Boolean> DeleteAsync(string url, int id)
        {
            // Crea la petición a la API
            return await SendHttpClient.DeleteAsync(url, id);
        }

        #endregion Sincrono
    }
}

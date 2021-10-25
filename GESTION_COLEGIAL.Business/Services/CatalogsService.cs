using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class CatalogsService
    {
        public static async Task<IEnumerable<T>> List<T>(string url)
        {
            // Crea la peticion a la api
            var resultSerialize = await SendHttpClient.Get<T>(url);
            return resultSerialize;
        }

        public static async Task<bool> Create(string url, dynamic model)
        {
            // Crea la peticion a la api
            bool result = await SendHttpClient.Post(url, model);
            return result;
        }

        public static async Task<bool> Edit(string url, dynamic model)
        {
            // Crea la peticion a la api
            bool result = await SendHttpClient.Put(url, model);

            //if: Indica que el valor se edito correctamente.
            //else: Indica que el valor no se edito correctamente.
            return result;
            //if (!result)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }

        public static async Task<T> Find<T>(string url, int id)
        {
            var result = await SendHttpClient.Find<T>(url, id);
            return result;
        }

        public static async Task<T> Exist<T>(string url, string evaluar)
        {
            // Crea la peticion a la api
            var result = await SendHttpClient.Exist<T>(url, evaluar);

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
        /// Obtiene un listado de registros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> Dropdown<T>(string url)
        {
            // Crea la peticion a la api
            var resultSerialize = await SendHttpClient.Get<T>(url);
            return resultSerialize;
        }

        public static async Task<List<T>> CheckList<T>(string url)
        {
            // Crea la peticion a la api
            var resultSerialize = await SendHttpClient.Get<T>(url);
            return resultSerialize;
        }
        public static async Task<bool> Delete(string url, int id)
        {
            // Crea la peticion a la api
            bool result = await SendHttpClient.Delete(url, id);
            return result;
        }
    }
}
using GESTION_COLEGIAL.Business.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Extensions
{
    internal class ApiRequests
    {
        //#region Asincrono
        //public static IEnumerable<T> List<T>(string url)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.Get<T>(url);
        //}

        //public static Boolean Create(string url, dynamic model)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.Post(url, model);
        //}

        //public static Boolean Edit(string url, dynamic model)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.Put(url, model);

        //    //if: Indica que el valor se edito correctamente.
        //    //else: Indica que el valor no se edito correctamente.

        //    //if (!result)
        //    //{
        //    //    return false;
        //    //}
        //    //else
        //    //{
        //    //    return true;
        //    //}
        //}

        //public static T Find<T>(string url, int id)
        //{
        //    return await SendHttpClient.Find<T>(url, id);
        //}

        //public static IEnumerable<T> FindAll<T>(string url, int id)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.FindAll<T>(url, id);
        //    //return resultSerialize;
        //}

        //public static T Exist<T>(string url, string evaluar)
        //{
        //    // Crea la peticion a la api
        //    var result = await SendHttpClient.Exist<T>(url, evaluar);

        //    // Comprueba el estado de la solicitud
        //    //if: Indica que el resultado no contiene nada.
        //    //else: Retorna los valores solicitados.
        //    if (result == null)
        //    {
        //        return default;
        //    }
        //    else
        //    {
        //        return result;
        //    }
        //}

        ///// <summary>
        ///// Obtiene un listado de registros.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //public static IEnumerable<T> Dropdown<T>(string url)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.Get<T>(url);
        //}

        //public static IEnumerable<T> Dropdown<T>(string url, int id)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.Dropdown<T>(url, id);
        //}

        //public static List<T> CheckList<T>(string url)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.Get<T>(url);
        //}
        //public static Boolean Delete(string url, int id)
        //{
        //    // Crea la peticion a la api
        //    return await SendHttpClient.Delete(url, id);
        //}
        //#endregion

        #region Sincrono

        public static async Task<IEnumerable<T>> ListAsync<T>(string url)
        {
            // Crea la peticion a la api
            return await SendHttpClient.GetAsync<T>(url);
        }

        public static async Task<Boolean> CreateAsync(string url, dynamic model)
        {
            // Crea la peticion a la api
            return await SendHttpClient.PostAsync(url, model);
        }

        public static async Task<Boolean> EditAsync(string url, dynamic model)
        {
            // Crea la peticion a la api
            return await SendHttpClient.PutAsync(url, model);

            //if: Indica que el valor se edito correctamente.
            //else: Indica que el valor no se edito correctamente.

            //if (!result)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
        }

        public static async Task<T> FindAsync<T>(string url, int id)
        {
            return await SendHttpClient.FindAsync<T>(url, id);
        }

        public static async Task<IEnumerable<T>> FindAllAsync<T>(string url, int id)
        {
            // Crea la peticion a la api
            return await SendHttpClient.FindAllAsync<T>(url, id);
            //return resultSerialize;
        }

        public static async Task<T> ExistAsync<T>(string url, string evaluar)
        {
            // Crea la peticion a la api
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
        /// Obtiene un listado de registros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> DropdownAsync<T>(string url)
        {
            // Crea la peticion a la api
            return await SendHttpClient.GetAsync<T>(url);
        }

        public static async Task<IEnumerable<T>> DropdownAsync<T>(string url, int id)
        {
            // Crea la peticion a la api
            return await SendHttpClient.DropdownAsync<T>(url, id);
        }

        public static async Task<List<T>> CheckListAsync<T>(string url)
        {
            // Crea la peticion a la api
            return await SendHttpClient.GetAsync<T>(url);
        }

        public static async Task<Boolean> DeleteAsync(string url, int id)
        {
            // Crea la peticion a la api
            return await SendHttpClient.DeleteAsync(url, id);
        }

        #endregion Sincrono
    }
}
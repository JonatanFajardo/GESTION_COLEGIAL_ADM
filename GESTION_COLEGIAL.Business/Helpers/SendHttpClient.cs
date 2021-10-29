using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Helpers
{
    /// <summary>
    /// Crea peticiones a un servicio web.
    /// </summary>
    public static class SendHttpClient
    {
        private const string baseUrl = "https://localhost:44341/api/";

        /// <summary>
        /// Obtiene valores del servicio.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<List<T>> Get<T>(string url/*, object model*/)
        {
            try
            {
                var httpclient = new HttpClient();
                var httpResponse = await httpclient.GetAsync($"{baseUrl}{url}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return null;
                }
                var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
                var resultSerialize = JsonConvert.DeserializeObject<List<T>>(content);
                return resultSerialize;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        /// <summary>
        /// Envia datos a una API.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<bool> Post(string url, object model)
        {
            var httpclient = new HttpClient();
            var content = JsonConvert.SerializeObject(model);//se convierte a json el contenido a enviar
            var contentSerialized = new StringContent(content, Encoding.Default, "application/json");//Agregamos informacion adicional al json
            var httpResponse = await httpclient.PostAsync($"{baseUrl}{url}", contentSerialized);//
            //httpResponse.Wait();

            //var postJob = httpResponse.Result;
            if (!httpResponse.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Solicita la eliminacion de un registro.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id">Identificador del registro a eliminar.</param>
        /// <returns></returns>
        public static async Task<bool> Delete(string url, int id)
        {
            var httpclient = new HttpClient();
            var content = JsonConvert.SerializeObject(id);//se convierte a json el contenido a enviar
            var contentSerialized = new StringContent(content, Encoding.Default, "application/json");//Agregamos informacion adicional al json
            var httpResponse = await httpclient.PutAsync($"{baseUrl}{url}?value={id}", contentSerialized);//
            //httpResponse.Wait();

            //var postJob = httpResponse.Result;
            if (!httpResponse.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Solicita la modificacion de un registro.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="model">Entidad que encapsula los datos a actualizar.</param>
        /// <returns></returns>
        public static async Task<bool> Put(string url, object model)
        {
            var httpclient = new HttpClient();
            var content = JsonConvert.SerializeObject(model);//se convierte a json el contenido a enviar
            var contentSerialized = new StringContent(content, Encoding.Default, "application/json");//Agregamos informacion adicional al json
            var httpResponse = await httpclient.PutAsync($"{baseUrl}{url}", contentSerialized);
            //httpResponse.Wait();

            //var postJob = httpResponse.Result;
            if (!httpResponse.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Solicita la verificacion de la existencia de un registro.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<T> Exist<T>(string url, string value)
        {
            try
            {
                string direction = $"{baseUrl}{url}?value={value.Trim()}";
                var httpclient = new HttpClient();
                var httpResponse = await httpclient.GetAsync(direction);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return default;
                }
                var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
                var resultSerialize = JsonConvert.DeserializeObject<T>(content);
                return resultSerialize;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        /// <summary>
        /// Obtiene un registro especifico.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<T> Find<T>(string url, int value)
        {
            try
            {
                string direction = $"{baseUrl}{url}?value={value}";
                var httpclient = new HttpClient();
                var httpResponse = await httpclient.GetAsync(direction);

                //if (!httpResponse.IsSuccessStatusCode)
                //{
                //    return null;
                //}
                var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
                var resultSerialize = JsonConvert.DeserializeObject<T>(content);
                return resultSerialize;

            }
            catch (Exception e)
            {

                throw;
            }
        }

        public static async Task<List<T>> FindAll<T>(string url, int value)
        {
            try
            {
                string direction = $"{baseUrl}{url}?value={value}";
                var httpclient = new HttpClient();
                var httpResponse = await httpclient.GetAsync(direction);

                //if (!httpResponse.IsSuccessStatusCode)
                //{
                //    return null;
                //}
                var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
                var resultSerialize = JsonConvert.DeserializeObject<List<T>>(content);
                return resultSerialize;

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}

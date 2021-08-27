using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Helpers
{
    public static class SendHttpClient
    {
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
            var httpResponse = await httpclient.PostAsync(url, contentSerialized);//
            //httpResponse.Wait();

            //var postJob = httpResponse.Result;
            if (!httpResponse.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Put(string url, object model)
        {
            var httpclient = new HttpClient();
            var content = JsonConvert.SerializeObject(model);//se convierte a json el contenido a enviar
            var contentSerialized = new StringContent(content, Encoding.Default, "application/json");//Agregamos informacion adicional al json
            var httpResponse = await httpclient.PutAsync(url, contentSerialized);//
            //httpResponse.Wait();

            //var postJob = httpResponse.Result;
            if (!httpResponse.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }


        public static async Task<List<T>> Exist<T>(string url, string value)
        {
            try
            {
                string direction = $"{url}?value={value.Trim()}";
                var httpclient = new HttpClient();
                var httpResponse = await httpclient.GetAsync(direction);

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

        public static async Task<T> Find<T>(string url, int value)
        {
            try
            {
                string direction = $"{url}?value={value}";
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

        /// <summary>
        /// Obteiene valores desde una API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<List<T>> Get<T>(string url/*, object model*/)
        {
            try
            {
                var httpclient = new HttpClient();
                var httpResponse = await httpclient.GetAsync(url);

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
    }
}

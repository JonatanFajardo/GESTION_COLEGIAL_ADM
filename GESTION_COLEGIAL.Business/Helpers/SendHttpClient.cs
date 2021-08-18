using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Helpers
{
    public static class SendHttpClient
    {
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

        public static async Task<T> Get<T>(string url)
        {
            var httpclient = new HttpClient();
            //var content = JsonConvert.SerializeObject(model);//se convierte a json el contenido a enviar
            //var contentSerialized = new StringContent(content, Encoding.Default, "application/json");//Agregamos informacion adicional al json
            var httpResponse = await httpclient.GetAsync(url);//

            if (!httpResponse.IsSuccessStatusCode)
            {
                return default(T);
            }
            var resultSerialize = await httpResponse.Content.ReadAsAsync<T>();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
            return resultSerialize;
        }
    }
}

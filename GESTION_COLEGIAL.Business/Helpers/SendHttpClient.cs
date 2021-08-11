using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
    }
}

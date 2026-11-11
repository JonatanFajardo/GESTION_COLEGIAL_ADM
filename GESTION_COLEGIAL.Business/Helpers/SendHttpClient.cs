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
		// Dirección de la API. En Docker se pasa por la variable de entorno GC_API_BASEURL
		// (ej. http://api:5076/api/v1/); sin ella se usa la API local de desarrollo.
		private static readonly string baseUrl = ResolverBaseUrl();

		// Un solo cliente para toda la app: crear uno por petición agota las conexiones.
		private static readonly HttpClient httpclient = CrearCliente();

		private static string ResolverBaseUrl()
		{
			var url = Environment.GetEnvironmentVariable("GC_API_BASEURL");
			if (string.IsNullOrWhiteSpace(url)) return "http://localhost:5076/api/v1/";
			return url.EndsWith("/") ? url : url + "/";
		}

		private static HttpClient CrearCliente()
		{
			var handler = new HttpClientHandler
			{
				// Acepta el certificado autofirmado de la API en desarrollo (mismo comportamiento que antes).
				ServerCertificateCustomValidationCallback = (mensaje, certificado, cadena, errores) => true
			};
			return new HttpClient(handler);
		}

		#region Asincrono

		/// <summary>
		/// Obtiene valores del servicio.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<string> GetRawAsync(string url)
		{
			try
			{
				var httpResponse = await httpclient.GetAsync($"{baseUrl}{url}");
				if (!httpResponse.IsSuccessStatusCode) return "[]";
				var content = await httpResponse.Content.ReadAsStringAsync();
				return string.IsNullOrWhiteSpace(content) ? "[]" : content;
			}
			catch { return "[]"; }
		}

		public static async Task<List<T>> GetAsync<T>(string url/*, object model*/)
		{
			try
			{
				var httpResponse = await httpclient.GetAsync($"{baseUrl}{url}");

				if (!httpResponse.IsSuccessStatusCode)
				{
					var errorContent = await httpResponse.Content.ReadAsStringAsync();
					return null;
				}

				var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.

				// Si el contenido está vacío o es null, retornar lista vacía
				if (string.IsNullOrWhiteSpace(content) || content == "null")
				{
					return new List<T>();
				}

				var resultSerialize = JsonConvert.DeserializeObject<List<T>>(content);

				// Si la deserialización devuelve null, retornar lista vacía
				return resultSerialize ?? new List<T>();
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public static async Task<List<T>> DropdownAsync<T>(string url, int id)
		{
			try
			{
				string direction = $"{baseUrl}{url}?id={id}";
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

		/// <summary>
		/// Envia datos a una API.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		public static async Task<bool> PostAsync(string url, object model)
		{
			var content = JsonConvert.SerializeObject(model);//se convierte a json el contenido a enviar
			var contentSerialized = new StringContent(content, Encoding.UTF8, "application/json");//Agregamos informacion adicional al json

			var httpResponse = await httpclient.PostAsync($"{baseUrl}{url}", contentSerialized);


			if (!httpResponse.IsSuccessStatusCode)
			{
				//Ver los errores.
				var errorContent = await httpResponse.Content.ReadAsStringAsync();
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
		public static async Task<bool> DeleteAsync(string url, int id)
		{
			var content = JsonConvert.SerializeObject(id);//se convierte a json el contenido a enviar
			var contentSerialized = new StringContent(content, Encoding.UTF8, "application/json");//Agregamos informacion adicional al json
			var httpResponse = await httpclient.PutAsync($"{baseUrl}{url}?value={id}", contentSerialized);

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
		public static async Task<bool> PutAsync(string url, object model)
		{
			var content = JsonConvert.SerializeObject(model);//se convierte a json el contenido a enviar
			var contentSerialized = new StringContent(content, Encoding.UTF8, "application/json");//Agregamos informacion adicional al json
			var httpResponse = await httpclient.PutAsync($"{baseUrl}{url}", contentSerialized);

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
		public static async Task<T> ExistAsync<T>(string url, string value)
		{
			try
			{
				string direction = $"{baseUrl}{url}?value={value.Trim()}";
				var httpResponse = await httpclient.GetAsync(direction);

				if (!httpResponse.IsSuccessStatusCode)
				{
					return default;
				}
				var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
				var resultSerialize = JsonConvert.DeserializeObject<T>(content);
				return resultSerialize;
			}
			catch (Exception)
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
		public static async Task<T> FindAsync<T>(string url, int value)
		{
			try
			{
				string direction = $"{baseUrl}{url}?value={value}";
				var httpResponse = await httpclient.GetAsync(direction);

				var content = await httpResponse.Content.ReadAsStringAsync();//resultado de la respuesta y tambien la convertimos al tipo de dato que desiemos.
				var resultSerialize = JsonConvert.DeserializeObject<T>(content);
				return resultSerialize;
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public static async Task<List<T>> FindAllAsync<T>(string url, int value)
		{
			try
			{
				string direction = $"{baseUrl}{url}?value={value}";
				var httpResponse = await httpclient.GetAsync(direction);

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
		/// Envia datos a una API mediante POST y retorna una respuesta tipada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="url"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		public static async Task<T> PostAsyncWithResponse<T>(string url, object model)
		{
			try
			{
				var content = JsonConvert.SerializeObject(model);
				var contentSerialized = new StringContent(content, Encoding.UTF8, "application/json");

				var httpResponse = await httpclient.PostAsync($"{baseUrl}{url}", contentSerialized);

				if (!httpResponse.IsSuccessStatusCode)
				{
					var errorContent = await httpResponse.Content.ReadAsStringAsync();
					return default;
				}

				var responseContent = await httpResponse.Content.ReadAsStringAsync();
				var resultSerialize = JsonConvert.DeserializeObject<T>(responseContent);
				return resultSerialize;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Obtiene un objeto único desde el servicio.
		/// Intenta deserializar como objeto único, si falla intenta como lista y retorna el primer elemento.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="url">URL completa con query params (ej: "Controller/Action?param=value")</param>
		/// <returns>Objeto de tipo T deserializado o el primer elemento si es una lista</returns>
		public static async Task<T> GetSingleAsync<T>(string url)
		{
			try
			{
				var httpResponse = await httpclient.GetAsync($"{baseUrl}{url}");

				if (!httpResponse.IsSuccessStatusCode)
				{
					var errorContent = await httpResponse.Content.ReadAsStringAsync();
					return default;
				}

				var content = await httpResponse.Content.ReadAsStringAsync();

				// Si el contenido está vacío o es null, retornar default
				if (string.IsNullOrWhiteSpace(content) || content == "null")
				{
					return default;
				}

				// Intentar deserializar como objeto único primero
				try
				{
					var resultSerialize = JsonConvert.DeserializeObject<T>(content);
					return resultSerialize;
				}
				catch
				{
					// Si falla, intentar como lista y retornar el primer elemento
					try
					{
						var resultList = JsonConvert.DeserializeObject<List<T>>(content);
						return resultList != null && resultList.Count > 0 ? resultList[0] : default;
					}
					catch
					{
						return default;
					}
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}

		#endregion Asincrono
	}
}
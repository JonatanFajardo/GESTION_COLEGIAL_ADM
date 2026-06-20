using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models.Graduacion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio MVC del modulo de graduacion. Habla contra el API
    /// Graduacion/* (candidatos, graduar, backfill).
    /// </summary>
    public class GraduacionService
    {
        private const string BaseUrl = "http://localhost:5076/api/v1/";

        /// <summary>
        /// Obtiene la lista de candidatos a graduacion.
        /// </summary>
        public async Task<IEnumerable<CandidatoGraduacionViewModel>> ListCandidatosAsync()
        {
            string url = "Graduacion/CandidatosAsync";
            var result = await ApiRequests.ListAsync<CandidatoGraduacionViewModel>(url);
            return result ?? new List<CandidatoGraduacionViewModel>();
        }

        /// <summary>
        /// Ejecuta la graduacion del alumno indicado. Devuelve un resultado con
        /// el mensaje original del SP cuando la validacion falla.
        /// </summary>
        public async Task<GraduacionResultadoViewModel> GraduarAsync(int alumnoId, int usuarioId)
        {
            string url = $"{BaseUrl}Graduacion/GraduarAsync?alumnoId={alumnoId}&usuarioId={usuarioId}";
            var resultado = new GraduacionResultadoViewModel { AlumnoId = alumnoId };
            try
            {
                using (var http = new HttpClient())
                {
                    var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync(url, content);
                    var body = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        resultado.Exitoso = true;
                        resultado.Mensaje = "Alumno graduado correctamente.";
                    }
                    else
                    {
                        resultado.Exitoso = false;
                        resultado.Mensaje = LimpiarMensaje(body);
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Error de conexion: " + ex.Message;
            }
            return resultado;
        }

        /// <summary>
        /// Ejecuta el backfill historico de graduaciones.
        /// </summary>
        public async Task<BackfillResultViewModel> BackfillAsync(int? anio, int topN, bool dryRun)
        {
            string query = $"topN={topN}&dryRun={(dryRun ? "true" : "false")}";
            if (anio.HasValue) query += $"&anio={anio.Value}";
            string url = $"{BaseUrl}Graduacion/BackfillAsync?{query}";

            try
            {
                using (var http = new HttpClient())
                {
                    var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                    var response = await http.PostAsync(url, content);
                    var body = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        return new BackfillResultViewModel
                        {
                            DryRun = dryRun,
                            Anio = anio,
                            TopN = topN,
                            Detalle = new List<BackfillDetalleViewModel>
                            {
                                new BackfillDetalleViewModel { Resultado = "Error", Mensaje = LimpiarMensaje(body) }
                            }
                        };
                    }
                    return JsonConvert.DeserializeObject<BackfillResultViewModel>(body)
                           ?? new BackfillResultViewModel { DryRun = dryRun, Anio = anio, TopN = topN };
                }
            }
            catch (Exception ex)
            {
                return new BackfillResultViewModel
                {
                    DryRun = dryRun,
                    Anio = anio,
                    TopN = topN,
                    Detalle = new List<BackfillDetalleViewModel>
                    {
                        new BackfillDetalleViewModel { Resultado = "Error", Mensaje = ex.Message }
                    }
                };
            }
        }

        private static string LimpiarMensaje(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return "Error al ejecutar la operacion.";
            // El API devuelve el mensaje en texto plano o JSON simple.
            var trimmed = body.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
            {
                try { return JsonConvert.DeserializeObject<string>(trimmed); }
                catch { return trimmed.Trim('"'); }
            }
            return trimmed;
        }
    }

    /// <summary>
    /// Resultado de una graduacion individual desde la perspectiva del UI.
    /// </summary>
    public class GraduacionResultadoViewModel
    {
        public int AlumnoId { get; set; }
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
    }
}

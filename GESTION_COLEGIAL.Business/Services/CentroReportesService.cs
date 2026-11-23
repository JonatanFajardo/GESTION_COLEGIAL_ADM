using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Centro de reportes: lee las filas de cualquier reporte de la API (los que ya existían y los
    /// nuevos de CentroReportes) y guarda / lista los reportes generados por el usuario.
    /// </summary>
    public class CentroReportesService
    {
        private const string Base = "CentroReportes/";

        /// <summary>Filas del reporte; null si la API respondió con error o no contestó.</summary>
        public async Task<List<JObject>> FilasAsync(string url)
        {
            try { return await SendHttpClient.GetAsync<JObject>(url); }
            catch (Exception) { return null; }
        }

        /// <summary>true si quedó registrado.</summary>
        public async Task<bool> RegistrarAsync(ReporteGeneradoViewModel model)
        {
            try { return !await ApiRequests.PostAsync(Base + "RegistrarAsync", model); }
            catch (Exception) { return false; }
        }

        public async Task<List<ReporteRecienteViewModel>> RecientesAsync(int usuId)
        {
            try
            {
                return (await ApiRequests.ListAsync<ReporteRecienteViewModel>($"{Base}RecientesAsync?usuId={usuId}&top=30"))?.ToList()
                       ?? new List<ReporteRecienteViewModel>();
            }
            catch (Exception) { return new List<ReporteRecienteViewModel>(); }
        }
    }
}

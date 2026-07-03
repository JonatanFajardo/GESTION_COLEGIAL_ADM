using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models.Graduacion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio MVC del directorio de graduados.
    /// Fusiona la data viva de la API (Graduados/ListAsync) con el snapshot
    /// histórico almacenado en App_Data/graduados-historicos.json.
    /// </summary>
    public class AlumnosGraduadosService
    {
        // Los IDs históricos ocupan el rango >= 900000 para no colisionar con la DB.
        public const int IdBaseHistorico = 900000;

        private static List<AlumnoGraduadoViewModel> _historicosCache;
        private static string _historicosCachePath;
        private static DateTime _historicosCacheStamp;

        public async Task<IEnumerable<AlumnoGraduadoViewModel>> ListAsync(string historicosPath = null)
        {
            var apiTask = ApiRequests.ListAsync<AlumnoGraduadoViewModel>("Graduados/ListAsync");
            var historicos = LoadHistoricos(historicosPath);
            var api = await apiTask;

            var combinados = new List<AlumnoGraduadoViewModel>();
            if (api != null) combinados.AddRange(api);
            combinados.AddRange(historicos);
            return combinados;
        }

        public async Task<AlumnoGraduadoViewModel> FindAsync(int id, string historicosPath = null)
        {
            if (id >= IdBaseHistorico)
            {
                return LoadHistoricos(historicosPath).FirstOrDefault(h => h.AluGrad_Id == id);
            }
            return await ApiRequests.GetSingleAsync<AlumnoGraduadoViewModel>($"Graduados/FindAsync?id={id}");
        }

        /// <summary>
        /// Carga y cachea los históricos desde disco. Si el archivo cambió (mtime),
        /// se recarga automáticamente sin reiniciar la app.
        /// </summary>
        private static List<AlumnoGraduadoViewModel> LoadHistoricos(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return new List<AlumnoGraduadoViewModel>();
            }

            var stamp = File.GetLastWriteTimeUtc(path);
            if (_historicosCache != null && _historicosCachePath == path && _historicosCacheStamp == stamp)
            {
                return _historicosCache;
            }

            try
            {
                var json = File.ReadAllText(path);
                var compactos = JsonConvert.DeserializeObject<List<AlumnoGraduadoCompactDto>>(json)
                                ?? new List<AlumnoGraduadoCompactDto>();
                var lista = compactos.ConvertAll(c => c.ToViewModel());
                _historicosCache = lista;
                _historicosCachePath = path;
                _historicosCacheStamp = stamp;
                return lista;
            }
            catch
            {
                return new List<AlumnoGraduadoViewModel>();
            }
        }
    }
}

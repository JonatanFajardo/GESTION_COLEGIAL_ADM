using GESTION_COLEGIAL.Business.Models.Instituto;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Lectura y escritura del archivo App_Data/instituto-config.json.
    /// Cache invalidada por mtime, para reflejar cambios sin reiniciar IIS.
    /// </summary>
    public class InstitutoConfigService
    {
        private static InstitutoConfigViewModel _cache;
        private static string _cachePath;
        private static DateTime _cacheStamp;
        private static readonly object _lock = new object();

        public static readonly InstitutoConfigViewModel Fallback = new InstitutoConfigViewModel
        {
            Nombre = "Instituto Nova Sula",
            Direccion = "San Pedro Sula, Cortés, Honduras",
        };

        public InstitutoConfigViewModel Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return Clone(Fallback);

            var stamp = File.GetLastWriteTimeUtc(path);
            if (_cache != null && _cachePath == path && _cacheStamp == stamp)
                return Clone(_cache);

            lock (_lock)
            {
                try
                {
                    var json = File.ReadAllText(path);
                    var model = JsonConvert.DeserializeObject<InstitutoConfigViewModel>(json) ?? Clone(Fallback);
                    if (string.IsNullOrWhiteSpace(model.Nombre)) model.Nombre = Fallback.Nombre;
                    if (string.IsNullOrWhiteSpace(model.Direccion)) model.Direccion = Fallback.Direccion;
                    _cache = model;
                    _cachePath = path;
                    _cacheStamp = stamp;
                    return Clone(model);
                }
                catch
                {
                    return Clone(Fallback);
                }
            }
        }

        public void Save(string path, InstitutoConfigViewModel model)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Ruta no válida.", nameof(path));
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            lock (_lock)
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                var json = JsonConvert.SerializeObject(model, Formatting.Indented);
                File.WriteAllText(path, json);

                _cache = Clone(model);
                _cachePath = path;
                _cacheStamp = File.GetLastWriteTimeUtc(path);
            }
        }

        private static InstitutoConfigViewModel Clone(InstitutoConfigViewModel src)
            => new InstitutoConfigViewModel { Nombre = src.Nombre, Direccion = src.Direccion };
    }
}

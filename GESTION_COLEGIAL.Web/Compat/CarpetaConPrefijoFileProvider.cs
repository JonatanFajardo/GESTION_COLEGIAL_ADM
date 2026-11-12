using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Publica una carpeta física bajo un prefijo de URL (ej. /Content → ..\GESTION_COLEGIAL.UI\Content).
    /// Se usa durante la migración para leer los estilos y scripts de la UI vieja sin duplicarlos,
    /// exponiendo solo esas carpetas (nunca Web.config, bin ni el código fuente).
    /// </summary>
    public sealed class CarpetaConPrefijoFileProvider : IFileProvider
    {
        private readonly string _prefijo;
        private readonly PhysicalFileProvider _carpeta;

        public CarpetaConPrefijoFileProvider(string prefijo, string rutaFisica)
        {
            _prefijo = "/" + prefijo.Trim('/');
            _carpeta = new PhysicalFileProvider(rutaFisica);
        }

        public IFileInfo GetFileInfo(string subpath) =>
            QuitarPrefijo(subpath, out var resto) ? _carpeta.GetFileInfo(resto) : new NotFoundFileInfo(subpath);

        public IDirectoryContents GetDirectoryContents(string subpath) =>
            QuitarPrefijo(subpath, out var resto) ? _carpeta.GetDirectoryContents(resto) : NotFoundDirectoryContents.Singleton;

        public IChangeToken Watch(string filter) =>
            QuitarPrefijo(filter, out var resto) ? _carpeta.Watch(resto) : NullChangeToken.Singleton;

        private bool QuitarPrefijo(string ruta, out string resto)
        {
            ruta = "/" + (ruta ?? string.Empty).TrimStart('/');

            if (ruta.Equals(_prefijo, StringComparison.OrdinalIgnoreCase))
            {
                resto = "/";
                return true;
            }

            if (ruta.StartsWith(_prefijo + "/", StringComparison.OrdinalIgnoreCase))
            {
                resto = ruta.Substring(_prefijo.Length);
                return true;
            }

            resto = null;
            return false;
        }
    }
}

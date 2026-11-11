namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Reemplazo de Server.MapPath de MVC 5: convierte "~/App_Data/archivo.json" en ruta física.
    /// Mientras dure la migración, App_Data, Content y Scripts apuntan a la UI vieja,
    /// así las dos UIs leen y guardan los mismos archivos (graduados históricos, datos del instituto).
    /// </summary>
    public static class RutasApp
    {
        private static readonly string[] CarpetasDeLaUiVieja = { "App_Data", "Content", "Scripts" };

        private static string _raizApp = AppContext.BaseDirectory;
        private static string _raizUiVieja;

        public static void Configurar(string raizApp, string raizUiVieja)
        {
            _raizApp = raizApp;
            _raizUiVieja = !string.IsNullOrEmpty(raizUiVieja) && Directory.Exists(raizUiVieja) ? raizUiVieja : null;
        }

        public static string MapPath(string rutaVirtual)
        {
            var relativa = (rutaVirtual ?? string.Empty)
                .TrimStart('~')
                .TrimStart('/', '\\')
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            if (_raizUiVieja != null)
            {
                var primeraCarpeta = relativa.Split(Path.DirectorySeparatorChar)[0];
                if (CarpetasDeLaUiVieja.Any(c => c.Equals(primeraCarpeta, StringComparison.OrdinalIgnoreCase)))
                {
                    return Path.Combine(_raizUiVieja, relativa);
                }
            }

            return Path.Combine(_raizApp, relativa);
        }
    }
}

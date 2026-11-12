namespace GESTION_COLEGIAL.UI.Compat
{
    /// <summary>
    /// Reemplazo de Server.MapPath de MVC 5: convierte "~/App_Data/archivo.json" en ruta física
    /// dentro de la carpeta del proyecto (en Docker, /app; App_Data va en un volumen).
    /// </summary>
    public static class RutasApp
    {
        private static string _raizApp = AppContext.BaseDirectory;

        public static void Configurar(string raizApp)
        {
            _raizApp = raizApp;
        }

        public static string MapPath(string rutaVirtual)
        {
            var relativa = (rutaVirtual ?? string.Empty)
                .TrimStart('~')
                .TrimStart('/', '\\')
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

            return Path.Combine(_raizApp, relativa);
        }
    }
}

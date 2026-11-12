namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>
    /// Página-índice de Configuración: grupos de tarjetas que enlazan a cada hub de catálogos o parámetros.
    /// </summary>
    public class ConfiguracionViewModel
    {
        public List<GrupoConfiguracion> Grupos { get; set; } = new List<GrupoConfiguracion>();

        public bool TieneTarjetas => Grupos.Any(g => g.Tarjetas.Count > 0);
    }

    public class GrupoConfiguracion
    {
        public string Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Lo que casi no cambia se muestra plegado al final.</summary>
        public bool Plegado { get; set; }

        public List<TarjetaConfiguracion> Tarjetas { get; set; } = new List<TarjetaConfiguracion>();
    }

    public class TarjetaConfiguracion
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        /// <summary>Clase de Bootstrap Icons (la misma que usa el hub en su encabezado).</summary>
        public string Icono { get; set; }

        /// <summary>Controller del hub; la tarjeta abre su Index.</summary>
        public string Controller { get; set; }

        /// <summary>Pantalla (permiso) que exige el hub. Sin ese permiso la tarjeta no se muestra.</summary>
        public string Pantalla { get; set; }

        /// <summary>Lo que se ajusta dentro del hub, para ubicarse sin entrar.</summary>
        public string[] Contenido { get; set; } = new string[0];
    }
}

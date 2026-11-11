using Newtonsoft.Json;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Resultado del rollover (RollForward): conteos de filas copiadas del
    /// año origen al año destino. Las claves JSON vienen en snake_case desde la API.
    /// </summary>
    public class RolloverResultViewModel
    {
        [JsonProperty("periodos_copiados")]
        public int PeriodosCopiados { get; set; }

        [JsonProperty("secciones_copiadas")]
        public int SeccionesCopiadas { get; set; }

        [JsonProperty("ofertas_copiadas")]
        public int OfertasCopiadas { get; set; }
    }
}

#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class EventoErrorDto
    {
        public int? ErrorId { get; set; }
        public string NombreArchivoError { get; set; }
        public DateTime? FechaError { get; set; }
        public string RutaError { get; set; }
        public string MensajeError { get; set; }
        public string InnerExceptionError { get; set; }
    }
}
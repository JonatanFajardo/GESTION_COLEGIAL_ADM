namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class DuracionDetailDto
    {
        public int? DuracionId { get; set; }
        public string DescripcionDuracion { get; set; }
        public string NombreUsuarioRegistraDuracion { get; set; }
        public DateTime? FechaRegistroDuracion { get; set; }
        public string NombreUsuarioModificaDuracion { get; set; }
        public DateTime? FechaModificacionDuracion { get; set; }
    }
}
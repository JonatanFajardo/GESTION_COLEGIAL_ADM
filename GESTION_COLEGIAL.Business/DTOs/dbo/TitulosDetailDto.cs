namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class TituloDetailDto
    {
        public int? TituloId { get; set; }
        public string DescripcionTitulo { get; set; }
        public string NombreUsuarioRegistraTitulo { get; set; }
        public DateTime? FechaRegistroTitulo { get; set; }
        public string NombreUsuarioModificaTitulo { get; set; }
        public DateTime? FechaModificacionTitulo { get; set; }
    }
}
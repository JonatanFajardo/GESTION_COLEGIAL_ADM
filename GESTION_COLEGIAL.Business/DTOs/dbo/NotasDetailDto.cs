namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class NotaDetailDto
    {
        public int? NotaId { get; set; }
        public int? ValorNota { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionSemestre { get; set; }
        public string DescripcionParcial { get; set; }
        public DateTime? AnioNota { get; set; }
        public string Activo { get; set; }
        public string NombreUsuarioRegistraNota { get; set; }
        public DateTime? FechaRegistroNota { get; set; }
        public string NombreUsuarioModificaNota { get; set; }
        public DateTime? FechaModificacionNota { get; set; }
    }
}
namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class MateriaDetailDto
    {
        public int? MateriaId { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionDuracion { get; set; }
        public string EsActivoMateria { get; set; }
        public string NombreUsuarioRegistraMateria { get; set; }
        public DateTime? FechaRegistroMateria { get; set; }
        public string NombreUsuarioModificaMateria { get; set; }
        public DateTime? FechaModificacionMateria { get; set; }
    }
}
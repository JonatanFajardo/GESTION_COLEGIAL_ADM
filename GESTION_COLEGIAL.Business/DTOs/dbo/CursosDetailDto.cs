namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class CursoDetailDto
    {
        public int? CursoId { get; set; }
        public string NombreCurso { get; set; }
        public string DescripcionAula { get; set; }
        public string DescripcionSeccion { get; set; }
        public string DescripcionNivel { get; set; }
        public string DescripcionModalidad { get; set; }
        public string EsActivoCurso { get; set; }
        public string NombreUsuarioRegistraCurso { get; set; }
        public DateTime? FechaRegistroCurso { get; set; }
        public string NombreUsuarioModificaCurso { get; set; }
        public DateTime? FechaModificacionCurso { get; set; }
    }
}
namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class CursoNivelDetailDto
    {
        public int? CursoNivelId { get; set; }
        public string DescripcionCursoNivel { get; set; }
        public string NombreUsuarioRegistraCursoNivel { get; set; }
        public DateTime? FechaRegistroCursoNivel { get; set; }
        public string NombreUsuarioModificaCursoNivel { get; set; }
        public DateTime? FechaModificacionCursoNivel { get; set; }
    }
}
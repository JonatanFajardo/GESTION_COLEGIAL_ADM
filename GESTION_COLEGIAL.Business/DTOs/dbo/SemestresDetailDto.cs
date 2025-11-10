using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class SemestreDetailDto
    {
        public int? SemestreId { get; set; }
        public string DescripcionSemestre { get; set; }
        public string EsActivoSemestre { get; set; }
        public string NombreUsuarioRegistraSemestre { get; set; }
        public DateTime? FechaRegistroSemestre { get; set; }
        public string NombreUsuarioModificaSemestre { get; set; }
        public DateTime? FechaModificacionSemestre { get; set; }
    }
}
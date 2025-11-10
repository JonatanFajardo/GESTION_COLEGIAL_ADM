using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class NivelEducativoDetailDto
    {
        public int? NivelId { get; set; }
        public string DescripcionNivel { get; set; }
        public string EsActivoNivel { get; set; }
        public string NombreUsuarioRegistraNivel { get; set; }
        public DateTime? FechaRegistroNivel { get; set; }
        public string NombreUsuarioModificaNivel { get; set; }
        public DateTime? FechaModificacionNivel { get; set; }
    }
}
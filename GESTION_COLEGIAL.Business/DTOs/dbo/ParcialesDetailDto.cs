using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class ParcialDetailDto
    {
        public int? ParcialId { get; set; }
        public string DescripcionParcial { get; set; }
        public string NombreUsuarioRegistraParcial { get; set; }
        public DateTime? FechaRegistroParcial { get; set; }
        public string NombreUsuarioModificaParcial { get; set; }
        public DateTime? FechaModificacionParcial { get; set; }
    }
}
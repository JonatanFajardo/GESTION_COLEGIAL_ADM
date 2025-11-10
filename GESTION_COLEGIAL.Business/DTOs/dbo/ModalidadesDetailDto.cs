using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class ModalidadDetailDto
    {
        public int? ModalidadId { get; set; }
        public string DescripcionModalidad { get; set; }
        public string NombreUsuarioRegistraModalidad { get; set; }
        public DateTime? FechaRegistroModalidad { get; set; }
        public string NombreUsuarioModificaModalidad { get; set; }
        public DateTime? FechaModificacionModalidad { get; set; }
    }
}
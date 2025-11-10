using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class SeccionDetailDto
    {
        public int? SeccionId { get; set; }
        public string DescripcionSeccion { get; set; }
        public string NombreUsuarioRegistraSeccion { get; set; }
        public DateTime? FechaRegistroSeccion { get; set; }
        public string NombreUsuarioModificaSeccion { get; set; }
        public DateTime? FechaModificacionSeccion { get; set; }
    }
}
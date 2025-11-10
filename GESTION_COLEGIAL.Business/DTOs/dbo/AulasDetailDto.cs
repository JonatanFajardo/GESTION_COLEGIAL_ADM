using System;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class AulaDetailDto
    {
        public int? AulaId { get; set; }
        public string DescripcionAula { get; set; }
        public string NombreUsuarioRegistraAula { get; set; }
        public DateTime? FechaRegistroAula { get; set; }
        public string NombreUsuarioModificaAula { get; set; }
        public DateTime? FechaModificacionAula { get; set; }
    }
}
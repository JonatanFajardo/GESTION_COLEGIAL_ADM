using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class EstadoDetailDto
    {
        public int? EstadoId { get; set; }
        public string DescripcionEstado { get; set; }
        public string NombreUsuarioRegistraEstado { get; set; }
        public DateTime? FechaRegistroEstado { get; set; }
        public string NombreUsuarioModificaEstado { get; set; }
        public DateTime? FechaModificacionEstado { get; set; }
    }
}
using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class CargoDetailDto
    {
        public int? CargoId { get; set; }
        public string DescripcionCargo { get; set; }
        public string NombreUsuarioRegistraCargo { get; set; }
        public DateTime? FechaRegistroCargo { get; set; }
        public string NombreUsuarioModificaCargo { get; set; }
        public DateTime? FechaModificacionCargo { get; set; }
    }
}
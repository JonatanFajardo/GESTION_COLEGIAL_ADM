using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class PagoFindViewModel
    {
        public int PagoId { get; set; }
        public int AlumnoId { get; set; }
        public int? EncargadoId { get; set; }
        public int FormaPagoId { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
        public string NumeroReferencia { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}
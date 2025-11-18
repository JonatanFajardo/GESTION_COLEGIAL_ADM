using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ReciboDetailViewModel
    {
        public int ReciboId { get; set; }
        public int PagoId { get; set; }
        public string NumeroRecibo { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public string RutaArchivo { get; set; } = string.Empty;
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}
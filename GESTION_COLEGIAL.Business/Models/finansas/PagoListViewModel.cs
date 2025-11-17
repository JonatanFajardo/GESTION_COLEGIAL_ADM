using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class PagoListViewModel
    {
        public int PagoId { get; set; }
        public int AlumnoId { get; set; }
        public string NombreAlumno { get; set; } = string.Empty;
        public int? EncargadoId { get; set; }
        public int FormaPagoId { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
        public string NumeroReferencia { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }
}
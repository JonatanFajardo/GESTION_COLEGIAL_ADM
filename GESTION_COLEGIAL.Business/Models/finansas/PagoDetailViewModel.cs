using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class PagoDetailViewModel
    {
        // Información del Recibo (de PR_tbPagos_GetReciboResult)
        public int ReciboId { get; set; }
        public string NumeroRecibo { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public string RutaArchivo { get; set; } = string.Empty;

        // Información del Pago
        public int PagoId { get; set; }
        public int AlumnoId { get; set; }
        public int? EncargadoId { get; set; }
        public int FormaPagoId { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
        public string NumeroReferencia { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public string FormaPagoDescripcion { get; set; } = string.Empty;
        public decimal TotalDistribuido { get; set; }

        // Información del Alumno (de PR_tbPagos_GetReciboResult)
        public string AlumnoNombre { get; set; } = string.Empty;
        public string AlumnoIdentidad { get; set; } = string.Empty;

        // Información de Forma de Pago
        public string FormaPago { get; set; } = string.Empty;

        // Información de auditoría
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public string NombreCompletoUsuarioRegistra { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public string NombreCompletoUsuarioModifica { get; set; } = string.Empty;
        public DateTime? FechaModifica { get; set; }
    }
}
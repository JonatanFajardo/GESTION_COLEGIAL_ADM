using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class PagoListViewModel
    {
        public long? Fila { get; set; }
        public int PagoId { get; set; }
        public int AlumnoId { get; set; }
        public string NombreAlumno { get; set; } = string.Empty;
        public int? EncargadoId { get; set; }
        public int FormaPagoId { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
        //public decimal MontoTotal { get; set; }
        public string NumeroReferencia { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        //public string FormaPago { get; set; } = string.Empty;
        public string Alumno { get; set; } = string.Empty;
        public string Encargado { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
    }
}
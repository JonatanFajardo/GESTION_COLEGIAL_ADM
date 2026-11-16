using System;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Anular un pago. Destino: "Devuelto" (se devolvió el dinero) o "SaldoFavor".</summary>
    public class AnularPagoViewModel
    {
        public int PagoId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }

    /// <summary>Perdonar (rebajar sin cobrar) parte o todo un cargo. Tipo: Exoneración, Beca o Ajuste.</summary>
    public class PerdonarCargoViewModel
    {
        public int CuentaCobrarId { get; set; }
        public decimal Monto { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }

    /// <summary>Saldo a favor de un alumno.</summary>
    public class SaldoFavorViewModel
    {
        public int AlumnoId { get; set; }
        public decimal Saldo { get; set; }
    }

    /// <summary>Devolver dinero del saldo a favor. Metodo: Efectivo, Transferencia o Cheque.</summary>
    public class DevolverSaldoViewModel
    {
        public int AlumnoId { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }

    /// <summary>Aplicar saldo a favor a los cargos pendientes del alumno.</summary>
    public class AplicarSaldoViewModel
    {
        public int AlumnoId { get; set; }
        public decimal Monto { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }

    /// <summary>Registro de una corrección hecha a la cuenta del alumno.</summary>
    public class CorreccionCuentaViewModel
    {
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public decimal Monto { get; set; }
    }

    /// <summary>Resultado de un POST a la API: si hubo error y el mensaje para el usuario.</summary>
    public class RespuestaApi
    {
        public bool Error { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}

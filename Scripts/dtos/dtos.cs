
#nullable enable
using System;

namespace ModuloFinanciero.DTOs
{
    // ===== Conceptos de Pago =====
    public class ConceptoPagoListDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsRecurrente { get; set; }
        public bool EsObligatorio { get; set; }
        public bool EsActivo { get; set; }
    }

    public class ConceptoPagoFindDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsRecurrente { get; set; }
        public bool EsObligatorio { get; set; }
        public bool EsActivo { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }

    public class ConceptoPagoDetailDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsRecurrente { get; set; }
        public bool EsObligatorio { get; set; }
        public bool EsActivo { get; set; }
        public int CantidadTarifas { get; set; }
    }

    public class ConceptoPagoDropdownDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class ConceptoPagoExistDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }

    // ===== Tarifas =====
    public class TarifaListDto
    {
        public int Id { get; set; }
        public int ConceptoPagoId { get; set; }
        public decimal Monto { get; set; }
        public short AnioVigencia { get; set; }
    }

    public class TarifaFindDto
    {
        public int Id { get; set; }
        public int ConceptoPagoId { get; set; }
        public int? NivelId { get; set; }
        public int? CuotaNivelId { get; set; }
        public decimal Monto { get; set; }
        public short AnioVigencia { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }

    public class TarifaDetailDto
    {
        public int Id { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public short AnioVigencia { get; set; }
        public int? NivelId { get; set; }
        public int? CuotaNivelId { get; set; }
    }

    public class TarifaDropdownDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    public class TarifaExistDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }

    // ===== Formas de Pago =====
    public class FormaPagoListDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
    }

    public class FormaPagoFindDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsActivo { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }

    public class FormaPagoDropdownDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class FormaPagoExistDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }

    // ===== Descuentos =====
    public class DescuentoListDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TipoDescuento { get; set; } = "P";
        public decimal Valor { get; set; }
        public bool EsActivo { get; set; }
    }

    public class DescuentoFindDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TipoDescuento { get; set; } = "P";
        public decimal Valor { get; set; }
        public bool EsActivo { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }

    public class DescuentoDropdownDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class DescuentoExistDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }

    // ===== Estados de Pago =====
    public class EstadoPagoListDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class EstadoPagoFindDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }

    public class EstadoPagoDropdownDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }

    public class EstadoPagoExistDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }

    // ===== Cuentas por Cobrar =====
    public class CuentaCobrarListDto
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public decimal MontoPendiente { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class CuentaCobrarFindDto
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int ConceptoPagoId { get; set; }
        public int? TarifaId { get; set; }
        public decimal MontoOriginal { get; set; }
        public decimal MontoDescuento { get; set; }
        public decimal MontoMora { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoPendiente { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int EstadoPagoId { get; set; }
        public string? Observaciones { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }

    public class CuentaCobrarDetailDto
    {
        public int Id { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public decimal MontoPendiente { get; set; }
        public decimal TotalPagado { get; set; }
    }

    public class CuentaCobrarDropdownDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    public class CuentaCobrarExistDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }

    // ===== Pagos =====
    public class PagoListDto
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
    }

    public class PagoFindDto
    {
        public int Id { get; set; }
        public int AlumnoId { get; set; }
        public int FormaPagoId { get; set; }
        public decimal MontoTotal { get; set; }
        public DateTime FechaPago { get; set; }
        public string? Observaciones { get; set; }
        public bool EsEliminado { get; set; }
        public int UsuarioRegistraId { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? UsuarioModificaId { get; set; }
        public DateTime? FechaModifica { get; set; }
    }

    public class PagoDropdownDto
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    public class PagoExistDto
    {
        public bool Exists { get; set; }
        public string? Message { get; set; }
    }
}

# FormaPagoDetailViewModel
$content1 = @"
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class FormaPagoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Fpa_Id { get; set; }
        
        [Display(Name = "Descripción")]
        public string Fpa_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public bool Fpa_EsActivo { get; set; }
        
        [Display(Name = "Cantidad de Pagos")]
        public int? CantidadPagos { get; set; }
        
        [Display(Name = "Creado por")]
        public string NombreCompletoUsuarioRegistra { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime Fpa_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string NombreCompletoUsuarioModifica { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? Fpa_FechaModifica { get; set; }
    }
}
"@
$content1 | Out-File -FilePath "GESTION_COLEGIAL.Business\Models\FormaPagoDetailViewModel.cs" -Encoding UTF8

# EstadoPagoDetailViewModel
$content2 = @"
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class EstadoPagoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Epa_Id { get; set; }
        
        [Display(Name = "Descripción")]
        public string Epa_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public bool Epa_EsActivo { get; set; }
        
        [Display(Name = "Cantidad de Cuentas")]
        public int? CantidadCuentas { get; set; }
        
        [Display(Name = "Creado por")]
        public string NombreCompletoUsuarioRegistra { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime Epa_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string NombreCompletoUsuarioModifica { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? Epa_FechaModifica { get; set; }
    }
}
"@
$content2 | Out-File -FilePath "GESTION_COLEGIAL.Business\Models\EstadoPagoDetailViewModel.cs" -Encoding UTF8

# DescuentoDetailViewModel
$content3 = @"
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class DescuentoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Des_Id { get; set; }
        
        [Display(Name = "Descripción")]
        public string Des_Descripcion { get; set; }
        
        [Display(Name = "Tipo de Descuento")]
        public string Des_TipoDescuento { get; set; }
        
        [Display(Name = "Valor")]
        public decimal Des_Valor { get; set; }
        
        [Display(Name = "Activo")]
        public bool Des_EsActivo { get; set; }
        
        [Display(Name = "Cantidad de Aplicaciones")]
        public int? CantidadAplicaciones { get; set; }
        
        [Display(Name = "Creado por")]
        public string NombreCompletoUsuarioRegistra { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime Des_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string NombreCompletoUsuarioModifica { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? Des_FechaModifica { get; set; }
    }
}
"@
$content3 | Out-File -FilePath "GESTION_COLEGIAL.Business\Models\DescuentoDetailViewModel.cs" -Encoding UTF8

# ConceptoPagoDetailViewModel
$content4 = @"
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ConceptoPagoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Cpa_Id { get; set; }
        
        [Display(Name = "Descripción")]
        public string Cpa_Descripcion { get; set; }
        
        [Display(Name = "Es Recurrente")]
        public bool Cpa_EsRecurrente { get; set; }
        
        [Display(Name = "Es Obligatorio")]
        public bool Cpa_EsObligatorio { get; set; }
        
        [Display(Name = "Activo")]
        public bool Cpa_EsActivo { get; set; }
        
        [Display(Name = "Cantidad de Tarifas")]
        public int? CantTarifas { get; set; }
        
        [Display(Name = "Creado por")]
        public string NombreCompletoUsuarioRegistra { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime Cpa_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string NombreCompletoUsuarioModifica { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? Cpa_FechaModifica { get; set; }
    }
}
"@
$content4 | Out-File -FilePath "GESTION_COLEGIAL.Business\Models\ConceptoPagoDetailViewModel.cs" -Encoding UTF8

Write-Output "Creados 4 DetailViewModels financieros (FormasPago, EstadosPago, Descuentos, ConceptosPago)"

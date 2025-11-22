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

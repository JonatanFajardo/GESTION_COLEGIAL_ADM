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

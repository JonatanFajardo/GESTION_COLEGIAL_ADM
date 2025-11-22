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

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

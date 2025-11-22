using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class DuracionDetailViewModel : BaseViewModel
    {
        [Key]
        public int Dur_Id { get; set; }
        
        [Display(Name = "DescripciÃ³n")]
        public string Dur_Descripcion { get; set; }
        
        [Display(Name = "Creado por")]
        public string Dur_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Dur_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Dur_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Dur_FechaModifica { get; set; }
    }
}

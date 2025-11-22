using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class SeccionDetailViewModel : BaseViewModel
    {
        [Key]
        public int Sec_Id { get; set; }
        
        [Display(Name = "DescripciÃ³n")]
        public string Sec_Descripcion { get; set; }
        
        [Display(Name = "Creado por")]
        public string Sec_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Sec_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Sec_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Sec_FechaModifica { get; set; }
    }
}

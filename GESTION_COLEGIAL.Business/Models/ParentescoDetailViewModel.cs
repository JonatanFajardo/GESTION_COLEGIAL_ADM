using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ParentescoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Par_Id { get; set; }
        
        [Display(Name = "DescripciÃ³n")]
        public string Par_Descripcion { get; set; }
        
        [Display(Name = "Creado por")]
        public string Par_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Par_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Par_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Par_FechaModifica { get; set; }
    }
}

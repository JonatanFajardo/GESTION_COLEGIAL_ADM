using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ParcialDetailViewModel : BaseViewModel
    {
        [Key]
        public int Pac_Id { get; set; }
        
        [Display(Name = "DescripciÃ³n")]
        public string Pac_Descripcion { get; set; }
        
        [Display(Name = "Creado por")]
        public string Pac_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Pac_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Pac_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Pac_FechaModifica { get; set; }
    }
}

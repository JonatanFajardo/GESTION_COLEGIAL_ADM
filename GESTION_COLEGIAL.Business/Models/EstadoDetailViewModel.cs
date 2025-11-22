using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class EstadoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Est_Id { get; set; }
        
        [Display(Name = "DescripciÃ³n")]
        public string Est_Descripcion { get; set; }
        
        [Display(Name = "Creado por")]
        public string Est_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Est_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Est_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Est_FechaModifica { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class CursoNivelDetailViewModel : BaseViewModel
    {
        [Key]
        public int Cun_Id { get; set; }
        
        [Display(Name = "DescripciÃ³n")]
        public string Cun_Descripcion { get; set; }
        
        [Display(Name = "Creado por")]
        public string Cun_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Cun_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Cun_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Cun_FechaModifica { get; set; }
    }
}

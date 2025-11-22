using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class MateriaDetailViewModel : BaseViewModel
    {
        [Key]
        public int Mat_Id { get; set; }
        
        [Display(Name = "Nombre")]
        public string Mat_Nombre { get; set; }
        
        [Display(Name = "DuraciÃ³n")]
        public string Dur_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public string Mat_EsActivo { get; set; }
        
        [Display(Name = "Creado por")]
        public string Mat_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Mat_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Mat_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Mat_FechaModifica { get; set; }
    }
}

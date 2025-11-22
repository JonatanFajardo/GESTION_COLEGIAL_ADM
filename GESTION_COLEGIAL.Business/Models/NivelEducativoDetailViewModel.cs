using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class NivelEducativoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Niv_Id { get; set; }
        
        [Display(Name = "DescripciÃ³n")]
        public string Niv_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public string Niv_EsActivo { get; set; }
        
        [Display(Name = "Creado por")]
        public string Niv_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creaciÃ³n")]
        public DateTime? Niv_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Niv_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificaciÃ³n")]
        public DateTime? Niv_FechaModifica { get; set; }
    }
}

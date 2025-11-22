using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class SemestreDetailViewModel : BaseViewModel
    {
        [Key]
        public int Sem_Id { get; set; }
        
        [Display(Name = "Descripción")]
        public string Sem_Descripcion { get; set; }
        
        [Display(Name = "Activo")]
        public string Sem_EsActivo { get; set; }
        
        [Display(Name = "Creado por")]
        public string Sem_UsuarioRegistraNombre { get; set; }
        
        [Display(Name = "Fecha de creación")]
        public DateTime? Sem_FechaRegistra { get; set; }
        
        [Display(Name = "Modificado por")]
        public string Sem_UsuarioModificaNombre { get; set; }
        
        [Display(Name = "Fecha de modificación")]
        public DateTime? Sem_FechaModifica { get; set; }
    }
}

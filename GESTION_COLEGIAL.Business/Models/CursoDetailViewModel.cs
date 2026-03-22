using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class CursoDetailViewModel : BaseViewModel
    {
        [Key]
        public int Cur_Id { get; set; }

        [Display(Name = "Nombre")]
        public string Cur_Nombre { get; set; }

        [Display(Name = "Creado por")]
        public string Cur_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime? Cur_FechaRegistra { get; set; }

        [Display(Name = "Modificado por")]
        public string Cur_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha de modificación")]
        public DateTime? Cur_FechaModifica { get; set; }
    }
}

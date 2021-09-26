
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class ParentescoViewModel : BaseViewModel
    {
        [Key]

        public int Par_Id { get; set; }
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Par_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Par_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Par_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Par_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Par_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Par_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Par_FechaModifica { get; set; }
    }
}
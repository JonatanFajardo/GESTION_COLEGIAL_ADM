using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class ModalidadViewModel
    {
        [Key]
        public int Mda_Id { get; set; }

        [Display(Name = "Descripcion")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Mda_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Mda_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Mda_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Mda_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Mda_UsuarioModifica { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Mda_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Mda_FechaModifica { get; set; }
    }
}
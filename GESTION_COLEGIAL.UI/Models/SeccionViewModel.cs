
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class SeccionViewModel : BaseViewModel
    {

        [Key]
        public int Sec_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Sec_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Sec_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Sec_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Sec_FechaRegistra { get; set; }

        [Display(Name = "Fecha modifica")]
        public int? Sec_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Sec_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Sec_FechaModifica { get; set; }


    }
}
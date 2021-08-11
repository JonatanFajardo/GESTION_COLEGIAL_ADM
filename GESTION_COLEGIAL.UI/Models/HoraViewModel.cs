
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class HoraViewModel
    {

        [Key]
        public int Hor_Id { get; set; }

        [StringLength(11)]
        [Display(Name = "Hora")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Hor_Hora { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Hor_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Hor_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Hor_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Hor_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Hor_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Hor_FechaModifica { get; set; }
    }

}
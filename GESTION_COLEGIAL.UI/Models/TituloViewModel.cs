
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class TituloViewModel
    {

        [Key]
        public int Tit_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Tit_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Tit_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Tit_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Tit_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Tit_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Tit_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Tit_FechaModifica { get; set; }

    }
}
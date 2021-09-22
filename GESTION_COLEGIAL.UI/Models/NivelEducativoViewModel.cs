
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class NivelEducativoViewModel
    {
        [Key]
        public int Niv_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Niv_Descripcion { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? EsActivo { get; set; }
        public string? Niv_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Niv_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Niv_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Niv_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Niv_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Niv_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Niv_FechaModifica { get; set; }

    }
}
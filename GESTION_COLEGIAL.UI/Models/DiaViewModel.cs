
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class DiaViewModel
    {

        [Key]
        public int Dia_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Dia_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Dia_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Dia_UsuarioRegistra { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Dia_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Dia_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Dia_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Dia_FechaModifica { get; set; }

    }
}
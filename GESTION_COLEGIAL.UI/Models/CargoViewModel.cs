
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class CargoViewModel
    {

        [Key]
        public int Car_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Car_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Car_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Car_UsuarioRegistra { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Car_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Car_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Car_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Car_FechaModifica { get; set; }

    }
}
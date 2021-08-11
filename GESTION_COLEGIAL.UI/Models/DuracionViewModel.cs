
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class DuracionViewModel
    {

        [Key]
        public int Dur_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Dur_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Dur_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Dur_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Dur_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Dur_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Dur_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Dur_FechaModifica { get; set; }
    }
}
﻿
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class MateriaViewModel
    {
        [Key]
        public int Mat_Id { get; set; }

        [StringLength(150)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Mat_Nombre { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Dur_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Mat_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Mat_UsuarioRegistra { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Mat_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Mat_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Mat_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Mat_FechaModifica { get; set; }

    }
}
﻿
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class SemestreViewModel
    {

        [Key]
        public int Sem_Id { get; set; }

        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Sem_Descripcion { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? EsActivo { get; set; }
        public string Sem_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Sem_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Sem_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Sem_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Sem_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Sem_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Sem_FechaModifica { get; set; }

    }
}
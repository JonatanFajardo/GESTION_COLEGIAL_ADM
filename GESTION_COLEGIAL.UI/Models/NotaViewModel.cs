
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class NotaViewModel
    {
        [Key]
        public int Not_Id { get; set; }

        [Display(Name = "Nota")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Not_Nota { get; set; }

        [Display(Name = "Materia")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Mat_Id { get; set; }

        [Display(Name = "Semestre")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Sem_Id { get; set; }

        [Display(Name = "Parcial")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Pac_Id { get; set; }

        [Column(TypeName = "Año")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime Not_Año { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? EsActivo { get; set; }
        public string Not_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Not_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Not_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Not_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Not_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Not_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Not_FechaModifica { get; set; }

    }
}
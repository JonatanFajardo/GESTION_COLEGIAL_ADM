
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class CursoNombre : BaseViewModel
    {
        [Key]
        public int Cno_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Cno_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Cno_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Cno_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Cno_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Cno_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Cno_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Cno_FechaModifica { get; set; }

    }
}
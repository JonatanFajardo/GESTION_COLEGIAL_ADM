
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class HorarioProfesor : BaseViewModel
    {
        [Key]
        public int HoPr_Id { get; set; }

        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Cur_Id { get; set; }

        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Cun_Id { get; set; }

        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int HoPr_HoraInicio { get; set; }

        [Display(Name = "Hora finaliza")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int HoPr_HoraFinaliza { get; set; }

        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Dia_Id { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int HoPr_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string HoPr_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime HoPr_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? HoPr_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string HoPr_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? HoPr_FechaModifica { get; set; }

    }
}
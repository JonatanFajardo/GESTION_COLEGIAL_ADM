
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.UI.Models
{
    public class HorarioAlumnoViewModel
    {
        [Key]
        public int HoAl_Id { get; set; }

        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Cur_Id { get; set; }

        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Cun_Id { get; set; }

        [Display(Name = "Materia")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Mat_Id { get; set; }

        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int HoAl_HoraInicio { get; set; }

        [Display(Name = "Hora finaliza")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int HoAl_HoraFinaliza { get; set; }

        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Dia_Id { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int HoAl_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string HoAl_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime HoAl_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? HoAl_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string HoAl_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? HoAl_FechaModifica { get; set; }

    }
}
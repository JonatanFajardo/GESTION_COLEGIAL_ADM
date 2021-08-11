
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class CursoViewModel
    {

        [Key]
        public int Cur_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Cno_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Aul_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Sec_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Niv_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Mda_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Cur_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Cur_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Cur_UsuarioRegistra { get; set; }
        [Column(TypeName = "datetime")]

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime Cur_FechaRegistra { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int? Cur_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Cur_UsuarioModifica { get; set; }
        [Column(TypeName = "datetime")]

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime? Cur_FechaModifica { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class SeccionViewModel
    {

        [Key]
        public int Sec_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Sec_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Sec_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Sec_UsuarioRegistra { get; set; }

        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime Sec_FechaRegistra { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int? Sec_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Sec_UsuarioModifica { get; set; }

        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime? Sec_FechaModifica { get; set; }


    }
}
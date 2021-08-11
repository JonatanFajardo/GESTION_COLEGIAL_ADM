
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class ParentescoViewModel
    {
        [Key]

        public int Par_Id { get; set; }
        [StringLength(100)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]

        public string Par_Descripcion { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Par_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Par_UsuarioRegistra { get; set; }
        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]

        public DateTime Par_FechaRegistra { get; set; }
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]

        public int? Par_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Par_UsuarioModifica { get; set; }
        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]

        public DateTime? Par_FechaModifica { get; set; }
    }
}
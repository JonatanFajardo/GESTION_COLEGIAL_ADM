
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class UsuarioViewModel
    {
        [Key]
        public int Usu_Id { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Emp_Id { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Usu_Name { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int? Con_Id { get; set; }

        [MaxLength(8000)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public byte[] Usu_ContrasenaRecuperacion
        {
            get; set;
        }
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public int Rol_Id { get; set; }

        [StringLength(16)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Usu_Ip { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Usu_EsActivo { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Usu_Suspendido { get; set; }

        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime? Usu_FechaCreacion { get; set; }

        [Column(TypeName = "datetime")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime? Usu_fechaModificacion
        {
            get; set;
        }
    }
}
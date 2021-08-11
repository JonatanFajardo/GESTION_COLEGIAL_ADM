
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.UI.Models
{
    public class PersonaViewModel
    {
        [Key]
        public int Per_Id { get; set; }

        [StringLength(13)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_Identidad { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_PrimerNombre { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_SegundoNombre { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_ApellidoPaterno { get; set; }

        [StringLength(50)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_ApellidoMaterno { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public DateTime Per_FechaNacimiento { get; set; }

        [StringLength(150)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_CorreoElectronico { get; set; }

        [StringLength(9)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_Telefono { get; set; }

        [StringLength(150)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_Direccion { get; set; }

        [StringLength(1)]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public string Per_Sexo { get; set; }

        [Display(Name = "")]
        [Required(ErrorMessage = "El campo  es requerido")]
        public bool? Per_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Per_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Per_UsuarioRegistra { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Per_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Per_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Per_UsuarioModifica { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Per_FechaModifica { get; set; }

    }
}
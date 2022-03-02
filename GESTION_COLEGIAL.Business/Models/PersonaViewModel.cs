
using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class PersonaViewModel : BaseViewModel
    {
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 digitos")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe debe ser numerico")]
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_Identidad { get; set; }

        [StringLength(50)]
        [Display(Name = "Primer nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_PrimerNombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Segundo nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_SegundoNombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Apellido paterno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_ApellidoPaterno { get; set; }

        [StringLength(50)]
        [Display(Name = "Apellido materno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_ApellidoMaterno { get; set; }

        [Display(Name = "Fecha nacimiento")]
        [Required(ErrorMessage = "El campo es requerido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Per_FechaNacimiento { get; set; }

        [StringLength(150)]
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo valido")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_CorreoElectronico { get; set; }

        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe de tener minimo 8 digitos y como maximo 11")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe debe ser numerico")]
        public string Per_Telefono { get; set; }

        [StringLength(150)]
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_Direccion { get; set; }

        [StringLength(1)]
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_Sexo { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]

        public bool Per_EsActivo { get; set; }
        public string EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Per_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Per_UsuarioRegistraNombre { get; set; }

        [Display(Name = "Fecha registra")]
        public DateTime Per_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        public int? Per_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Per_UsuarioModificaNombre { get; set; }

        [Display(Name = "Fecha modifica")]
        public DateTime? Per_FechaModifica { get; set; }

    }
}
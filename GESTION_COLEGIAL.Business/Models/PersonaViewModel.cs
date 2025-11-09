using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de una persona.
    /// </summary>
    public class PersonaViewModel : BaseViewModel
    {
        /// <summary>
        /// Identidad de la persona.
        /// </summary>
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 digitos")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe debe ser numerico")]
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string NumeroIdentidad { get; set; }

        /// <summary>
        /// Primer nombre de la persona.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Primer nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string PrimerNombre { get; set; }

        /// <summary>
        /// Segundo nombre de la persona.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Segundo nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string SegundoNombre { get; set; }

        /// <summary>
        /// Apellido paterno de la persona.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Apellido paterno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string ApellidoPaterno { get; set; }

        /// <summary>
        /// Apellido materno de la persona.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Apellido materno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string ApellidoMaterno { get; set; }

        /// <summary>
        /// Fecha de nacimiento de la persona.
        /// </summary>
        [Display(Name = "Fecha nacimiento")]
        [Required(ErrorMessage = "El campo es requerido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        /// <summary>
        /// Correo electrónico de la persona.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo valido")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Teléfono de la persona.
        /// </summary>
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe de tener minimo 8 digitos y como maximo 11")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe debe ser numerico")]
        public string Telefono { get; set; }

        /// <summary>
        /// Dirección de la persona.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Direccion { get; set; }

        /// <summary>
        /// Sexo de la persona.
        /// </summary>
        [StringLength(1)]
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Sexo { get; set; }

        /// <summary>
        /// Indica si la persona está activa.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool EsActivoPersona { get; set; }
        /// <summary>
        /// Indica si la persona está activa.
        /// </summary>

        public string EsActivo { get; set; }

        /// <summary>
        /// Identificador del usuario que registra la persona.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraPersonaId { get; set; }

        /// <summary>
        /// Nombre del usuario que registra la persona.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraPersona { get; set; }

        /// <summary>
        /// Fecha de registro de la persona.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroPersona { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica la persona.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaPersonaId { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica la persona.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaPersona { get; set; }

        /// <summary>
        /// Fecha de modificación de la persona.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionPersona { get; set; }
    }
}


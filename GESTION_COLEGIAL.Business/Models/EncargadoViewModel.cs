using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
	public class EncargadoViewModel : BaseViewModel
	{
		/// <summary>
		/// Obtiene o establece el ID del encargado.
		/// </summary>
		[Key]
		public int EncargadoId { get; set; }

		/// <summary>
		/// Obtiene o establece el ID de la persona.
		/// </summary>
		[Display(Name = "Persona")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int PersonaId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del encargado.
		/// </summary>
		public string NombreEncargado { get; set; }

		/// <summary>
		/// Obtiene o establece la ocupación del encargado.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Ocupación")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string OcupacionEncargado { get; set; }

		/// <summary>
		/// Obtiene o establece la identidad de la persona.
		/// </summary>
		[StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 dígitos")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		[Display(Name = "Identidad")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string NumeroIdentidad { get; set; }

		/// <summary>
		/// Obtiene o establece el primer nombre de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Primer nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string PrimerNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el segundo nombre de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Segundo nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string SegundoNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido paterno de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido paterno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string ApellidoPaterno { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido materno de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido materno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string ApellidoMaterno { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de nacimiento de la persona.
		/// </summary>
		[Display(Name = "Fecha de nacimiento")]
		[Required(ErrorMessage = "El campo es requerido")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime FechaNacimiento { get; set; }

		/// <summary>
		/// Obtiene o establece el correo electrónico de la persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Correo electrónico")]
		[EmailAddress(ErrorMessage = "Ingrese un correo válido")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string CorreoElectronico { get; set; }

		/// <summary>
		/// Obtiene o establece el teléfono de la persona.
		/// </summary>
		[Display(Name = "Teléfono")]
		[Required(ErrorMessage = "El campo es requerido")]
		[StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe tener un mínimo de 8 dígitos y un máximo de 11")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		public string Telefono { get; set; }

		/// <summary>
		/// Obtiene o establece la dirección de la persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Direccion { get; set; }

		/// <summary>
		/// Obtiene o establece el sexo de la persona.
		/// </summary>
		[StringLength(1)]
		[Display(Name = "Sexo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Sexo { get; set; }

		/// <summary>
		/// Obtiene o establece el estado de actividad del encargado.
		/// </summary>
		[Display(Name = "Es activo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string EsActivo { get; set; }

		/// <summary>
		/// Obtiene o establece un valor que indica si la persona está activa.
		/// </summary>
		public bool EsActivoPersona { get; set; }

		/// <summary>
		/// Obtiene o establece el ID del usuario que registró la persona.
		/// </summary>
		[Display(Name = "Usuario que registra ID")]
		public int UsuarioRegistraPersonaId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que registró la persona.
		/// </summary>
		[Display(Name = "Usuario que registra Nombre")]
		public string NombreUsuarioRegistraPersona { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de registro de la persona.
		/// </summary>
		[Display(Name = "Fecha de registro")]
		public DateTime FechaRegistroPersona { get; set; }

		/// <summary>
		/// Obtiene o establece el ID del usuario que modificó la persona.
		/// </summary>
		[Display(Name = "Usuario que modifica ID")]
		public int UsuarioModificaPersonaId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que modificó la persona.
		/// </summary>
		[Display(Name = "Usuario que modifica Nombre")]
		public string NombreUsuarioModificaPersona { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de modificación de la persona.
		/// </summary>
		[Display(Name = "Fecha de modificación")]
		public DateTime? FechaModificacionPersona { get; set; }
	}
}

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
		public int Enc_Id { get; set; }

		/// <summary>
		/// Obtiene o establece el ID de la persona.
		/// </summary>
		[Display(Name = "Persona")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Per_Id { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del encargado.
		/// </summary>
		public string Enc_Nombre { get; set; }

		/// <summary>
		/// Obtiene o establece la ocupación del encargado.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Ocupación")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Enc_Ocupacion { get; set; }

		/// <summary>
		/// Obtiene o establece la identidad de la persona.
		/// </summary>
		[StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 dígitos")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		[Display(Name = "Identidad")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_Identidad { get; set; }

		/// <summary>
		/// Obtiene o establece el primer nombre de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Primer nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_PrimerNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el segundo nombre de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Segundo nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_SegundoNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido paterno de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido paterno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_ApellidoPaterno { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido materno de la persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido materno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_ApellidoMaterno { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de nacimiento de la persona.
		/// </summary>
		[Display(Name = "Fecha de nacimiento")]
		[Required(ErrorMessage = "El campo es requerido")]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Per_FechaNacimiento { get; set; }

		/// <summary>
		/// Obtiene o establece el correo electrónico de la persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Correo electrónico")]
		[EmailAddress(ErrorMessage = "Ingrese un correo válido")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_CorreoElectronico { get; set; }

		/// <summary>
		/// Obtiene o establece el teléfono de la persona.
		/// </summary>
		[Display(Name = "Teléfono")]
		[Required(ErrorMessage = "El campo es requerido")]
		[StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe tener un mínimo de 8 dígitos y un máximo de 11")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		public string Per_Telefono { get; set; }

		/// <summary>
		/// Obtiene o establece la dirección de la persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_Direccion { get; set; }

		/// <summary>
		/// Obtiene o establece el sexo de la persona.
		/// </summary>
		[StringLength(1)]
		[Display(Name = "Sexo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_Sexo { get; set; }

		/// <summary>
		/// Obtiene o establece el estado de actividad del encargado.
		/// </summary>
		[Display(Name = "Es activo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string EsActivo { get; set; }

		/// <summary>
		/// Obtiene o establece un valor que indica si la persona está activa.
		/// </summary>
		public bool Per_EsActivo { get; set; }

		/// <summary>
		/// Obtiene o establece el ID del usuario que registró la persona.
		/// </summary>
		[Display(Name = "Usuario que registra ID")]
		public int Per_UsuarioRegistra { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que registró la persona.
		/// </summary>
		[Display(Name = "Usuario que registra Nombre")]
		public string Per_UsuarioRegistraNombre { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de registro de la persona.
		/// </summary>
		[Display(Name = "Fecha de registro")]
		public DateTime Per_FechaRegistra { get; set; }

		/// <summary>
		/// Obtiene o establece el ID del usuario que modificó la persona.
		/// </summary>
		[Display(Name = "Usuario que modifica ID")]
		public int Per_UsuarioModifica { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que modificó la persona.
		/// </summary>
		[Display(Name = "Usuario que modifica Nombre")]
		public string Per_UsuarioModificaNombre { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de modificación de la persona.
		/// </summary>
		[Display(Name = "Fecha de modificación")]
		public DateTime? Per_FechaModifica { get; set; }
	}
}
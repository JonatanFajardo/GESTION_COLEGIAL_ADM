using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
	/// <summary>
	/// Representa el modelo de vista para la entidad Alumno.
	/// </summary>
	public class AlumnoViewModel : BaseViewModel
	{
		/// <summary>
		/// Obtiene o establece el Id del Alumno.
		/// </summary>
		[Key]
		public int AlumnoId { get; set; }

		/// <summary>
		/// Obtiene o establece el Id de la Persona.
		/// </summary>
		[Display(Name = "Persona")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int PersonaId { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Nivel educativo.
		/// </summary>
		[Display(Name = "Nivel educativo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int NivelId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Nivel educativo.
		/// </summary>
		public string DescripcionNivel { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Curso nivel.
		/// </summary>
		[Display(Name = "Curso nivel")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int CursoNivelId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Curso nivel.
		/// </summary>
		public string DescripcionCursoNivel { get; set; }

		/// <summary>
		/// Obtiene o establece el Id de la Modalidad.
		/// </summary>
		[Display(Name = "Modalidad")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int ModalidadId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción de la Modalidad.
		/// </summary>
		public string DescripcionModalidad { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Curso.
		/// </summary>
		public string NombreCurso { get; set; }

		/// <summary>
		/// Obtiene o establece el anio cursado.
		/// </summary>
		public int? AnioCursado { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Curso.
		/// </summary>
		[Display(Name = "Curso")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int CursoId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Curso.
		/// </summary>
		public string DescripcionCurso { get; set; }

		/// <summary>
		/// Obtiene o establece el Id de la Sección.
		/// </summary>
		[Display(Name = "Secciones")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int SeccionId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción de la Sección.
		/// </summary>
		public string DescripcionSeccion { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del Alumno.
		/// </summary>
		public string NombreAlumno { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Estado.
		/// </summary>
		[Display(Name = "Estado")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int EstadoId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Estado.
		/// </summary>
		public string DescripcionEstado { get; set; }

		//Persona

		/// <summary>
		/// Obtiene o establece la imagen.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Imagen")]
		//[Required(ErrorMessage = "El campo es requerido")]
		public string ImagenPersona { get; set; }

		/// <summary>
		/// Obtiene o establece la identidad de la Persona.
		/// </summary>
		[StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 dígitos")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		[Display(Name = "Identidad")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string NumeroIdentidad { get; set; }

		/// <summary>
		/// Obtiene o establece el primer nombre de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Primer nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string PrimerNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el segundo nombre de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Segundo nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string SegundoNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido paterno de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido paterno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string ApellidoPaterno { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido materno de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido materno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string ApellidoMaterno { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de nacimiento de la Persona.
		/// </summary>
		[Display(Name = "Fecha nacimiento")]
		[Required(ErrorMessage = "El campo es requerido")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		[DataType(DataType.Date)]
		public DateTime FechaNacimiento { get; set; }

		/// <summary>
		/// Obtiene o establece el correo electrónico de la Persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Correo electrónico")]
		[EmailAddress(ErrorMessage = "Ingrese un correo válido")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string CorreoElectronico { get; set; }

		/// <summary>
		/// Obtiene o establece el teléfono de la Persona.
		/// </summary>
		[Display(Name = "Teléfono")]
		[Required(ErrorMessage = "El campo es requerido")]
		[StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe tener mínimo 8 dígitos y como máximo 11")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		public string Telefono { get; set; }

		/// <summary>
		/// Obtiene o establece la dirección de la Persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Direccion { get; set; }

		/// <summary>
		/// Obtiene o establece el sexo de la Persona.
		/// </summary>
		[StringLength(1)]
		[Display(Name = "Sexo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Sexo { get; set; }

		/// <summary>
		/// Obtiene o establece si la Persona está activa.
		/// </summary>
		[Display(Name = "Es activo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public bool EsActivoPersona { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del usuario que registró la Persona.
		/// </summary>
		[Display(Name = "Usuario registra Id")]
		public int UsuarioRegistraPersonaId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que registró la Persona.
		/// </summary>
		[Display(Name = "Usuario registra Nombre")]
		public string NombreUsuarioRegistraPersona { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de registro de la Persona.
		/// </summary>
		[Display(Name = "Fecha registra")]
		public DateTime FechaRegistroPersona { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del usuario que modificó la Persona.
		/// </summary>
		[Display(Name = "Usuario modifica Id")]
		public int UsuarioModificaPersonaId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que modificó la Persona.
		/// </summary>
		[Display(Name = "Usuario modifica Nombre")]
		public string NombreUsuarioModificaPersona { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de modificación de la Persona.
		/// </summary>
		[Display(Name = "Fecha modifica")]
		public DateTime? FechaModificacionPersona { get; set; }

		/// <summary>
		/// Propiedad con listado de niveles educativos.
		/// </summary>
		public SelectList NivelesEducativosList { get; set; }

		/// <summary>
		/// Propiedad con listado de cursos niveles.
		/// </summary>
		public IEnumerable<SelectListItem> CursosNivelesList { get; set; }

		/// <summary>
		/// Propiedad con listado de modalidades.
		/// </summary>
		public IEnumerable<SelectListItem> ModalidadesList { get; set; }

		/// <summary>
		/// Propiedad con listado de cursos.
		/// </summary>
		public IEnumerable<SelectListItem> CursosList { get; set; }

		/// <summary>
		/// Propiedad con listado de secciones.
		/// </summary>
		public IEnumerable<SelectListItem> SeccionesList { get; set; }

		/// <summary>
		/// Propiedad con listado de estados.
		/// </summary>
		public SelectList EstadosList { get; set; }

		#region Dropdown

		/// <summary>
		/// Carga los elementos desplegables para las listas desplegables.
		/// </summary>
		/// <param name="nivelEducativoDropdownResults">Lista de resultados de nivel educativo.</param>
		/// <param name="estadoDropdownResults">Lista de resultados de estado.</param>
		public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoDropdownResults,
			IEnumerable<EstadoViewModel> estadoDropdownResults)
		{
			EstadosList = new SelectList(estadoDropdownResults, "EstadoId", "DescripcionEstado");
			NivelesEducativosList = new SelectList(nivelEducativoDropdownResults, "NivelId", "DescripcionNivel");
		}

		#endregion Dropdown
	}
}

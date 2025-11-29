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
		public int Alu_Id { get; set; }

		/// <summary>
		/// Obtiene o establece el Id de la Persona.
		/// </summary>
		[Display(Name = "Persona")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Per_Id { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Nivel educativo.
		/// </summary>
		[Display(Name = "Nivel educativo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Niv_Id { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Nivel educativo.
		/// </summary>
		public string Niv_Descripcion { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Curso nivel.
		/// </summary>
		[Display(Name = "Curso nivel")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Cun_Id { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Curso nivel.
		/// </summary>
		public string Cun_Descripcion { get; set; }

		/// <summary>
		/// Obtiene o establece el Id de la Modalidad.
		/// </summary>
		[Display(Name = "Modalidad")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Mda_Id { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción de la Modalidad.
		/// </summary>
		public string Mda_Descripcion { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Curso.
		/// </summary>
		public string Cno_Descripcion { get; set; }

		/// <summary>
		/// Obtiene o establece el anio cursado.
		/// </summary>
		public int? AnioCursado { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Curso.
		/// </summary>
		[Display(Name = "Curso")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Cur_Id { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Curso.
		/// </summary>
		public string Cur_Descripcion { get; set; }

		/// <summary>
		/// Obtiene o establece el Id de la Sección.
		/// </summary>
		[Display(Name = "Secciones")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Sec_Id { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción de la Sección.
		/// </summary>
		public string Sec_Descripcion { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del Alumno.
		/// </summary>
		public string Alu_Nombre { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del Estado.
		/// </summary>
		[Display(Name = "Estado")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int Est_Id { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del Estado.
		/// </summary>
		public string Est_Descripcion { get; set; }

		//Persona

		/// <summary>
		/// Obtiene o establece la imagen.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Imagen")]
		//[Required(ErrorMessage = "El campo es requerido")]
		public string Per_Imagen { get; set; }

		/// <summary>
		/// Obtiene o establece la identidad de la Persona.
		/// </summary>
		[StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 dígitos")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		[Display(Name = "Identidad")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_Identidad { get; set; }

		/// <summary>
		/// Obtiene o establece el primer nombre de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Primer nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_PrimerNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el segundo nombre de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Segundo nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_SegundoNombre { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido paterno de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido paterno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_ApellidoPaterno { get; set; }

		/// <summary>
		/// Obtiene o establece el apellido materno de la Persona.
		/// </summary>
		[StringLength(50)]
		[Display(Name = "Apellido materno")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_ApellidoMaterno { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de nacimiento de la Persona.
		/// </summary>
		[Display(Name = "Fecha nacimiento")]
		[Required(ErrorMessage = "El campo es requerido")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
		[DataType(DataType.Date)]
		public DateTime Per_FechaNacimiento { get; set; }

		/// <summary>
		/// Obtiene o establece el correo electrónico de la Persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Correo electrónico")]
		[EmailAddress(ErrorMessage = "Ingrese un correo válido")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_CorreoElectronico { get; set; }

		/// <summary>
		/// Obtiene o establece el teléfono de la Persona.
		/// </summary>
		[Display(Name = "Teléfono")]
		[Required(ErrorMessage = "El campo es requerido")]
		[StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe tener mínimo 8 dígitos y como máximo 11")]
		[RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
		public string Per_Telefono { get; set; }

		/// <summary>
		/// Obtiene o establece la dirección de la Persona.
		/// </summary>
		[StringLength(150)]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_Direccion { get; set; }

		/// <summary>
		/// Obtiene o establece el sexo de la Persona.
		/// </summary>
		[StringLength(1)]
		[Display(Name = "Sexo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string Per_Sexo { get; set; }

		/// <summary>
		/// Obtiene o establece si la Persona está activa.
		/// </summary>
		[Display(Name = "Es activo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public bool Per_EsActivo { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del usuario que registró la Persona.
		/// </summary>
		[Display(Name = "Usuario registra Id")]
		public int Per_UsuarioRegistra { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que registró la Persona.
		/// </summary>
		[Display(Name = "Usuario registra Nombre")]
		public string Per_UsuarioRegistraNombre { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de registro de la Persona.
		/// </summary>
		[Display(Name = "Fecha registra")]
		public DateTime Per_FechaRegistra { get; set; }

		/// <summary>
		/// Obtiene o establece el Id del usuario que modificó la Persona.
		/// </summary>
		[Display(Name = "Usuario modifica Id")]
		public int Per_UsuarioModifica { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que modificó la Persona.
		/// </summary>
		[Display(Name = "Usuario modifica Nombre")]
		public string Per_UsuarioModificaNombre { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha de modificación de la Persona.
		/// </summary>
		[Display(Name = "Fecha modifica")]
		public DateTime? Per_FechaModifica { get; set; }

        #region Dropdown

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


		/// <summary>
		/// Carga los elementos desplegables para las listas desplegables.
		/// </summary>
		/// <param name="nivelEducativoDropdownResults">Lista de resultados de nivel educativo.</param>
		/// <param name="estadoDropdownResults">Lista de resultados de estado.</param>
		public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoDropdownResults,
			IEnumerable<EstadoViewModel> estadoDropdownResults)
		{
			EstadosList = new SelectList(estadoDropdownResults, "Est_Id", "Est_Descripcion");
			NivelesEducativosList = new SelectList(nivelEducativoDropdownResults, "Niv_Id", "Niv_Descripcion");
		}

		#endregion Dropdown
	}
}
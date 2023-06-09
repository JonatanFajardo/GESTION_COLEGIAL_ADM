using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista para buscar un alumno.
    /// </summary>
    public class AlumnoFindViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del alumno.
        /// </summary>
        [Key]
        public int Alu_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la persona.
        /// </summary>
        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Per_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del nivel educativo.
        /// </summary>
        [Display(Name = "Nivel educativo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Niv_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del nivel educativo.
        /// </summary>
        public string Niv_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del curso nivel.
        /// </summary>
        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cun_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del curso nivel.
        /// </summary>
        public string Cun_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la modalidad.
        /// </summary>
        [Display(Name = "Modalidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Mda_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la modalidad.
        /// </summary>
        public string Mda_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del curso nivel.
        /// </summary>
        public string Cno_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del curso.
        /// </summary>
        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cur_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del curso.
        /// </summary>
        public string Cur_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la sección.
        /// </summary>
        [Display(Name = "Secciones")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sec_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la sección.
        /// </summary>
        public string Sec_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del alumno.
        /// </summary>
        public string Alu_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del estado.
        /// </summary>
        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Est_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del estado.
        /// </summary>
        public string Est_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la identidad de la persona.
        /// </summary>
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 dígitos")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        private PersonaViewModel Persona { get; set; }

        /// <summary>
        /// Obtiene o establece la identidad de la persona.
        /// </summary>
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
        [Display(Name = "Fecha nacimiento")]
        [Required(ErrorMessage = "El campo es requerido")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DataType(DataType.Date)]
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
        [StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe tener mínimo 8 dígitos y como máximo 11")]
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
        /// Obtiene o establece un valor que indica si la persona está activa.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool Per_EsActivo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registra a la persona.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Per_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registra a la persona.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Per_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la persona.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Per_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modifica a la persona.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Per_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modifica a la persona.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Per_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la persona.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Per_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el listado de niveles educativos.
        /// </summary>
        public SelectList NivelesEducativosList { get; set; }

        /// <summary>
        /// Obtiene o establece el listado de cursos niveles.
        /// </summary>
        public IEnumerable<SelectListItem> CursosNivelesList { get; set; }

        /// <summary>
        /// Obtiene o establece el listado de modalidades.
        /// </summary>
        public IEnumerable<SelectListItem> ModalidadesList { get; set; }

        /// <summary>
        /// Obtiene o establece el listado de cursos.
        /// </summary>
        public IEnumerable<SelectListItem> CursosList { get; set; }

        /// <summary>
        /// Obtiene o establece el listado de secciones.
        /// </summary>
        public IEnumerable<SelectListItem> SeccionesList { get; set; }

        /// <summary>
        /// Obtiene o establece el listado de estados.
        /// </summary>
        public SelectList EstadosList { get; set; }

        #region Dropdown

        /// <summary>
        /// Carga los dropdown.
        /// </summary>
        /// <param name="nivelEducativoDropdownResults"></param>
        /// <param name="estadoDropdownResults"></param>
        public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoDropdownResults,
            IEnumerable<EstadoViewModel> estadoDropdownResults)
        {
            EstadosList = new SelectList(estadoDropdownResults, "Est_Id", "Est_Descripcion");
            NivelesEducativosList = new SelectList(nivelEducativoDropdownResults, "Niv_Id", "Niv_Descripcion");
        }

        #endregion Dropdown
    }
}   
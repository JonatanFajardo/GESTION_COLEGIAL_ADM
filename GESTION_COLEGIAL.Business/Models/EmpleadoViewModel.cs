using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para un empleado.
    /// </summary>
    public class EmpleadoViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del empleado.
        /// </summary>
        [Key]
        public int EmpleadoId { get; set; }

        /// <summary>
        /// Obtiene o establece el código del empleado.
        /// </summary>
        [StringLength(50, ErrorMessage = "El número máximo de dígitos permitidos es 50")]
        [Display(Name = "Código")]
        [RegularExpression("([0-9]*)", ErrorMessage = "El campo debe ser numérico")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string CodigoEmpleado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int PersonaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del empleado.
        /// </summary>
        public string NombreEmpleado { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del título del empleado.
        /// </summary>
        public string DescripcionTitulo { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del cargo del empleado.
        /// </summary>
        public string DescripcionCargo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del título del empleado.
        /// </summary>
        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int TituloId { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del cargo del empleado.
        /// </summary>
        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int CargoId { get; set; }

        /// <summary>
        /// Obtiene o establece la identidad de la persona asociada al empleado.
        /// </summary>
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 dígitos")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string NumeroIdentidad { get; set; }

        /// <summary>
        /// Obtiene o establece el primer nombre de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Primer nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string PrimerNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el segundo nombre de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Segundo nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string SegundoNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido paterno de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Apellido paterno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string ApellidoPaterno { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido materno de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Apellido materno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string ApellidoMaterno { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage = "El campo es requerido")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        /// <summary>
        /// Obtiene o establece el correo electrónico de la persona asociada al empleado.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Obtiene o establece el teléfono de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe tener mínimo 8 dígitos y como máximo 11")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
        public string Telefono { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección de la persona asociada al empleado.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Direccion { get; set; }

        /// <summary>
        /// Obtiene o establece el sexo de la persona asociada al empleado.
        /// </summary>
        [StringLength(1)]
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Sexo { get; set; }

        /// <summary>
        /// Obtiene o establece el estado de actividad del empleado.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string EsActivo { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la persona asociada al empleado está activa.
        /// </summary>
        public bool EsActivoPersona { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int UsuarioRegistraPersonaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que registra nombre")]
        public string NombreUsuarioRegistraPersona { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistroPersona { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? UsuarioModificaPersonaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que modifica nombre")]
        public string NombreUsuarioModificaPersona { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? FechaModificacionPersona { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de títulos.
        /// </summary>
        public SelectList titulosList { get; set; }

        /// <summary>
        /// Obtiene o establece la lista desplegable de cargos.
        /// </summary>
        public SelectList cargosList { get; set; }

        /// <summary>
        /// Carga las listas desplegables de títulos y cargos.
        /// </summary>
        /// <param name="tituloDropdownResults">Los resultados de la lista desplegable de títulos.</param>
        /// <param name="cargoDropdownResults">Los resultados de la lista desplegable de cargos.</param>
        public void LoadDropDownList(IEnumerable<TituloViewModel> tituloDropdownResults,
                                    IEnumerable<CargoViewModel> cargoDropdownResults)
        {
            titulosList = new SelectList(tituloDropdownResults, "TituloId", "DescripcionTitulo");
            cargosList = new SelectList(cargoDropdownResults, "CargoId", "DescripcionCargo");
        }
    }
}

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
        public int Emp_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código del empleado.
        /// </summary>
        [StringLength(50, ErrorMessage = "El número máximo de dígitos permitidos es 50")]
        [Display(Name = "Código")]
        [RegularExpression("([0-9]*)", ErrorMessage = "El campo debe ser numérico")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Emp_Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Per_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del empleado.
        /// </summary>
        public string Emp_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del título del empleado.
        /// </summary>
        public string Tit_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del cargo del empleado.
        /// </summary>
        public string Car_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del título del empleado.
        /// </summary>
        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Tit_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del cargo del empleado.
        /// </summary>
        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Car_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la identidad de la persona asociada al empleado.
        /// </summary>
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El campo debe contener 13 dígitos")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_Identidad { get; set; }

        /// <summary>
        /// Obtiene o establece el primer nombre de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Primer nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_PrimerNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el segundo nombre de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Segundo nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_SegundoNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido paterno de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Apellido paterno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_ApellidoPaterno { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido materno de la persona asociada al empleado.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Apellido materno")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_ApellidoMaterno { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de nacimiento de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Fecha de nacimiento")]
        [Required(ErrorMessage = "El campo es requerido")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Per_FechaNacimiento { get; set; }

        /// <summary>
        /// Obtiene o establece el correo electrónico de la persona asociada al empleado.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_CorreoElectronico { get; set; }

        /// <summary>
        /// Obtiene o establece el teléfono de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El campo es requerido")]
        [StringLength(11, MinimumLength = 8, ErrorMessage = "El campo debe tener mínimo 8 dígitos y como máximo 11")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "El campo debe ser numérico")]
        public string Per_Telefono { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección de la persona asociada al empleado.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_Direccion { get; set; }

        /// <summary>
        /// Obtiene o establece el sexo de la persona asociada al empleado.
        /// </summary>
        [StringLength(1)]
        [Display(Name = "Sexo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Per_Sexo { get; set; }

        /// <summary>
        /// Obtiene o establece el estado de actividad del empleado.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string EsActivo { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la persona asociada al empleado está activa.
        /// </summary>
        public bool Per_EsActivo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int Per_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que registra nombre")]
        public string Per_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime Per_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? Per_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó a la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Usuario que modifica nombre")]
        public string Per_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la persona asociada al empleado.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? Per_FechaModifica { get; set; }

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
            titulosList = new SelectList(tituloDropdownResults, "Tit_Id", "Tit_Descripcion");
            cargosList = new SelectList(cargoDropdownResults, "Car_Id", "Car_Descripcion");
        }
    }
}
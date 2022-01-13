using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class EmpleadoViewModel : BaseViewModel
    {  
        [Key]
        public int Emp_Id { get; set; }

        [StringLength(50, ErrorMessage ="El numero maximo de digitos permitidos es 50")]
        [Display(Name = "Código")]
        [RegularExpression("([0-9]*)", ErrorMessage = "El campo debe debe ser numerico")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Emp_Codigo { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Per_Id { get; set; }

        public string Emp_Nombre { get; set; }
        public string Tit_Descripcion { get; set; }
        public string Car_Descripcion { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Tit_Id { get; set; }

        [Display(Name = "Cargo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Car_Id { get; set; }

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
        //Indica la forma en que se muestra. (Before: 00/00/00 00:00:00, after: 00/00/00)
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
        public string EsActivo { get; set; }
        public bool Per_EsActivo { get; set; }

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

        // Propiedad con listado de títulos.
        public SelectList titulosList { get; set; }

        // Propiedad con listado de cargos.
        public SelectList cargosList { get; set; }      

        #region Dropdown
        /// <summary>
        /// Carga los datos solicitados.
        /// </summary>
        /// <remarks>
        /// Permite mostrar una colección de dropdown.
        /// </remarks>
        /// <param name="tituloDropdownResults"></param>
        /// <param name="cargoDropdownResults"></param>
        public void LoadDropDownList(IEnumerable<TituloViewModel> tituloDropdownResults,
                                    IEnumerable<CargoViewModel> cargoDropdownResults)
        {
            titulosList = new SelectList(tituloDropdownResults, "Tit_Id", "Tit_Descripcion");
            cargosList = new SelectList(cargoDropdownResults, "Car_Id", "Car_Descripcion");
        }
        #endregion Dropdown
    }
}
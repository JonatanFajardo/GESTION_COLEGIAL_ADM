using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class AlumnoViewModel : BaseViewModel
    {

        [Key]
        public int Alu_Id { get; set; }

        [Display(Name = "Persona")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Per_Id { get; set; }

        [Display(Name = "Nivel educativo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }

        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cun_Id { get; set; }
        public string Cun_Descripcion { get; set; }

        [Display(Name = "Modalidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Mda_Id { get; set; }
        public string Mda_Descripcion { get; set; }

        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cur_Id { get; set; }
        public string Cur_Descripcion { get; set; }

        [Display(Name = "Secciones")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sec_Id { get; set; }
        public string Sec_Descripcion { get; set; }

        public string Alu_Nombre { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Est_Id { get; set; }
        public string Est_Descripcion { get; set; }

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

        // Propiedad con listado de niveles educativos.
        public SelectList NivelesEducativosList { get; set; }

        // Propiedad con listado de cursos niveles.
        public IEnumerable<SelectListItem> CursosNivelesList { get; set; }

        // Propiedad con listado de modalidades.
        public IEnumerable<SelectListItem> ModalidadesList { get; set; }

        // Propiedad con listado de cursos.
        public IEnumerable<SelectListItem> CursosList { get; set; }

        // Propiedad con listado de secciones.
        public IEnumerable<SelectListItem> SeccionesList { get; set; }

        // Propiedad con listado de estados.
        public SelectList EstadosList { get; set; }

        #region Dropdown

        public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoDropdownResults, 
            IEnumerable<EstadoViewModel> estadoDropdownResults)
        {
            EstadosList = new SelectList(estadoDropdownResults, "Est_Id", "Est_Descripcion");
            NivelesEducativosList = new SelectList(nivelEducativoDropdownResults, "Niv_Id", "Niv_Descripcion");
        }


        //public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoDropdownResults,
        //                            IEnumerable<CursoNivelViewModel> cursoNivelDropdownResults,
        //                            IEnumerable<ModalidadViewModel> modalidadDropdownResults,
        //                            IEnumerable<CursoViewModel> cursoDropdownResults,
        //                            IEnumerable<SeccionViewModel> seccionDropdownResults,
        //                            IEnumerable<EstadoViewModel> estadoDropdownResults)
        //{
        //    NivelEducativoList = new SelectList(nivelEducativoDropdownResults, "Niv_Id", "Niv_Descripcion");
        //    CursosNivelesList = new SelectList(cursoNivelDropdownResults, "Cun_Id", "Cun_Descripcion");
        //    ModalidadesList = new SelectList(modalidadDropdownResults, "Mda_Id", "Mda_Descripcion");
        //    CursosList = new SelectList(cursoDropdownResults, "Cur_Id", "Cur_Descripcion");
        //    SeccionesList = new SelectList(seccionDropdownResults, "Sec_Id", "Sec_Descripcion");
        //    EstadosList = new SelectList(estadoDropdownResults, "Est_Id", "Est_Descripcion");
        //}
        #endregion Dropdown
    }
}
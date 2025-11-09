using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de la entidad "Materia"
    /// </summary>
    public class MateriaViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador de la materia.
        /// </summary>
        [Key]
        public int MateriaId { get; set; }

        /// <summary>
        /// Nombre de la materia.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Materias", HttpMethod = "POST", AdditionalFields = nameof(MateriaId) + "," + nameof(NombreMateria))]
        public string NombreMateria { get; set; }

        /// <summary>
        /// Identificador de la duración de la materia.
        /// </summary>
        [Display(Name = "Duración")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int DuracionId { get; set; }

        /// <summary>
        /// Indica si la materia está activa.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool EsActivoMateria { get; set; }

        /// <summary>
        /// Representación en cadena del estado de activo.
        /// </summary>
        public string EsActivo { get; set; }

        /// <summary>
        /// Identificador del usuario que registra la materia.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraMateriaId { get; set; }

        /// <summary>
        /// Nombre del usuario que registra la materia.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraMateria { get; set; }

        /// <summary>
        /// Fecha de registro de la materia.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroMateria { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica la materia.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaMateriaId { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica la materia.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaMateria { get; set; }

        /// <summary>
        /// Fecha de modificación de la materia.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionMateria { get; set; }

        /// <summary>
        /// Indica si la materia está seleccionada.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}


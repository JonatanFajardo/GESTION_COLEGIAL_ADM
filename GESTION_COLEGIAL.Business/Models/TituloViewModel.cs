using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// ViewModel para la entidad Titulo.
    /// </summary>
    public class TituloViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador del título.
        /// </summary>
        [Key]
        public int TituloId { get; set; }

        /// <summary>
        /// Descripción del título.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Titulos", HttpMethod = "POST", AdditionalFields = nameof(TituloId) + "," + nameof(DescripcionTitulo))] // Validación remota para verificar la existencia
        public string DescripcionTitulo { get; set; }

        /// <summary>
        /// Identificador del usuario que registra el título.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraTituloId { get; set; }

        /// <summary>
        /// Nombre del usuario que registra el título.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraTitulo { get; set; }

        /// <summary>
        /// Fecha de registro del título.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroTitulo { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica el título.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaTituloId { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica el título.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaTitulo { get; set; }

        /// <summary>
        /// Fecha de modificación del título.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionTitulo { get; set; }
    }
}


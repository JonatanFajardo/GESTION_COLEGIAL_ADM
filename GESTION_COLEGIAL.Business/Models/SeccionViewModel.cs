using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista de una sección.
    /// </summary>
    public class SeccionViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la sección.
        /// </summary>
        [Key]
        public int SeccionId { get; set; }

        /// <summary>
        /// Descripción de la sección.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Secciones", HttpMethod = "POST", AdditionalFields = nameof(SeccionId) + "," + nameof(DescripcionSeccion))]
        public string DescripcionSeccion { get; set; }

        /// <summary>
        /// Identificador del usuario que registra la sección.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraSeccionId { get; set; }

        /// <summary>
        /// Nombre del usuario que registra la sección.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraSeccion { get; set; }

        /// <summary>
        /// Fecha de registro de la sección.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroSeccion { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica la sección.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaSeccionId { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica la sección.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaSeccion { get; set; }

        /// <summary>
        /// Fecha de modificación de la sección.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionSeccion { get; set; }

        public bool IsSelected { get; set; }
    }
}


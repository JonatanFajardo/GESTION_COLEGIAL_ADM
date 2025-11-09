using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de un estado.
    /// </summary>
    public class EstadoViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del estado.
        /// </summary>
        [Key]
        public int EstadoId { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del estado.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Estados", HttpMethod = "POST", AdditionalFields = nameof(EstadoId) + "," + nameof(DescripcionEstado))]
        public string DescripcionEstado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el estado.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int UsuarioRegistraEstadoId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el estado.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string NombreUsuarioRegistraEstado { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del estado.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistroEstado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el estado.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? UsuarioModificaEstadoId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el estado.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string NombreUsuarioModificaEstado { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del estado.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? FechaModificacionEstado { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista para la entidad "Duración"
    /// </summary>
    public class DuracionViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la duración.
        /// </summary>
        [Key]
        public int DuracionId { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la duración.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Duraciones", HttpMethod = "POST", AdditionalFields = nameof(DuracionId) + "," + nameof(DescripcionDuracion))]
        public string DescripcionDuracion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró la duración.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int UsuarioRegistraDuracionId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró la duración.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string NombreUsuarioRegistraDuracion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la duración.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistroDuracion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó la duración.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? UsuarioModificaDuracionId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó la duración.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string NombreUsuarioModificaDuracion { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la duración.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? FechaModificacionDuracion { get; set; }
    }
}

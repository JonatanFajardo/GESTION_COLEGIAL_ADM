using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de un día.
    /// </summary>
    public class DiaViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del día.
        /// </summary>
        [Key]
        public int DiaId { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del día.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Dias", HttpMethod = "POST", AdditionalFields = nameof(DiaId) + "," + nameof(DescripcionDia))]
        public string DescripcionDia { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el día.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int UsuarioRegistraDiaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el día.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string NombreUsuarioRegistraDia { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del día.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistroDia { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el día.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? UsuarioModificaDiaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el día.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string NombreUsuarioModificaDia { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del día.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? FechaModificacionDia { get; set; }
    }
}

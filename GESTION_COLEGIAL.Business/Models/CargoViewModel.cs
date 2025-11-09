using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista de un cargo.
    /// </summary>
    public class CargoViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del cargo.
        /// </summary>
        [Key]
        public int CargoId { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del cargo.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Cargos", HttpMethod = "POST", AdditionalFields = nameof(CargoId) + "," + nameof(DescripcionCargo))]
        public string DescripcionCargo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el cargo.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraCargoId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el cargo.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraCargo { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del cargo.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroCargo { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el cargo.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaCargoId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el cargo.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaCargo { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del cargo.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionCargo { get; set; }
    }
}

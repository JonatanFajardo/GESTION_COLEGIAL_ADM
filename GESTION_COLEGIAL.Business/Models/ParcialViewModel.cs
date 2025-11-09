using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de un parcial.
    /// </summary>
    public class ParcialViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador del parcial.
        /// </summary>
        [Key]
        public int ParcialId { get; set; }

        /// <summary>
        /// Descripción del parcial.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Parciales", HttpMethod = "POST", AdditionalFields = nameof(ParcialId) + "," + nameof(DescripcionParcial))]
        public string DescripcionParcial { get; set; }

        /// <summary>
        /// Identificador del usuario que registra el parcial.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraParcialId { get; set; }

        /// <summary>
        /// Nombre del usuario que registra el parcial.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraParcial { get; set; }

        /// <summary>
        /// Fecha de registro del parcial.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroParcial { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica el parcial.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaParcialId { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica el parcial.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaParcial { get; set; }

        /// <summary>
        /// Fecha de modificación del parcial.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionParcial { get; set; }
    }
}


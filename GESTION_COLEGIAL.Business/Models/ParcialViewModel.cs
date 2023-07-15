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
        public int Pac_Id { get; set; }

        /// <summary>
        /// Descripción del parcial.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Parciales", HttpMethod = "POST", AdditionalFields = nameof(Pac_Id) + "," + nameof(Pac_Descripcion))]
        public string Pac_Descripcion { get; set; }

        /// <summary>
        /// Identificador del usuario que registra el parcial.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Pac_UsuarioRegistra { get; set; }

        /// <summary>
        /// Nombre del usuario que registra el parcial.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Pac_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Fecha de registro del parcial.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Pac_FechaRegistra { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica el parcial.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Pac_UsuarioModifica { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica el parcial.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Pac_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Fecha de modificación del parcial.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Pac_FechaModifica { get; set; }
    }
}

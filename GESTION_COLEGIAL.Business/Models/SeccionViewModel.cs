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
        public int Sec_Id { get; set; }

        /// <summary>
        /// Descripción de la sección.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Secciones", HttpMethod = "POST", AdditionalFields = nameof(Sec_Id) + "," + nameof(Sec_Descripcion))]
        public string Sec_Descripcion { get; set; }

        /// <summary>
        /// Identificador del usuario que registra la sección.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Sec_UsuarioRegistra { get; set; }

        /// <summary>
        /// Nombre del usuario que registra la sección.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Sec_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Fecha de registro de la sección.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Sec_FechaRegistra { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica la sección.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Sec_UsuarioModifica { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica la sección.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Sec_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Fecha de modificación de la sección.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Sec_FechaModifica { get; set; }

        public bool IsSelected { get; set; }
    }
}

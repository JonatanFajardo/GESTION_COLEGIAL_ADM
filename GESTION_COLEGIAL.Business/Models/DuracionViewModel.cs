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
        public int Dur_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la duración.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Duraciones", HttpMethod = "POST", AdditionalFields = nameof(Dur_Id) + "," + nameof(Dur_Descripcion))]
        public string Dur_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró la duración.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int Dur_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró la duración.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string Dur_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la duración.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime Dur_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó la duración.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? Dur_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó la duración.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string Dur_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la duración.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? Dur_FechaModifica { get; set; }
    }
}
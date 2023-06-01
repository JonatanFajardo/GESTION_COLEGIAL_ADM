using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class HoraViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la hora.
        /// </summary>
        [Key]
        public int Hor_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la hora.
        /// </summary>
        [StringLength(11)]
        [Display(Name = "Hora")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Horas", HttpMethod = "POST", AdditionalFields = nameof(Hor_Id) + "," + nameof(Hor_Hora))]
        public string Hor_Hora { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró la hora.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int Hor_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró la hora.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string Hor_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la hora.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime Hor_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó la hora.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? Hor_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó la hora.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string Hor_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la hora.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? Hor_FechaModifica { get; set; }
    }
}
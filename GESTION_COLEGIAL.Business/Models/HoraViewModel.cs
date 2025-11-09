using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa un modelo de vista para la entidad "Hora".
    /// </summary>
    public class HoraViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID de la hora.
        /// </summary>
        [Key]
        public int HorarioId { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la hora.
        /// </summary>
        [StringLength(11)]
        [Display(Name = "Hora")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Horas", HttpMethod = "POST", AdditionalFields = nameof(HorarioId) + "," + nameof(Hora))]
        public string Hora { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró la hora.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int UsuarioRegistraHorarioId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró la hora.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string NombreUsuarioRegistraHorario { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro de la hora.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistroHorario { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó la hora.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? UsuarioModificaHorarioId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó la hora.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string NombreUsuarioModificaHorario { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación de la hora.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? FechaModificacionHorario { get; set; }
    }
}

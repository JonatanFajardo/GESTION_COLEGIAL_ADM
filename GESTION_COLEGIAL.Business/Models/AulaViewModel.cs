using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista de un aula.
    /// </summary>
    public class AulaViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del aula.
        /// </summary>
        [Key]
        public int AulaId { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del aula.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string DescripcionAula { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el aula.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraAulaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el aula.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraAula { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del aula.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroAula { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el aula.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaAulaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el aula.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaAula { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del aula.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionAula { get; set; }
    }
}

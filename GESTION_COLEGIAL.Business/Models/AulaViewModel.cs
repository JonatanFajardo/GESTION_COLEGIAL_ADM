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
        public int Aul_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del aula.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Aul_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el aula.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Aul_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el aula.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Aul_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del aula.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Aul_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el aula.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Aul_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el aula.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Aul_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del aula.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Aul_FechaModifica { get; set; }
    }
}
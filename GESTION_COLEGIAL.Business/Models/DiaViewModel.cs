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
        public int Dia_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del día.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Dias", HttpMethod = "POST", AdditionalFields = nameof(Dia_Id) + "," + nameof(Dia_Descripcion))]
        public string Dia_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el día.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int Dia_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el día.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string Dia_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del día.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime Dia_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el día.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? Dia_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el día.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string Dia_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del día.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? Dia_FechaModifica { get; set; }
    }
}
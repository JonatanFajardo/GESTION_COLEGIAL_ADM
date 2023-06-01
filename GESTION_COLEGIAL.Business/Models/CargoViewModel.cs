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
        public int Car_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del cargo.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Cargos", HttpMethod = "POST", AdditionalFields = nameof(Car_Id) + "," + nameof(Car_Descripcion))]
        public string Car_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el cargo.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Car_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el cargo.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Car_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del cargo.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Car_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el cargo.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Car_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el cargo.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Car_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del cargo.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Car_FechaModifica { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de un nivel educativo.
    /// </summary>
    public class NivelEducativoViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador del nivel educativo.
        /// </summary>
        [Key]
        public int Niv_Id { get; set; }

        /// <summary>
        /// Descripción del nivel educativo.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "NivelesEducativos", HttpMethod = "POST", AdditionalFields = nameof(Niv_Id) + "," + nameof(Niv_Descripcion))]
        public string Niv_Descripcion { get; set; }

        /// <summary>
        /// Indica si el nivel educativo está activo.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool Niv_EsActivo { get; set; }

        /// <summary>
        /// Representación en cadena del estado de activo.
        /// </summary>
        public string EsActivo { get; set; }

        /// <summary>
        /// Identificador del usuario que registra el nivel educativo.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Niv_UsuarioRegistra { get; set; }

        /// <summary>
        /// Nombre del usuario que registra el nivel educativo.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Niv_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Fecha de registro del nivel educativo.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Niv_FechaRegistra { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica el nivel educativo.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Niv_UsuarioModifica { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica el nivel educativo.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Niv_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Fecha de modificación del nivel educativo.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Niv_FechaModifica { get; set; }
    }
}

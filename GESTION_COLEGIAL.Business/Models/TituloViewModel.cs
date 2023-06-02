using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// ViewModel para la entidad Titulo.
    /// </summary>
    public class TituloViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador del título.
        /// </summary>
        [Key]
        public int Tit_Id { get; set; }

        /// <summary>
        /// Descripción del título.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Titulos", HttpMethod = "POST", AdditionalFields = nameof(Tit_Id) + "," + nameof(Tit_Descripcion))] // Validación remota para verificar la existencia
        public string Tit_Descripcion { get; set; }

        /// <summary>
        /// Identificador del usuario que registra el título.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Tit_UsuarioRegistra { get; set; }

        /// <summary>
        /// Nombre del usuario que registra el título.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Tit_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Fecha de registro del título.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Tit_FechaRegistra { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica el título.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Tit_UsuarioModifica { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica el título.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Tit_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Fecha de modificación del título.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Tit_FechaModifica { get; set; }
    }
}

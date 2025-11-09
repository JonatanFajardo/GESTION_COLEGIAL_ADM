using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ParentescoViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador del parentesco.
        /// </summary>
        [Key]
        public int ParentescoId { get; set; }

        /// <summary>
        /// Descripción del parentesco.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Parentescos", HttpMethod = "POST", AdditionalFields = nameof(ParentescoId) + "," + nameof(DescripcionParentesco))]
        public string DescripcionParentesco { get; set; }

        /// <summary>
        /// Identificador del usuario que registra el parentesco.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraParentescoId { get; set; }

        /// <summary>
        /// Nombre del usuario que registra el parentesco.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraParentesco { get; set; }

        /// <summary>
        /// Fecha de registro del parentesco.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroParentesco { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica el parentesco.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaParentescoId { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica el parentesco.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaParentesco { get; set; }

        /// <summary>
        /// Fecha de modificación del parentesco.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionParentesco { get; set; }
    }
}


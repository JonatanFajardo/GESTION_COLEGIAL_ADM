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
        public int Par_Id { get; set; }

        /// <summary>
        /// Descripción del parentesco.
        /// </summary>
        [StringLength(100)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Parentescos", HttpMethod = "POST", AdditionalFields = nameof(Par_Id) + "," + nameof(Par_Descripcion))]
        public string Par_Descripcion { get; set; }

        /// <summary>
        /// Identificador del usuario que registra el parentesco.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Par_UsuarioRegistra { get; set; }

        /// <summary>
        /// Nombre del usuario que registra el parentesco.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Par_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Fecha de registro del parentesco.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Par_FechaRegistra { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica el parentesco.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Par_UsuarioModifica { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica el parentesco.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Par_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Fecha de modificación del parentesco.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Par_FechaModifica { get; set; }
    }
}

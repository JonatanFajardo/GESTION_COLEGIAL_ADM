using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    [Table("tbEstados", Schema = "app")]
    public class EstadoViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del estado.
        /// </summary>
        [Key]
        public int Est_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del estado.
        /// </summary>
        [StringLength(150)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "Estados", HttpMethod = "POST", AdditionalFields = nameof(Est_Id) + "," + nameof(Est_Descripcion))]
        public string Est_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el estado.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int Est_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el estado.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string Est_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del estado.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime Est_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el estado.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? Est_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el estado.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string Est_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del estado.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? Est_FechaModifica { get; set; }
    }
}
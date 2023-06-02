using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de una nota.
    /// </summary>
    public class NotaViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador de la nota.
        /// </summary>
        [Key]
        public int Not_Id { get; set; }

        /// <summary>
        /// Valor de la nota.
        /// </summary>
        [Display(Name = "Nota")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Not_Nota { get; set; }

        /// <summary>
        /// Identificador de la materia asociada a la nota.
        /// </summary>
        [Display(Name = "Materia")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Mat_Id { get; set; }

        /// <summary>
        /// Identificador del semestre asociado a la nota.
        /// </summary>
        [Display(Name = "Semestre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sem_Id { get; set; }

        /// <summary>
        /// Identificador del parcial asociado a la nota.
        /// </summary>
        [Display(Name = "Parcial")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Pac_Id { get; set; }

        /// <summary>
        /// Año de la nota.
        /// </summary>
        [Column(TypeName = "Año")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime Not_Año { get; set; }

        /// <summary>
        /// Indica si la nota está activa.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool? EsActivo { get; set; }

        /// <summary>
        /// Representación en texto de si la nota está activa.
        /// </summary>
        public string Not_EsActivo { get; set; }

        /// <summary>
        /// Identificador del usuario que registra la nota.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Not_UsuarioRegistra { get; set; }

        /// <summary>
        /// Nombre del usuario que registra la nota.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Not_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Fecha de registro de la nota.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Not_FechaRegistra { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica la nota.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Not_UsuarioModifica { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica la nota.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Not_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Fecha de modificación de la nota.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Not_FechaModifica { get; set; }
    }
}

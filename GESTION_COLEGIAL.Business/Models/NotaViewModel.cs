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
        public int NotaId { get; set; }

        /// <summary>
        /// Valor de la nota.
        /// </summary>
        [Display(Name = "Nota")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int ValorNota { get; set; }

        /// <summary>
        /// Identificador de la materia asociada a la nota.
        /// </summary>
        [Display(Name = "Materia")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int MateriaId { get; set; }

        /// <summary>
        /// Identificador del semestre asociado a la nota.
        /// </summary>
        [Display(Name = "Semestre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int SemestreId { get; set; }

        /// <summary>
        /// Identificador del parcial asociado a la nota.
        /// </summary>
        [Display(Name = "Parcial")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int ParcialId { get; set; }

        /// <summary>
        /// Año de la nota.
        /// </summary>
        [Column(TypeName = "Año")]
        [Display(Name = "")]
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime AnioNota { get; set; }

        /// <summary>
        /// Indica si la nota está activa.
        /// </summary>
        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public bool? EsActivo { get; set; }

        /// <summary>
        /// Representación en texto de si la nota está activa.
        /// </summary>
        public string EsActivoNota { get; set; }

        /// <summary>
        /// Identificador del usuario que registra la nota.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int UsuarioRegistraNotaId { get; set; }

        /// <summary>
        /// Nombre del usuario que registra la nota.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string NombreUsuarioRegistraNota { get; set; }

        /// <summary>
        /// Fecha de registro de la nota.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime FechaRegistroNota { get; set; }

        /// <summary>
        /// Identificador del usuario que modifica la nota.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? UsuarioModificaNotaId { get; set; }

        /// <summary>
        /// Nombre del usuario que modifica la nota.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string NombreUsuarioModificaNota { get; set; }

        /// <summary>
        /// Fecha de modificación de la nota.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? FechaModificacionNota { get; set; }
    }
}



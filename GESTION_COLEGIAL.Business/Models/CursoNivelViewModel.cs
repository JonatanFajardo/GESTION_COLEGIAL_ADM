using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista de un curso nivel.
    /// </summary>
    public class CursoNivelViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del curso nivel.
        /// </summary>
        [Key]
        public int Cun_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del curso nivel.
        /// </summary>
        [StringLength(50)]
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Remote(action: "ExistAsync", controller: "CursosNiveles", HttpMethod = "POST", AdditionalFields = nameof(Cun_Id) + "," + nameof(Cun_Descripcion))]
        public string Cun_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del nivel educativo asociado al curso nivel.
        /// </summary>
        public int Niv_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del nivel educativo asociado al curso nivel.
        /// </summary>
        public string Niv_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el curso nivel.
        /// </summary>
        [Display(Name = "Usuario registra Id")]
        public int Cun_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el curso nivel.
        /// </summary>
        [Display(Name = "Usuario registra Nombre")]
        public string Cun_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que se registró el curso nivel.
        /// </summary>
        [Display(Name = "Fecha registra")]
        public DateTime Cun_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el curso nivel.
        /// </summary>
        [Display(Name = "Usuario modifica Id")]
        public int? Cun_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el curso nivel.
        /// </summary>
        [Display(Name = "Usuario modifica Nombre")]
        public string Cun_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que se modificó el curso nivel.
        /// </summary>
        [Display(Name = "Fecha modifica")]
        public DateTime? Cun_FechaModifica { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si el curso nivel está seleccionado.
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
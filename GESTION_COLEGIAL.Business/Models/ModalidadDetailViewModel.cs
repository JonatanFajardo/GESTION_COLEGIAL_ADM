using System;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Modelo de vista para el detalle de la entidad "Modalidad" con campos de auditoría
    /// </summary>
    public class ModalidadDetailViewModel : BaseViewModel
    {
        /// <summary>
        /// Identificador de la modalidad (Clave primaria)
        /// </summary>
        [Key]
        public int Mda_Id { get; set; }

        /// <summary>
        /// Descripción de la modalidad
        /// </summary>
        [Display(Name = "Descripción")]
        public string Mda_Descripcion { get; set; }

        /// <summary>
        /// Nombre del usuario que registró la modalidad
        /// </summary>
        [Display(Name = "Creado por")]
        public string Mda_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Fecha en que se registró la modalidad
        /// </summary>
        [Display(Name = "Fecha de creación")]
        public DateTime? Mda_FechaRegistra { get; set; }

        /// <summary>
        /// Nombre del usuario que modificó la modalidad
        /// </summary>
        [Display(Name = "Modificado por")]
        public string Mda_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Fecha en que se modificó la modalidad
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? Mda_FechaModifica { get; set; }
    }
}

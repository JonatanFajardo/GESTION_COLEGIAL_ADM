

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Indica en que estado se encuentra el alumno, activo o inactivo.
    /// </summary>
    public partial class tbEstados
    { 

        /// <summary>
        /// Identificador único de un estado.
        /// </summary>
        public int Est_Id { get; set; }

        /// <summary>
        /// Información del estado actual del alumno en el instituto.
        /// </summary>
        public string Est_Descripcion { get; set; }

        public bool Est_EsEliminado { get; set; }
        public int Est_UsuarioRegistra { get; set; }
        public DateTime Est_FechaRegistra { get; set; }
        public int? Est_UsuarioModifica { get; set; }
        public DateTime? Est_FechaModifica { get; set; }

        public virtual tbUsuarios Est_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Est_UsuarioRegistraNavigation { get; set; } 
    }
}
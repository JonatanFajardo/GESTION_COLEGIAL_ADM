

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Materia que puede impartir un profesor o recibir el alumno.
    /// </summary>
    public partial class tbMaterias
    { 
        /// <summary>
        /// Identificador único de la materia.
        /// </summary>
        public int Mat_Id { get; set; }

        /// <summary>
        /// Nombre por el cual es distinguida la materia.
        /// </summary>
        public string Mat_Nombre { get; set; }

        /// <summary>
        /// Identificador de la duración.
        /// </summary>
        public int Dur_Id { get; set; }

        /// <summary>
        /// Valor que indica si este registro sera visible.
        /// </summary>
        public bool Mat_EsActivo { get; set; }

        public bool Mat_EsEliminado { get; set; }
        public int Mat_UsuarioRegistra { get; set; }
        public DateTime Mat_FechaRegistra { get; set; }
        public int? Mat_UsuarioModifica { get; set; }
        public DateTime? Mat_FechaModifica { get; set; }

        public virtual tbDuraciones Dur { get; set; }
        public virtual tbUsuarios Mat_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Mat_UsuarioRegistraNavigation { get; set; } 
    }
}
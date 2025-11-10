

using System;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Administra el nivel educativo que tendrá un curso.
    /// </summary>
    public partial class tbCursosNiveles
    { 
        /// <summary>
        /// Identificador único de la tabla tbCursosNiveles.
        /// </summary>
        public int Cun_Id { get; set; }

        public int? Cun_Orden { get; set; }

        /// <summary>
        /// Información del curso nivel.
        /// </summary>
        public string Cun_Descripcion { get; set; }

        /// <summary>
        /// Identificador del nivel del curso.
        /// </summary>
        public int Niv_Id { get; set; }

        public bool Cun_EsEliminado { get; set; }
        public int Cun_UsuarioRegistra { get; set; }
        public DateTime Cun_FechaRegistra { get; set; }
        public int? Cun_UsuarioModifica { get; set; }
        public DateTime? Cun_FechaModifica { get; set; }

        public virtual tbNivelesEducativos Niv { get; set; } 
    }
}
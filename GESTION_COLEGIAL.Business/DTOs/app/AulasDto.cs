using System;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra los nombres que contendrá un aula.
    /// </summary>
    public partial class tbAulas
    { 
        /// <summary>
        /// Identificador único de la tabla tbAulas.
        /// </summary>
        public int Aul_Id { get; set; }

        /// <summary>
        /// Información de la aula.
        /// </summary>
        public string Aul_Descripcion { get; set; }

        public bool Aul_EsEliminado { get; set; }
        public int Aul_UsuarioRegistra { get; set; }
        public DateTime Aul_FechaRegistra { get; set; }
        public int? Aul_UsuarioModifica { get; set; }
        public DateTime? Aul_FechaModifica { get; set; } 
    }
}
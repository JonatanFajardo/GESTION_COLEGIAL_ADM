

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Información del título.
    /// </summary>
    public partial class tbTitulos
    { 

        /// <summary>
        /// Identificador único del titulo.
        /// </summary>
        public int Tit_Id { get; set; }

        /// <summary>
        /// Nombre que tiene el titulo.
        /// </summary>
        public string Tit_Descripcion { get; set; }

        public bool Tit_EsEliminado { get; set; }
        public int Tit_UsuarioRegistra { get; set; }
        public DateTime Tit_FechaRegistra { get; set; }
        public int? Tit_UsuarioModifica { get; set; }
        public DateTime? Tit_FechaModifica { get; set; }

        public virtual tbUsuarios Tit_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Tit_UsuarioRegistraNavigation { get; set; } 
    }
}
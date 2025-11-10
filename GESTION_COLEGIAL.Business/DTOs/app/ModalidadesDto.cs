

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Especifica las diferentes modalidades en que pueden impartirse clases, estos son horas determinadas en la semana para que los alumnos asistan a clases.
    /// </summary>
    public partial class tbModalidades
    { 

        /// <summary>
        /// Identificador único de la modalidad.
        /// </summary>
        public int Mda_Id { get; set; }

        /// <summary>
        /// Descripción por el cual se conoce la modalidad.
        /// </summary>
        public string Mda_Descripcion { get; set; }

        public bool Mda_EsEliminado { get; set; }
        public int Mda_UsuarioRegistra { get; set; }
        public DateTime Mda_FechaRegistra { get; set; }
        public int? Mda_UsuarioModifica { get; set; }
        public DateTime? Mda_FechaModifica { get; set; } 
    }
}
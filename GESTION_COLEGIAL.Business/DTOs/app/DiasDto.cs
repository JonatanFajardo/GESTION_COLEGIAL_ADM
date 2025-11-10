

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra los días laborales.
    /// </summary>
    public partial class tbDias
    { 

        /// <summary>
        /// Identificador único de los dias.
        /// </summary>
        public int Dia_Id { get; set; }

        /// <summary>
        /// Nombre del dia.
        /// </summary>
        public string Dia_Descripcion { get; set; }

        public bool Dia_EsEliminado { get; set; }
        public int Dia_UsuarioRegistra { get; set; }
        public DateTime Dia_FechaRegistra { get; set; }
        public int? Dia_UsuarioModifica { get; set; }
        public DateTime? Dia_FechaModifica { get; set; }
         
    }
}
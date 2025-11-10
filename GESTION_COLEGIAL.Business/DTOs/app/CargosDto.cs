
using System;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra los puestos que puede tener un empleado en el colegio.
    /// </summary>
    public partial class tbCargos
    { 

        /// <summary>
        /// Identificador único del puesto.
        /// </summary>
        public int Car_Id { get; set; }

        /// <summary>
        /// Dato informativo del cargo.
        /// </summary>
        public string Car_Descripcion { get; set; }

        public bool Car_EsEliminado { get; set; }
        public int Car_UsuarioRegistra { get; set; }
        public DateTime Car_FechaRegistra { get; set; }
        public int? Car_UsuarioModifica { get; set; }
        public DateTime? Car_FechaModifica { get; set; }
         
    }
}
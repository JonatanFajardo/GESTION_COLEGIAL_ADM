

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Contiene los encargados relacionados con el alumno.
    /// </summary>
    public partial class tbEncargados_tbAlumnos
    {
        public int Enc_Id { get; set; }
        public int Alu_Id { get; set; }
        public int Par_Id { get; set; }

        public virtual tbAlumnos Alu { get; set; }
        public virtual tbEncargados Enc { get; set; }
        public virtual tbParentescos Par { get; set; }
    }
}
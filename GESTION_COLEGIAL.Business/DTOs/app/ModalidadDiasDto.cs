

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbModalidad_tbDias
    {
        public int Mdia_Id { get; set; }
        public int? Mda_Id { get; set; }
        public int? Dia_Id { get; set; }

        public virtual tbDias Dia { get; set; }
        public virtual tbModalidades Mda { get; set; }
    }
}
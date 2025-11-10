using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class SemestreFindDto
    {
        public int SemestreId { get; set; }
        public string DescripcionSemestre { get; set; }
        public bool EsActivoSemestre { get; set; }
    }
}
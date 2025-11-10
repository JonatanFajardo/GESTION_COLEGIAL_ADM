using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class NotaListDto
    {
        public int NotaId { get; set; }
        public int ValorNota { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionSemestre { get; set; }
        public string DescripcionParcial { get; set; }
        public DateTime AnioNota { get; set; }
        public string EsActivo { get; set; }
    }
}
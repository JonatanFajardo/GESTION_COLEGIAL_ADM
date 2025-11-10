using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class NotaFindDto
    {
        public int NotaId { get; set; }
        public int ValorNota { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionSemestre { get; set; }
        public string DescripcionParcial { get; set; }
        public DateTime AnioNota { get; set; }
        public bool EsActivoNota { get; set; }
    }
}
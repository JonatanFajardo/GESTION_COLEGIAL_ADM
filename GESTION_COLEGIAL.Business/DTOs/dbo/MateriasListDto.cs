using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class MateriaListDto
    {
        public int MateriaId { get; set; }
        public string NombreMateria { get; set; }
        public string DescripcionDuracion { get; set; }
        public string EsActivo { get; set; }
    }
}
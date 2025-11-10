using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class CursoListDto
    {
        public int CursoId { get; set; }
        public string NombreCurso { get; set; }
        public string DescripcionNivel { get; set; }
        public string EsActivo { get; set; }
    }
}
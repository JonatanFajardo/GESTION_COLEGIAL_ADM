using System;

namespace GESTION_COLEGIAL.Business.DTOs.dbo
{
    public class PromedioCursoUltimosAniosDto
    {
        public string Curso { get; set; }
        public int AnioCursado { get; set; }
        public decimal PromedioAnual { get; set; }
    }
}
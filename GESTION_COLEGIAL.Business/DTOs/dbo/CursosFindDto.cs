namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class CursoFindDto
    {
        public int CursoId { get; set; }
        public string NombreCurso { get; set; }
        public int NivelId { get; set; }
        public string DescripcionNivel { get; set; }
        public bool EsActivoCurso { get; set; }
    }
}
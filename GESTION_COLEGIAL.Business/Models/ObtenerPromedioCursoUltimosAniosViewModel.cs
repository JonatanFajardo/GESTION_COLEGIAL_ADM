namespace GESTION_COLEGIAL.Business.Models
{
    public class ObtenerPromedioCursoUltimosAniosViewModel : BaseViewModel
    {
        public string Curso { get; set; }
        public int AnioCursado { get; set; }
        public decimal PromedioAnual { get; set; }
    }
}

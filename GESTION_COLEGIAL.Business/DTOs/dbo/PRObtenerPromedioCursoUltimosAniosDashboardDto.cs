using System.ComponentModel.DataAnnotations.Schema;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class ObtenerPromedioCursoUltimosAniosDashboardDto
    {
        public int? AnioCursado { get; set; }

        [Column("PromedioAnual", TypeName = "decimal(38,6)")]
        public decimal? PromedioAnual { get; set; }
    }
}
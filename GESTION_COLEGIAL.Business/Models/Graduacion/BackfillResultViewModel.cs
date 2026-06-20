using System.Collections.Generic;

namespace GESTION_COLEGIAL.Business.Models.Graduacion
{
    public class BackfillResultViewModel
    {
        public int Total { get; set; }
        public int Exitosos { get; set; }
        public int Fallidos { get; set; }
        public bool DryRun { get; set; }
        public int? Anio { get; set; }
        public int TopN { get; set; }
        public List<BackfillDetalleViewModel> Detalle { get; set; } = new List<BackfillDetalleViewModel>();
    }

    public class BackfillDetalleViewModel
    {
        public int Alu_Id { get; set; }
        public string NombreCompleto { get; set; }
        public int? AnioGraduacion { get; set; }
        public string Resultado { get; set; }
        public string Mensaje { get; set; }
    }
}

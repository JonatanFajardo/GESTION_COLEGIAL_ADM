using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Datos de la pantalla Plantillas de evaluación (año elegido y opciones de los filtros).</summary>
    public class PlantillasEvaluacionIndexViewModel
    {
        public List<EvaluacionOpcionViewModel> Anios { get; set; } = new List<EvaluacionOpcionViewModel>();
        public int Anio { get; set; }
        public int AnioActual { get; set; }
        public List<EvaluacionOpcionViewModel> Semestres { get; set; } = new List<EvaluacionOpcionViewModel>();
        public List<EvaluacionOpcionViewModel> Parciales { get; set; } = new List<EvaluacionOpcionViewModel>();
    }
}

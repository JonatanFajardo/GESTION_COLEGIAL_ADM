using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Pantalla "Cierre de parciales": años para el selector, año elegido y si puede cerrar/publicar.</summary>
    public class CierreParcialesIndexViewModel
    {
        public List<CierreParcialAnioViewModel> Anios { get; set; } = new List<CierreParcialAnioViewModel>();
        public int Anio { get; set; }
        public int AnioActivo { get; set; }
        public bool PuedeCerrar { get; set; }
    }
}

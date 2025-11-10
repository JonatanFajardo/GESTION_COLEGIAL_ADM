using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class CardsInHomeDashboardDto
    {
        public int? Graduados { get; set; }
        public int? DiferenciaPromedioAnual { get; set; }
        public int? ActualPromedioAnual { get; set; }
        public int? DiferenciaNuevoIngreso { get; set; }
    }
}
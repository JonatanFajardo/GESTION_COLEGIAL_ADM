using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ResumenFinancieroViewModel
    {
        public decimal totalDeuda { get; set; }
        public decimal totalPagado { get; set; }
        public decimal totalPendiente { get; set; }
        public decimal totalMora { get; set; }
    }
}

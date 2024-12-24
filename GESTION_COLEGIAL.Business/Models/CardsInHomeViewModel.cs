namespace GESTION_COLEGIAL.Business.Models
{
    public class CardsInHomeViewModel : BaseViewModel
    {
        public int Graduados { get; set; }            // Número de graduados
        public int DiferenciaPromedioAnual { get; set; }  // Diferencia con el promedio anual
        public int ActualPromedioAnual { get; set; }   // Promedio anual actual
        public int DiferenciaNuevoIngreso { get; set; }  // Diferencia de nuevo ingreso

    }
}

using System.Collections.Generic;

namespace GESTION_COLEGIAL.Business.Models
{
    public class DashboardAulaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Nivel { get; set; }
        public string Modalidad { get; set; }
        public int Capacidad { get; set; }
        public int Alumnos { get; set; }
        public int Bajo { get; set; }
        public int Deuda { get; set; }
        public int Asistencia { get; set; }
        public bool BajoDisponible { get; set; }
        public bool AsistenciaDisponible { get; set; }
    }

    public class DashboardAulasResponseViewModel
    {
        public List<DashboardAulaViewModel> Aulas { get; set; } = new List<DashboardAulaViewModel>();
        public List<string> Niveles { get; set; } = new List<string>();
        public bool BajoDisponible { get; set; }
        public bool AsistenciaDisponible { get; set; }
    }
}

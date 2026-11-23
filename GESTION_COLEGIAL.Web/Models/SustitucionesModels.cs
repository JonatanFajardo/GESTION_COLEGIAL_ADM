namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Pantalla "Sustituciones de docentes": permisos de la sesión y fechas iniciales (yyyy-MM-dd).</summary>
    public class SustitucionesIndexViewModel
    {
        /// <summary>"Asignar sustituciones": marcar ausencias, asignar y cancelar suplentes.</summary>
        public bool PuedeAsignar { get; set; }
        /// <summary>"Ver todas las sustituciones" (o asignar): ve a todos; si no, solo lo propio.</summary>
        public bool VerTodas { get; set; }
        /// <summary>La sesión tiene un empleado ligado (sin él, quien solo ve lo propio no ve nada).</summary>
        public bool TieneEmpleado { get; set; }
        public string Hoy { get; set; }
        public string Fecha { get; set; }
        public string InicioMes { get; set; }
    }
}

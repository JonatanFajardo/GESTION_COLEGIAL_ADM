namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Alumno sin encargado o encargado sin hijos (pantalla "Vínculos familiares").</summary>
    public class VinculoPendienteViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Identidad { get; set; }
        public string Telefono { get; set; }
        /// <summary>Grado y sección (alumno) u ocupación (encargado).</summary>
        public string Detalle { get; set; }
    }
}

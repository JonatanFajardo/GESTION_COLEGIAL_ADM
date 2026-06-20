namespace GESTION_COLEGIAL.Business.Models.Graduacion
{
    /// <summary>
    /// Proyeccion del endpoint Graduacion/CandidatosAsync.
    /// </summary>
    public class CandidatoGraduacionViewModel
    {
        public int Alu_Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Niv_Descripcion { get; set; }
        public string Cur_Descripcion { get; set; }
        public string Sec_Descripcion { get; set; }
        public bool NotasOk { get; set; }
        public decimal DeudaPendiente { get; set; }
        public bool DeudaOk { get; set; }
        public string EstadoValidacion { get; set; }
        public string MotivosBloqueo { get; set; }
    }
}

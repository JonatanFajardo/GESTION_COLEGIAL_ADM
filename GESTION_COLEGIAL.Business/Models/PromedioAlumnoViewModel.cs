namespace GESTION_COLEGIAL.Business.Models
{
    public class PromedioAlumnoViewModel
    {
        public int Alu_Id { get; set; }
        public string Alu_NombreCompleto { get; set; }
        public decimal Promedio { get; set; }
        public bool EsBajo { get; set; }
    }
}

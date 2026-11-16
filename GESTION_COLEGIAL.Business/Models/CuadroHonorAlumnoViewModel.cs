namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Un alumno del cuadro de honor: su promedio del año, el concepto y su ubicación.
    /// NivelHonor: 'S' = Cuadro de Honor Superior, 'H' = Cuadro de Honor.
    /// </summary>
    public class CuadroHonorAlumnoViewModel
    {
        public int Alu_Id { get; set; }
        public string Alu_NombreCompleto { get; set; }
        public decimal Promedio { get; set; }
        public string Concepto { get; set; }
        public string Nivel { get; set; }
        public string Curso { get; set; }
        public string Seccion { get; set; }
        public string NivelHonor { get; set; }
    }
}

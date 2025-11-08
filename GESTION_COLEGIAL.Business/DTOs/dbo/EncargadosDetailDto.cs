namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class EncargadoDetailDto
    {
        public int? EncargadoId { get; set; }
        public string NumeroIdentidad { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Sexo { get; set; }
        public string EsActivoPersona { get; set; }
        public string NombreUsuarioRegistraEncargado { get; set; }
        public DateTime? FechaRegistroPersona { get; set; }
        public string NombreUsuarioModificaEncargado { get; set; }
        public DateTime? FechaModificacionPersona { get; set; }
        public string OcupacionEncargado { get; set; }
    }
}
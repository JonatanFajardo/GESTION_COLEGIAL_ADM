using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class EncargadoFindDto
    {
        public int EncargadoId { get; set; }
        public int PersonaId { get; set; }
        public string NumeroIdentidad { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string CorreoElectronico { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Sexo { get; set; }
        public bool EsActivoPersona { get; set; }
        public string OcupacionEncargado { get; set; }
    }
}
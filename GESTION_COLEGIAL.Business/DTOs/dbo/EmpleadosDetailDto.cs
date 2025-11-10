using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class EmpleadoDetailDto
    {
        public int? EmpleadoId { get; set; }
        public string CodigoEmpleado { get; set; }
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
        public string EsActivo { get; set; }
        public string NombreUsuarioRegistraEmpleado { get; set; }
        public DateTime? FechaRegistroPersona { get; set; }
        public string NombreUsuarioModificaEmpleado { get; set; }
        public DateTime? FechaModificacionPersona { get; set; }
        public string DescripcionTitulo { get; set; }
        public string DescripcionCargo { get; set; }
    }
}
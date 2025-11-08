using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class AlumnoDetailDto
    {
        public int? AlumnoId { get; set; }
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
        public string NombreCurso { get; set; }
        public string DescripcionEstado { get; set; }
        public string DescripcionNivel { get; set; }
        public string DescripcionModalidad { get; set; }
        public string DescripcionCursoNivel { get; set; }
        public string DescripcionSeccion { get; set; }
        public string EsActivoPersona { get; set; }
        public string NombreUsuarioRegistraAlumno { get; set; }
        public DateTime? FechaRegistroPersona { get; set; }
        public string NombreUsuarioModificaAlumno { get; set; }
        public DateTime? FechaModificacionPersona { get; set; }
    }
}
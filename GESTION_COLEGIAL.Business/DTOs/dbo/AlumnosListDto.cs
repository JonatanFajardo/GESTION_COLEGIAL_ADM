using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class AlumnoListDto
    {
        public int AlumnoId { get; set; }
        public string ImagenPersona { get; set; }
        public string NumeroIdentidad { get; set; }
        public string NombreAlumno { get; set; }
        public string NombreCurso { get; set; }
        public string DescripcionModalidad { get; set; }
        public string DescripcionSeccion { get; set; }
        public string DescripcionNivel { get; set; }
        public int? AnioCursado { get; set; }
        public string DescripcionEstado { get; set; }
    }
}
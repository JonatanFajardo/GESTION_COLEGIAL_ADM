using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class ObtenerCantidadAlumnosDto
    {
        public string NivelEducativo { get; set; }
        public string Curso { get; set; }
        public string Modalida { get; set; }
        public string secciones { get; set; }
        public int? CantidadAlumnos { get; set; }
    }
}
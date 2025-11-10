

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbPersonasHistorial
    {
        public int Per_Id { get; set; }
        public string Per_Identidad { get; set; }
        public string Per_PrimerNombre { get; set; }
        public string Per_SegundoNombre { get; set; }
        public string Per_ApellidoPaterno { get; set; }
        public string Per_ApellidoMaterno { get; set; }
        public DateTime? Per_FechaNacimiento { get; set; }
        public string Per_CorreoElectronico { get; set; }
        public string Per_Telefono { get; set; }
        public string Per_Direccion { get; set; }
        public string Per_Sexo { get; set; }
        public bool? EsActivo { get; set; }
        public bool? EsEliminado { get; set; }
        public string Accion { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; }
        public string HostName { get; set; }
    }
}
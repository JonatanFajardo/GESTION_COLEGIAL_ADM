

using System;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbUsuarios
    { 
        public int Usu_Id { get; set; }
        public int Emp_Id { get; set; }
        public string Usu_Name { get; set; }
        public string Usu_Contraseña { get; set; }
        public int Rol_Id { get; set; }
        public string Usu_Ip { get; set; }
        public bool Usu_EsActivo { get; set; }
        public bool? Usu_Suspendido { get; set; }
        public bool? Usu_EsEliminado { get; set; }
        public DateTime? Usu_FechaCreacion { get; set; }
        public DateTime? Usu_fechaModificacion { get; set; } 
    }
}
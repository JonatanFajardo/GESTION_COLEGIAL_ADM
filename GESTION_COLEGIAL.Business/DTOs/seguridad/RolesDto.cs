

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbRoles
    {
        public int Rol_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public bool Rol_Estado { get; set; }
        public bool Rol_EsEliminado { get; set; }
        public int Rol_UsuarioRegistra { get; set; }
        public DateTime Rol_FechaRegistra { get; set; }
        public int? Rol_UsuarioModifica { get; set; }
        public DateTime? Rol_FechaModifica { get; set; }
    }
}
using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class RolDetailViewModel : BaseViewModel
    {
        public int Rol_Id { get; set; }
        public string Rol_Descripcion { get; set; }
        public bool Rol_Estado { get; set; }
        public DateTime? Rol_FechaRegistra { get; set; }
        public DateTime? Rol_FechaModifica { get; set; }
        public int CantidadPantallas { get; set; }
    }
}

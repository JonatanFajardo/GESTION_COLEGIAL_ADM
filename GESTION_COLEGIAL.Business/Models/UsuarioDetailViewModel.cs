using System;

namespace GESTION_COLEGIAL.Business.Models
{
    public class UsuarioDetailViewModel : BaseViewModel
    {
        public int Usu_Id { get; set; }
        public string Usu_Name { get; set; }
        public string Rol_Descripcion { get; set; }
        public bool Usu_EsActivo { get; set; }
        public DateTime? Usu_FechaCreacion { get; set; }
        public DateTime? Usu_fechaModificacion { get; set; }
    }
}

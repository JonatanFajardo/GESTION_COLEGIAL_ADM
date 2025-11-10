

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbMateriasHistorial
    {
        public int Mat_Id { get; set; }
        public string Mat_Nombre { get; set; }
        public int? Dur_Id { get; set; }
        public int? Cur_Id { get; set; }
        public bool? EsActivo { get; set; }
        public bool? EsEliminado { get; set; }
        public string Accion { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; }
        public string HostName { get; set; }
    }
}
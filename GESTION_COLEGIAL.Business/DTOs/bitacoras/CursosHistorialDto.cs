#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbCursosHistorial
    {
        public int Cur_Id { get; set; }
        public int? Cno_Id { get; set; }
        public int? Aul_Id { get; set; }
        public int? Sec_Id { get; set; }
        public int? Niv_Id { get; set; }
        public int? mda_Id { get; set; }
        public bool? EsActivo { get; set; }
        public bool? EsEliminado { get; set; }
        public string Accion { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; }
        public string HostName { get; set; }
    }
}
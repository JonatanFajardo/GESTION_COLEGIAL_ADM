#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbEncargadosHistorial
    {
        public int Enc_Id { get; set; }
        public int? Per_Id { get; set; }
        public bool? EsActivo { get; set; }
        public bool? EsEliminado { get; set; }
        public string Accion { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; }
        public string HostName { get; set; }
    }
}
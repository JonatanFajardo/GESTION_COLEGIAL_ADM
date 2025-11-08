namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class HoraDetailDto
    {
        public int? HorarioId { get; set; }
        public string Hora { get; set; }
        public string NombreUsuarioRegistraHorario { get; set; }
        public DateTime? FechaRegistroHorario { get; set; }
        public string NombreUsuarioModificaHorario { get; set; }
        public DateTime? FechaModificacionHorario { get; set; }
    }
}
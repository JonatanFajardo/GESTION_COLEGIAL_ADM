namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Un horario que el usuario puede ver en el pase de lista (con sus datos para filtrar en cascada).</summary>
    public class PaseListaHorarioViewModel
    {
        public int Hor_Id { get; set; }
        public int Dia_Id { get; set; }
        public string Dia { get; set; }
        public string Hora { get; set; }
        public string Materia { get; set; }
        public string Seccion { get; set; }
        public string Grado { get; set; }
        public int? Cun_Id { get; set; }
        public int? Sec_Id { get; set; }
        public int? Niv_Id { get; set; }
        public string Nivel { get; set; }
        public bool EsSustitucion { get; set; }
        public string Titular { get; set; }
    }

    public class PaseListaPuedeViewModel
    {
        public bool Puede { get; set; }
    }
}

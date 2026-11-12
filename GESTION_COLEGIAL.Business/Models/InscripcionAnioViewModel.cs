namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Un grupo de ubicación de los alumnos del año (nivel + grado + modalidad + sección)
    /// frente a su sección del año y cuántos están inscritos.
    /// </summary>
    public class ResumenInscripcionViewModel
    {
        public int? Ani_Id { get; set; }
        public int Niv_Id { get; set; }
        public int Cun_Id { get; set; }
        public int? Mda_Id { get; set; }
        public int Sec_Id { get; set; }
        public string Niv_Descripcion { get; set; }
        public string Cun_Descripcion { get; set; }
        public string Mda_Descripcion { get; set; }
        public string Sec_Descripcion { get; set; }
        public int Alumnos { get; set; }
        public int Inscritos { get; set; }
        public int Pendientes { get; set; }
        public int? Sea_Id { get; set; }
        public string Sea_Nombre { get; set; }
    }

    /// <summary>
    /// Resultado de crear secciones del año y/o inscribir alumnos en el año.
    /// </summary>
    public class ResultadoInscripcionViewModel
    {
        public int SeccionesCreadas { get; set; }
        public int Inscritos { get; set; }
        public int Reactivados { get; set; }
        public int YaInscritos { get; set; }
        public int SinSeccion { get; set; }
        public int Activados { get; set; }
    }
}

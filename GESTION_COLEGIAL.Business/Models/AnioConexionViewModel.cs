using System;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Una sección del año con su cupo y los alumnos inscritos.</summary>
    public class CupoSeccionViewModel
    {
        public int Sea_Id { get; set; }
        public int Ani_Id { get; set; }
        public string Sea_Nombre { get; set; }
        public string Niv_Descripcion { get; set; }
        public string Cun_Descripcion { get; set; }
        public string Sec_Descripcion { get; set; }
        public string Mda_Descripcion { get; set; }
        public int? Sea_Cupo { get; set; }
        public int Inscritos { get; set; }
        public int? Libres { get; set; }
    }

    /// <summary>Año donde se puede inscribir (el activo y los siguientes con secciones).</summary>
    public class AnioInscripcionViewModel
    {
        public int Ani_Id { get; set; }
        public int Ani_Anio { get; set; }
        public string Ani_Nombre { get; set; }
        public int Ani_Estado { get; set; }
        public string Ani_EstadoTexto { get; set; }
        public bool EsActivo { get; set; }
        public int Secciones { get; set; }
        public int SeccionesConCupo { get; set; }
        public int? Cupo { get; set; }
        public int Inscritos { get; set; }
    }

    /// <summary>¿Hay lugar en la sección que corresponde a esa ubicación, en ese año?</summary>
    public class VerificarCupoViewModel
    {
        public int? Ani_Id { get; set; }
        public string Ani_Nombre { get; set; }
        public bool EsActivo { get; set; }
        public int? Sea_Id { get; set; }
        public string Sea_Nombre { get; set; }
        public int? Cupo { get; set; }
        public int Inscritos { get; set; }
        public int? Libres { get; set; }
        public bool Permitido { get; set; }
        public string Mensaje { get; set; }
    }

    /// <summary>Respuesta de las escrituras: { Ok, Mensaje, ... }.</summary>
    public class AnioConexionResultadoViewModel
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public int Sea_Id { get; set; }
        public int Sea_Cupo { get; set; }
        public int Inscritos { get; set; }
        public int Libres { get; set; }
        public int Mta_Id { get; set; }
        public string Sea_Nombre { get; set; }
        public string Ani_Nombre { get; set; }
        public int? Cupo { get; set; }
        public int Aplicados { get; set; }
        public int CursoPorRevisar { get; set; }
    }

    /// <summary>Semestre (1 o 2) según los períodos del año.</summary>
    public class SemestreAnioViewModel
    {
        public int Sem_Id { get; set; }
        public string Nombre { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Fin { get; set; }
    }
}

using System;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Plantilla de evaluación de un año con sus tipos (TiposJson = [{Plt_Id, Nombre, Peso, Combinacion, Orden, Actividades}]).</summary>
    public class PlantillaEvaluacionViewModel
    {
        public int Ple_Id { get; set; }
        public int Anio { get; set; }
        public int? Niv_Id { get; set; }
        public string Nivel { get; set; }
        public int? Mat_Id { get; set; }
        public string Materia { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Version { get; set; }
        public string Alcance { get; set; }
        public bool PorDefecto { get; set; }
        public int Cuadernos { get; set; }
        public int? CuadernosCerrados { get; set; }
        public int? Actividades { get; set; }
        public string TiposJson { get; set; }
        public string ModificadoPor { get; set; }
        public DateTime? Modificado { get; set; }
    }

    public class EvaluacionOpcionViewModel
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public bool EsActual { get; set; }
    }

    /// <summary>Uso de actividades en una materia y sección para un parcial (Dirección).</summary>
    public class EvaluacionAvanceViewModel
    {
        public int Hor_Id { get; set; }
        public int Pac_Id { get; set; }
        public string Docente { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public int GradoOrden { get; set; }
        public string Seccion { get; set; }
        public string Curso { get; set; }
        public string Materia { get; set; }
        public int? Cev_Id { get; set; }
        public string Plantilla { get; set; }
        public int Alumnos { get; set; }
        public int Actividades { get; set; }
        public int ActividadesQueCuentan { get; set; }
        public int Capturadas { get; set; }
        public int Esperadas { get; set; }
        public decimal? Porcentaje { get; set; }
        public int ConCalculo { get; set; }
        public int NotasAlDia { get; set; }
        public DateTime? UltimoCambio { get; set; }
        public string Estado { get; set; }
    }

    /// <summary>Quién escribe: lo pone el controller con la sesión (la API lo vuelve a poner con el usuario firmado).</summary>
    public abstract class EvaluacionEscritorViewModel
    {
        public int? Usuario { get; set; }
    }

    public class PlantillaEvaluacionGuardarViewModel : EvaluacionEscritorViewModel
    {
        public int Ple_Id { get; set; }
        public int Anio { get; set; }
        public int? Niv_Id { get; set; }
        public int? Mat_Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        /// <summary>JSON [{id, n, p, c}].</summary>
        public string Tipos { get; set; }
        public int Version { get; set; }
    }

    public class PlantillaEvaluacionQuitarViewModel : EvaluacionEscritorViewModel
    {
        public int Ple_Id { get; set; }
        public int Version { get; set; }
    }

    public class ActividadEvaluacionGuardarViewModel : EvaluacionEscritorViewModel
    {
        public int Act_Id { get; set; }
        public int Hor_Id { get; set; }
        public int Pac_Id { get; set; }
        public int? Cet_Id { get; set; }
        public int? Plt_Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PuntosMax { get; set; }
        public DateTime? Fecha { get; set; }
        public bool NoCuenta { get; set; }
        public int Version { get; set; }
        /// <summary>JSON con más Hor_Id donde crear la misma actividad.</summary>
        public string Otras { get; set; }
    }

    public class ActividadEvaluacionQuitarViewModel : EvaluacionEscritorViewModel
    {
        public int Act_Id { get; set; }
        public int Version { get; set; }
    }

    public class PuntosEvaluacionGuardarViewModel : EvaluacionEscritorViewModel
    {
        public int Cev_Id { get; set; }
        /// <summary>JSON [{a, u, e, p, v}].</summary>
        public string Puntos { get; set; }
    }

    /// <summary>Respuesta de las escrituras de la API Evaluaciones.</summary>
    public class EvaluacionRespuestaViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public int? Cev_Id { get; set; }
        public int? Version { get; set; }
        public int Cantidad { get; set; }
        public int Omitidas { get; set; }
        public int Sincronizados { get; set; }
        public int Conservados { get; set; }
        public int NotasEscritas { get; set; }
        public int NotasFallidas { get; set; }
        public string AvisoNotas { get; set; }
        public string Mensaje { get; set; }
    }
}

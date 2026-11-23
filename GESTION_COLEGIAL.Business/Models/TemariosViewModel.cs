using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.Business.Models
{
    // Temario y avance del programa (API Temarios, script 074).

    /// <summary>Un temario de la biblioteca o su ficha.</summary>
    public class TemarioViewModel
    {
        public int Tem_Id { get; set; }
        public int Mat_Id { get; set; }
        public string Materia { get; set; }
        public int? Niv_Id { get; set; }
        public string Nivel { get; set; }
        public int Cun_Id { get; set; }
        public string Grado { get; set; }
        public int Cun_Orden { get; set; }
        public int? Cur_Id { get; set; }
        public string Curso { get; set; }
        public int Sem_Id { get; set; }
        public string Semestre { get; set; }
        public int Tem_Anio { get; set; }
        public string Tem_Nombre { get; set; }
        public string Tem_Descripcion { get; set; }
        public DateTime Tem_FechaInicio { get; set; }
        public int? SemanaActual { get; set; }
        public int Temas { get; set; }
        public int Semanas { get; set; }
        public int Secciones { get; set; }
        public int AvancePct { get; set; }
        public int SeccionesAtrasadas { get; set; }
        public bool EsMio { get; set; }
        public bool PuedeEditar { get; set; }
        public bool PuedeSupervisar { get; set; }
        public int? Tem_IdOrigen { get; set; }
        public string Origen { get; set; }
        public int? OrigenAnio { get; set; }
        public string Registra { get; set; }
        public DateTime Tem_FechaRegistra { get; set; }
        public DateTime? Tem_FechaModifica { get; set; }

        /// <summary>"Séptimo · Tercer Ciclo" (con curso) o solo el grado.</summary>
        public string Ubicacion => string.IsNullOrWhiteSpace(Curso) ? Grado : Grado + " · " + Curso;
        public string Periodo => Semestre + " " + Tem_Anio;
    }

    public class TemarioTemaViewModel
    {
        public int Tet_Id { get; set; }
        public int Tem_Id { get; set; }
        public int Tet_Semana { get; set; }
        public int Tet_Orden { get; set; }
        public string Tet_Titulo { get; set; }
        public string Tet_Objetivos { get; set; }
        public string Tet_Contenido { get; set; }
        public string Tet_Recursos { get; set; }
        public bool Esperado { get; set; }
        public int SeccionesDado { get; set; }
        public int Marcas { get; set; }
    }

    /// <summary>Una sección (curso + sección + jornada) que lleva la materia, con su avance.</summary>
    public class TemarioGrupoViewModel
    {
        public int Cur_Id { get; set; }
        public string Curso { get; set; }
        public int Sec_Id { get; set; }
        public string Seccion { get; set; }
        public int Mda_Id { get; set; }
        public string Jornada { get; set; }
        public string Docentes { get; set; }
        public int CantidadDocentes { get; set; }
        public bool PuedeMarcar { get; set; }
        public int Total { get; set; }
        public int Dados { get; set; }
        public int Parciales { get; set; }
        public int Omitidos { get; set; }
        public int Esperados { get; set; }
        public int Atrasados { get; set; }
        public int? SemanaActual { get; set; }
        public int Pct { get; set; }
        public string Semaforo { get; set; }
        public string Clave => Cur_Id + "-" + Sec_Id + "-" + Mda_Id;
    }

    public class TemarioAvanceViewModel
    {
        public int Tav_Id { get; set; }
        public int Tet_Id { get; set; }
        public int Cur_Id { get; set; }
        public int Sec_Id { get; set; }
        public int Mda_Id { get; set; }
        public string Tav_Estado { get; set; }
        public DateTime? Tav_Fecha { get; set; }
        public string Tav_Nota { get; set; }
        public string Usuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class TemarioResumenViewModel
    {
        public int Tem_Id { get; set; }
        public string Tem_Nombre { get; set; }
        public int Mat_Id { get; set; }
        public string Materia { get; set; }
        public int? Niv_Id { get; set; }
        public string Nivel { get; set; }
        public int Cun_Id { get; set; }
        public string Grado { get; set; }
        public int Cun_Orden { get; set; }
        public int Cur_Id { get; set; }
        public string Curso { get; set; }
        public int Sec_Id { get; set; }
        public string Seccion { get; set; }
        public int Mda_Id { get; set; }
        public string Jornada { get; set; }
        public int Sem_Id { get; set; }
        public int Tem_Anio { get; set; }
        public int Total { get; set; }
        public int Dados { get; set; }
        public int Parciales { get; set; }
        public int Omitidos { get; set; }
        public int Esperados { get; set; }
        public int Atrasados { get; set; }
        public int? SemanaActual { get; set; }
        public int Pct { get; set; }
        public string Semaforo { get; set; }
        public DateTime? UltimaFecha { get; set; }
        public string Docentes { get; set; }
    }

    public class TemarioSinTemarioViewModel
    {
        public int Sem_Id { get; set; }
        public string Semestre { get; set; }
        public int Mat_Id { get; set; }
        public string Materia { get; set; }
        public int? Niv_Id { get; set; }
        public string Nivel { get; set; }
        public int Cun_Id { get; set; }
        public string Grado { get; set; }
        public int Cun_Orden { get; set; }
        public int Secciones { get; set; }
    }

    public class TemarioOpcionViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class TemarioTemaHorarioViewModel
    {
        public int Tet_Id { get; set; }
        public int Tem_Id { get; set; }
        public string Tem_Nombre { get; set; }
        public int Tet_Semana { get; set; }
        public int Tet_Orden { get; set; }
        public string Tet_Titulo { get; set; }
        public string Tet_Objetivos { get; set; }
        public string Tet_Contenido { get; set; }
        public string Tet_Recursos { get; set; }
    }

    // ───────────── Escrituras (el ADM pone Usuario con la sesión; lo que mande el navegador se descarta) ─────────────

    public abstract class TemarioEscritorViewModel
    {
        public int? Usuario { get; set; }
    }

    public class TemarioGuardarViewModel : TemarioEscritorViewModel
    {
        public int Tem_Id { get; set; }
        public int Mat_Id { get; set; }
        public int Cun_Id { get; set; }
        public int? Cur_Id { get; set; }
        public int Sem_Id { get; set; }
        public int Tem_Anio { get; set; }
        public string Tem_Nombre { get; set; }
        public string Tem_Descripcion { get; set; }
        public DateTime? Tem_FechaInicio { get; set; }
    }

    public class TemarioCopiarViewModel : TemarioEscritorViewModel
    {
        public int Tem_IdOrigen { get; set; }
        public int? Tem_IdDestino { get; set; }
        public int? Sem_Id { get; set; }
        public int? Tem_Anio { get; set; }
        public string Tem_Nombre { get; set; }
        public DateTime? Tem_FechaInicio { get; set; }
    }

    public class TemarioEliminarViewModel : TemarioEscritorViewModel
    {
        public int Tem_Id { get; set; }
    }

    public class TemarioTemaGuardarViewModel : TemarioEscritorViewModel
    {
        public int Tet_Id { get; set; }
        public int Tem_Id { get; set; }
        public int Tet_Semana { get; set; }
        public string Tet_Titulo { get; set; }
        public string Tet_Objetivos { get; set; }
        public string Tet_Contenido { get; set; }
        public string Tet_Recursos { get; set; }
    }

    public class TemarioTemaAccionViewModel : TemarioEscritorViewModel
    {
        public int Tet_Id { get; set; }
        public string Direccion { get; set; }
    }

    public class TemarioAvanceGuardarViewModel : TemarioEscritorViewModel
    {
        public int Tet_Id { get; set; }
        public int Cur_Id { get; set; }
        public int Sec_Id { get; set; }
        public int Mda_Id { get; set; }
        public string Tav_Estado { get; set; }
        public DateTime? Tav_Fecha { get; set; }
        public string Tav_Nota { get; set; }
    }

    /// <summary>Respuesta de las escrituras de la API.</summary>
    public class TemarioResultadoViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public int Tem_Id { get; set; }
        public int Cantidad { get; set; }
        public string Mensaje { get; set; }
    }

    /// <summary>Textos y reglas compartidas por las pantallas del temario.</summary>
    public static class CatalogoTemario
    {
        public static readonly string[] EstadosAvance = { "dado", "parcial", "omitido" };

        public static string EstadoAvance(string estado) => estado switch
        {
            "dado" => "Dado",
            "parcial" => "Dado en parte",
            "omitido" => "Omitido",
            _ => "Sin marcar"
        };

        public static string Semaforo(string semaforo) => semaforo switch
        {
            "al_dia" => "Al día",
            "leve" => "Leve atraso",
            "atrasada" => "Atrasada",
            "sin_inicio" => "Sin iniciar",
            "sin_temas" => "Sin temas",
            _ => "—"
        };

        /// <summary>Lunes de la semana n contando desde el inicio de clases.</summary>
        public static DateTime InicioSemana(DateTime inicio, int semana) => inicio.Date.AddDays((semana - 1) * 7);

        /// <summary>Inicio de clases sugerido: primer lunes de febrero (I semestre) o de julio (II semestre).</summary>
        public static DateTime InicioSugerido(int anio, int semId)
        {
            var d = new DateTime(anio, semId == 2 ? 7 : 2, 1);
            while (d.DayOfWeek != DayOfWeek.Monday) d = d.AddDays(1);
            return d;
        }
    }
}

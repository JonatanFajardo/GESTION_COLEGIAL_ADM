using System;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Una clase de un docente ausente en una fecha (API Sustituciones/DiaAsync). Un ausente sin clases
    /// ese día viene una vez con Hor_Id null. Origen: vacaciones | permiso | incapacidad | asistencia | manual
    /// (con manual, OrigenId = la ausencia marcada a mano). Minutos del día: 420 = 7:00, 780 = 13:00.
    /// </summary>
    public class SustitucionDiaViewModel
    {
        public int Emp_IdAusente { get; set; }
        public string Ausente { get; set; }
        public string Foto { get; set; }
        public string Origen { get; set; }
        public int? OrigenId { get; set; }
        public string Detalle { get; set; }
        public int? Hor_Id { get; set; }
        public int? MinInicio { get; set; }
        public int? MinFin { get; set; }
        public string Materia { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public string Modalidad { get; set; }
        public string Curso { get; set; }
        public string Aula { get; set; }
        public string Cierre { get; set; }
        public int? Sus_Id { get; set; }
        public int? Emp_IdSuplente { get; set; }
        public string Suplente { get; set; }
        public string Sus_Notas { get; set; }
        public bool PuedeCancelar { get; set; }
    }

    /// <summary>Indicadores del mes de una fecha (con alcance de docente: lo que cubrió él).</summary>
    public class SustitucionMesViewModel
    {
        public int ClasesMes { get; set; }
        public int MinutosMes { get; set; }
        public int SuplentesMes { get; set; }
        public int ClasesDia { get; set; }
    }

    /// <summary>Un suplente posible para una clase, con las razones del orden.</summary>
    public class SustitucionSugerenciaViewModel
    {
        public int Emp_Id { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
        public bool SabeMateria { get; set; }
        public bool MismoGrado { get; set; }
        public bool MismoGrupo { get; set; }
        public int SustitucionesMes { get; set; }
        public int ClasesDia { get; set; }
        public int Orden { get; set; }
    }

    /// <summary>Un docente para "Marcar docente ausente".</summary>
    public class SustitucionDocenteViewModel
    {
        public int Emp_Id { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
        public int ClasesDia { get; set; }
        public string AusenteOrigen { get; set; }
    }

    /// <summary>Una sustitución del historial.</summary>
    public class SustitucionHistorialViewModel
    {
        public int Sus_Id { get; set; }
        public DateTime Sus_Fecha { get; set; }
        public int Hor_Id { get; set; }
        public int? MinInicio { get; set; }
        public int? MinFin { get; set; }
        public int Sus_Minutos { get; set; }
        public string Materia { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public string Modalidad { get; set; }
        public string Curso { get; set; }
        public string Aula { get; set; }
        public int Emp_IdAusente { get; set; }
        public string Ausente { get; set; }
        public int Emp_IdSuplente { get; set; }
        public string Suplente { get; set; }
        public string Sus_Origen { get; set; }
        public int? Sus_OrigenId { get; set; }
        public string OrigenDetalle { get; set; }
        public string Sus_Estado { get; set; }
        public string Sus_Notas { get; set; }
        public string Sus_MotivoCancelacion { get; set; }
        public string RegistradoPor { get; set; }
        public DateTime Sus_FechaRegistra { get; set; }
        public string CanceladoPor { get; set; }
        public DateTime? Sus_FechaCancela { get; set; }
    }

    /// <summary>Quién escribe: lo pone el controller con la sesión (lo que mande el navegador se descarta).</summary>
    public abstract class SustitucionEscritorViewModel
    {
        public int? Usuario { get; set; }
        public bool PuedeAsignar { get; set; }
    }

    public class SustitucionAsignarViewModel : SustitucionEscritorViewModel
    {
        public int Hor_Id { get; set; }
        public DateTime? Sus_Fecha { get; set; }
        public int Emp_IdSuplente { get; set; }
        public string Sus_Notas { get; set; }
    }

    public class SustitucionCancelarViewModel : SustitucionEscritorViewModel
    {
        public int Sus_Id { get; set; }
        public string Motivo { get; set; }
    }

    public class AusenciaDocenteMarcarViewModel : SustitucionEscritorViewModel
    {
        public int Emp_Id { get; set; }
        public DateTime? Aud_Fecha { get; set; }
        public string Aud_Motivo { get; set; }
    }

    public class AusenciaDocenteQuitarViewModel : SustitucionEscritorViewModel
    {
        public int Aud_Id { get; set; }
    }

    /// <summary>Respuesta de las escrituras de la API: { Ok, Id, Estado, Mensaje }.</summary>
    public class SustitucionResultadoViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
    }
}

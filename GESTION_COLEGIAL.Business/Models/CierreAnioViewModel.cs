using System;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Encabezado del asistente de cierre del año (API CierreAnio/ResumenAsync).</summary>
    public class CierreAnioResumenViewModel
    {
        public int Ani_Id { get; set; }
        public int Ani_Anio { get; set; }
        public string Ani_Nombre { get; set; }
        /// <summary>1 Borrador · 2 Programado · 3 Abierto · 4 Cerrando · 5 Cerrado · 6 Archivado</summary>
        public int Ani_Estado { get; set; }
        public string EstadoTexto { get; set; }
        public bool EsActivo { get; set; }
        public int Inscritos { get; set; }
        public int Secciones { get; set; }
        public int ParcialesPublicados { get; set; }
        public int ParcialesTotal { get; set; }
        public int Resultados { get; set; }
        public DateTime? FechaCalculo { get; set; }
        public int Confirmados { get; set; }
        public int? SiguienteId { get; set; }
        public int? SiguienteAnio { get; set; }
        public string SiguienteNombre { get; set; }
        public int? SiguienteEstado { get; set; }
        public string SiguienteEstadoTexto { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string UsuarioCierre { get; set; }
    }

    public class CierreAnioRevisionViewModel
    {
        public int Orden { get; set; }
        public string Grupo { get; set; }
        public string Clave { get; set; }
        public string Titulo { get; set; }
        public string Detalle { get; set; }
        public int Cantidad { get; set; }
        /// <summary>ok | aviso | bloquea</summary>
        public string Nivel { get; set; }
        public string Enlace { get; set; }
    }

    public class CierreAnioReglaViewModel
    {
        public int Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }
        public decimal NotaMinima { get; set; }
        public int MaxRecuperacion { get; set; }
        public bool EsValorInicial { get; set; }
        /// <summary>guardada | anio_anterior | escala | honduras</summary>
        public string Origen { get; set; }
    }

    public class CierreAnioSeccionViewModel
    {
        public int Sea_Id { get; set; }
        public string Sea_Nombre { get; set; }
        public int? Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }
        public int? Cun_Id { get; set; }
        public string Cun_Descripcion { get; set; }
        public int Cun_Orden { get; set; }
        public int? Sec_Id { get; set; }
        public string Sec_Descripcion { get; set; }
        public int Alumnos { get; set; }
        public int ConDecision { get; set; }
        public int Confirmados { get; set; }
        public int Promovidos { get; set; }
        public int Repiten { get; set; }
        public int Graduan { get; set; }
        public int Pendientes { get; set; }
        public int EnRecuperacion { get; set; }
        public int Cambiadas { get; set; }
    }

    public class CierreAnioResultadoViewModel
    {
        public int Rea_Id { get; set; }
        public int Alu_Id { get; set; }
        public string Alumno { get; set; }
        public string Identidad { get; set; }
        public int Mat_Id { get; set; }
        public string Materia { get; set; }
        public int PeriodosConNota { get; set; }
        public int PeriodosEsperados { get; set; }
        public decimal? Promedio { get; set; }
        public decimal? NotaRecuperacion { get; set; }
        public decimal? NotaFinal { get; set; }
        /// <summary>aprobado | recuperacion | reprobado | sin_notas</summary>
        public string Resultado { get; set; }
        public bool? AproboRecuperacion { get; set; }
        public string Concepto { get; set; }
        public decimal? NotaMinima { get; set; }
        public bool EnRecuperacion { get; set; }
        public bool Confirmada { get; set; }
    }

    public class CierreAnioDecisionViewModel
    {
        public int Pra_Id { get; set; }
        public int Alu_Id { get; set; }
        public string Alumno { get; set; }
        public string Identidad { get; set; }
        public int Materias { get; set; }
        public int Aprobadas { get; set; }
        public int EnRecuperacion { get; set; }
        public int Reprobadas { get; set; }
        public int SinNotas { get; set; }
        public decimal? Promedio { get; set; }
        /// <summary>promovido | repite | gradua | pendiente</summary>
        public string Propuesta { get; set; }
        public string Decision { get; set; }
        public string Efectiva { get; set; }
        public string Motivo { get; set; }
        public bool Confirmada { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public string UsuarioConfirma { get; set; }
        public bool EsUltimoGrado { get; set; }
        /// <summary>Aprobó el último grado de su nivel sin ser el último de su carrera (Noveno: termina la educación básica).</summary>
        public bool TerminaNivel { get; set; }
        /// <summary>Qué hizo la confirmación con una inscripción del año siguiente que ya existía: "movio" o "quito".</summary>
        public string AjusteDestino { get; set; }
        public bool HaySiguienteGrado { get; set; }
        public string Destino { get; set; }
        public int? AnioDestino { get; set; }
        public bool Archivado { get; set; }
    }

    public class CierreAnioHistorialViewModel
    {
        public int Alh_Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Accion { get; set; }
        public string AccionTexto { get; set; }
        public int? EstadoAnterior { get; set; }
        public int? EstadoNuevo { get; set; }
        public string EstadoAnteriorTexto { get; set; }
        public string EstadoNuevoTexto { get; set; }
        public string Motivo { get; set; }
        public string Detalle { get; set; }
        public bool Forzado { get; set; }
        public string Usuario { get; set; }
    }

    /// <summary>Lo que el ADM manda para una acción. Usuario y permisos los pone el controller con la sesión.</summary>
    public class CierreAnioAccionViewModel
    {
        public int Ani_Id { get; set; }
        public int? Sea_Id { get; set; }
        public int? Niv_Id { get; set; }
        public int? Rea_Id { get; set; }
        public int? Pra_Id { get; set; }
        public string Accion { get; set; }
        public string Decision { get; set; }
        public string Motivo { get; set; }
        public decimal? Nota { get; set; }
        public decimal? NotaMinima { get; set; }
        public int? MaxRecuperacion { get; set; }
        public bool Forzar { get; set; }
        public bool PasarActivo { get; set; }
        public int? Usuario { get; set; }
        public bool PuedeCerrar { get; set; }
        public bool PuedeReabrir { get; set; }
    }

    /// <summary>Respuesta de la API a una acción: { Ok, Mensaje, Datos }.</summary>
    public class CierreAnioRespuestaViewModel
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; }
        public CierreAnioEscrituraViewModel Datos { get; set; }
    }

    public class CierreAnioEscrituraViewModel
    {
        public int? Ani_Id { get; set; }
        public int? Estado { get; set; }
        public string EstadoTexto { get; set; }
        public bool? Forzado { get; set; }
        public int? Alumnos { get; set; }
        public int? Materias { get; set; }
        public int? Parciales { get; set; }
        public int? Aprobadas { get; set; }
        public int? EnRecuperacion { get; set; }
        public int? Reprobadas { get; set; }
        public int? SinNotas { get; set; }
        public int? Niv_Id { get; set; }
        public decimal? NotaMinima { get; set; }
        public int? MaxRecuperacion { get; set; }
        public int? Rea_Id { get; set; }
        public string Resultado { get; set; }
        public decimal? NotaFinal { get; set; }
        public bool? AproboRecuperacion { get; set; }
        public string Propuesta { get; set; }
        public int? Pra_Id { get; set; }
        public string Decision { get; set; }
        public string Efectiva { get; set; }
        public int? Confirmados { get; set; }
        public int? Promovidos { get; set; }
        public int? Repiten { get; set; }
        public int? Graduan { get; set; }
        public int? Inscritos { get; set; }
        public int? SinSeccion { get; set; }
        public int? Movidos { get; set; }
        public int? Quitados { get; set; }
        public int? AnioDestino { get; set; }
        public int? Deshechos { get; set; }
    }
}

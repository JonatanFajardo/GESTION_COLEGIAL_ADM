using System;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Un año que se puede revisar en "Cierre de parciales".</summary>
    public class CierreParcialAnioViewModel
    {
        public int Anio { get; set; }
        public bool EsActivo { get; set; }
    }

    /// <summary>Estado simple de un período (año, semestre, parcial): cuaderno de notas y boletín.</summary>
    public class CierreParcialPeriodoViewModel
    {
        public int Anio { get; set; }
        public int Sem_Id { get; set; }
        public int Pac_Id { get; set; }
        public string Parcial { get; set; }
        public string Semestre { get; set; }
        /// <summary>abierto | cerrado</summary>
        public string Estado { get; set; }
        public bool Publicado { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string UsuarioCierre { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public int NotasPeriodo { get; set; }

        public bool EstaCerrado => string.Equals(Estado, "cerrado", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Tarjeta del tablero: estado, fechas y avance de notas de un período.</summary>
    public class CierreParcialEstadoViewModel
    {
        public int Anio { get; set; }
        public int Sem_Id { get; set; }
        public int Pac_Id { get; set; }
        public string Parcial { get; set; }
        public string Semestre { get; set; }
        public string Estado { get; set; }
        public bool Publicado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaLimite { get; set; }
        /// <summary>Fechas del semestre según los períodos del año lectivo (null si el año no los tiene).</summary>
        public DateTime? SemestreInicio { get; set; }
        public DateTime? SemestreFin { get; set; }
        public bool SemestreVigente { get; set; }
        public bool LimiteVencido { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string UsuarioCierre { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public string UsuarioPublicacion { get; set; }
        public int Esperadas { get; set; }
        public int Cargadas { get; set; }
        public int Faltantes { get; set; }
        public int SeccionesConFaltantes { get; set; }
        public int MateriasConFaltantes { get; set; }
        public int NotasPeriodo { get; set; }
        public int SeccionesSinHorario { get; set; }
        public int AlumnosSinHorario { get; set; }
    }

    /// <summary>Notas faltantes de una materia en una sección.</summary>
    public class CierreParcialFaltanteViewModel
    {
        public int Cur_Id { get; set; }
        public int Cun_Id { get; set; }
        public int Sec_Id { get; set; }
        public int Mda_Id { get; set; }
        public int Mat_Id { get; set; }
        public string Nivel { get; set; }
        public string Carrera { get; set; }
        public string Grado { get; set; }
        public int GradoOrden { get; set; }
        public string Seccion { get; set; }
        public string Modalidad { get; set; }
        public string Materia { get; set; }
        public string Docentes { get; set; }
        public int Alumnos { get; set; }
        public int Cargadas { get; set; }
        public int Faltan { get; set; }
        public decimal? Porcentaje { get; set; }
    }

    /// <summary>Una acción del historial.</summary>
    public class CierreParcialHistorialViewModel
    {
        public int Cph_Id { get; set; }
        public int Anio { get; set; }
        public int Sem_Id { get; set; }
        public int Pac_Id { get; set; }
        public string Parcial { get; set; }
        public string Semestre { get; set; }
        /// <summary>cerrar | reabrir | publicar | despublicar | fechas</summary>
        public string Accion { get; set; }
        public string Motivo { get; set; }
        public int? Esperadas { get; set; }
        public int? Cargadas { get; set; }
        public int? Faltantes { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
    }

    /// <summary>
    /// Lo que manda la pantalla para una acción. Usuario y PuedeCerrar los pone el controlador con la
    /// sesión (lo que mande el navegador se descarta).
    /// </summary>
    public class CierreParcialAccionViewModel
    {
        public int Anio { get; set; }
        public int Sem_Id { get; set; }
        public int Pac_Id { get; set; }
        public string Motivo { get; set; }
        public bool ConfirmarFaltantes { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaLimite { get; set; }
        public int? Usuario { get; set; }
        public bool PuedeCerrar { get; set; }
    }

    /// <summary>Respuesta de una acción: { Ok, Estado, Publicado, Mensaje }.</summary>
    public class CierreParcialResultadoViewModel
    {
        public bool Ok { get; set; }
        public string Estado { get; set; }
        public bool Publicado { get; set; }
        public string Mensaje { get; set; }
    }
}

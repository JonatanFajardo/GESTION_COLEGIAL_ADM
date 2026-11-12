using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Tipos de día del calendario escolar (espejo de Skuul CalendarEventType).
    /// Feriado y Cierre = el colegio cierra (sin pase de lista ese día).
    /// </summary>
    public class TipoDiaCalendario
    {
        public string Codigo { get; set; }
        public string Etiqueta { get; set; }
        public bool CierraColegio { get; set; }
        public string Icono { get; set; }
        /// <summary>Clase CSS del color (cal-t-*).</summary>
        public string Clase { get; set; }

        public static readonly IReadOnlyList<TipoDiaCalendario> Todos = new List<TipoDiaCalendario>
        {
            new TipoDiaCalendario { Codigo = "feriado",        Etiqueta = "Feriado",           CierraColegio = true,  Icono = "bi-flag",           Clase = "cal-t-feriado" },
            new TipoDiaCalendario { Codigo = "cierre",         Etiqueta = "Cierre",            CierraColegio = true,  Icono = "bi-door-closed",    Clase = "cal-t-cierre" },
            new TipoDiaCalendario { Codigo = "dia_especial",   Etiqueta = "Día especial",      CierraColegio = false, Icono = "bi-stars",          Clase = "cal-t-especial" },
            new TipoDiaCalendario { Codigo = "acto_civico",    Etiqueta = "Acto cívico",       CierraColegio = false, Icono = "bi-megaphone",      Clase = "cal-t-acto" },
            new TipoDiaCalendario { Codigo = "actividad",      Etiqueta = "Actividad",         CierraColegio = false, Icono = "bi-balloon",        Clase = "cal-t-actividad" },
            new TipoDiaCalendario { Codigo = "reunion_padres", Etiqueta = "Reunión de padres", CierraColegio = false, Icono = "bi-people",         Clase = "cal-t-reunion" },
            new TipoDiaCalendario { Codigo = "cita",           Etiqueta = "Cita",              CierraColegio = false, Icono = "bi-person-check",   Clase = "cal-t-cita" },
            new TipoDiaCalendario { Codigo = "examen",         Etiqueta = "Exámenes",          CierraColegio = false, Icono = "bi-pencil-square",  Clase = "cal-t-examen" },
            new TipoDiaCalendario { Codigo = "otro",           Etiqueta = "Evento",            CierraColegio = false, Icono = "bi-calendar-event", Clase = "cal-t-otro" },
        };

        public static TipoDiaCalendario De(string codigo) =>
            Todos.FirstOrDefault(t => t.Codigo == codigo) ?? Todos.Last();

        public static bool Existe(string codigo) => Todos.Any(t => t.Codigo == codigo);

        /// <summary>Feriado y cierre: día completo y solo para todo el colegio o para secciones.</summary>
        public static bool Cierra(string codigo) => Todos.Any(t => t.Codigo == codigo && t.CierraColegio);
    }

    /// <summary>Un día del calendario. Audiencia vacía = todo el colegio.</summary>
    public class EventoCalendarioViewModel
    {
        public int Eve_Id { get; set; }
        public int? Ani_Id { get; set; }
        public string Eve_Titulo { get; set; }
        public string Eve_Descripcion { get; set; }
        public string Eve_Tipo { get; set; }
        public string Eve_Lugar { get; set; }
        public bool Eve_TodoElDia { get; set; }
        public DateTime Eve_Inicio { get; set; }
        public DateTime Eve_Fin { get; set; }
        public bool Eve_EsPublicado { get; set; }
        public string Eve_ClaveOrigen { get; set; }
        public bool Eve_CierraColegio { get; set; }
        public string Audiencia { get; set; }
        public int AudienciaCantidad { get; set; }

        public TipoDiaCalendario Tipo => TipoDiaCalendario.De(Eve_Tipo);
        public bool EsParaTodos => AudienciaCantidad == 0;

        /// <summary>
        /// Último día que toca. Un evento con hora que termina justo a las 00:00 no toca el día
        /// siguiente (p. ej. de 18:00 a 00:00 es solo del primer día).
        /// </summary>
        public DateTime UltimoDia =>
            !Eve_TodoElDia && Eve_Fin.TimeOfDay == TimeSpan.Zero && Eve_Fin > Eve_Inicio
                ? Eve_Fin.Date.AddDays(-1)
                : Eve_Fin.Date;

        public bool VariosDias => UltimoDia > Eve_Inicio.Date;

        /// <summary>¿Toca ese día?</summary>
        public bool Cubre(DateTime dia) => Eve_Inicio.Date <= dia.Date && UltimoDia >= dia.Date;
    }

    /// <summary>Un destinatario: seccion, rol, empleado o alumno.</summary>
    public class EventoAudienciaViewModel
    {
        public int Eva_Id { get; set; }
        public int Eve_Id { get; set; }
        public string Eva_Tipo { get; set; }
        public int Eva_RefId { get; set; }
        public string Eva_Nombre { get; set; }
        public string Eva_Detalle { get; set; }
        public int Eva_Orden { get; set; }
    }

    /// <summary>Datos que manda el editor para crear (Eve_Id = 0) o cambiar un día.</summary>
    public class EventoCalendarioGuardarViewModel
    {
        public int Eve_Id { get; set; }
        public string Eve_Titulo { get; set; }
        public string Eve_Descripcion { get; set; }
        public string Eve_Tipo { get; set; }
        public string Eve_Lugar { get; set; }
        public bool Eve_TodoElDia { get; set; } = true;
        public DateTime Eve_Inicio { get; set; }
        public DateTime Eve_Fin { get; set; }
        public List<int> Secciones { get; set; } = new List<int>();
        public List<int> Roles { get; set; } = new List<int>();
        public List<int> Empleados { get; set; } = new List<int>();
        public List<int> Alumnos { get; set; } = new List<int>();
        public string Eve_ClaveOrigen { get; set; }
        public bool? Publicar { get; set; }
        public int? Usuario { get; set; }
    }

    public class EventoGuardarResultadoViewModel
    {
        public int Eve_Id { get; set; }
        public bool Creado { get; set; }
        public string Mensaje { get; set; }
    }

    public class EventoPublicarResultadoViewModel
    {
        public int Eve_Id { get; set; }
        public bool Eve_EsPublicado { get; set; }
    }

    public class EventoEliminarResultadoViewModel
    {
        public int Eve_Id { get; set; }
        public string Mes { get; set; }
    }

    /// <summary>Borradores de un mes (Mes = "yyyy-MM").</summary>
    public class BorradoresCalendarioViewModel
    {
        public string Mes { get; set; }
        public int Borradores { get; set; }
    }

    /// <summary>Si el colegio cierra un día para una sección.</summary>
    public class CierreCalendarioViewModel
    {
        public bool Cerrado { get; set; }
        public int? Eve_Id { get; set; }
        public string Eve_Titulo { get; set; }
        public string Eve_Tipo { get; set; }
        public DateTime? Eve_Inicio { get; set; }
        public DateTime? Eve_Fin { get; set; }
        public int? Sea_Id { get; set; }
        public DateTime Fecha { get; set; }
    }

    /// <summary>Un día de hoy o de los próximos días (tarjeta de los dashboards).</summary>
    public class ProximoEventoViewModel
    {
        public bool EsHoy { get; set; }
        public int Eve_Id { get; set; }
        public string Eve_Titulo { get; set; }
        public string Eve_Tipo { get; set; }
        public string Eve_Lugar { get; set; }
        public bool Eve_TodoElDia { get; set; }
        public DateTime Eve_Inicio { get; set; }
        public DateTime Eve_Fin { get; set; }
        public bool Eve_CierraColegio { get; set; }
    }

    public class SeccionAudienciaViewModel
    {
        public int Sea_Id { get; set; }
        public string Sea_Nombre { get; set; }
        public int Ani_Id { get; set; }
        public int Ani_Anio { get; set; }
        public int? Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }
        public int Sea_Estado { get; set; }
        public int Cun_Orden { get; set; }
    }

    public class RolAudienciaViewModel
    {
        public int Rol_Id { get; set; }
        public string Rol_Etiqueta { get; set; }
    }

    /// <summary>Persona encontrada: Tipo = empleado | alumno.</summary>
    public class PersonaAudienciaViewModel
    {
        public string Tipo { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Detalle { get; set; }
    }

    /// <summary>Feriado o fecha escolar de Honduras propuesto para un año.</summary>
    public class FeriadoPropuestoViewModel
    {
        public string Clave { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public string Descripcion { get; set; }
        public string Grupo { get; set; }
        public bool Sugerido { get; set; }
        public bool Verificar { get; set; }
        public bool YaCargado { get; set; }
        public int? Eve_Id { get; set; }
        public bool? Publicado { get; set; }
    }

    public class FeriadoACargarViewModel
    {
        public string Clave { get; set; }
        public bool Publicar { get; set; }
    }

    public class CargarFeriadosViewModel
    {
        public int Anio { get; set; }
        public List<FeriadoACargarViewModel> Items { get; set; } = new List<FeriadoACargarViewModel>();
        public int? Usuario { get; set; }
    }

    public class CargarFeriadosResultadoViewModel
    {
        public int Creados { get; set; }
        public int YaEstaban { get; set; }
        /// <summary>Si algo falló a medias, qué faltó (null si todo salió bien).</summary>
        public string Mensaje { get; set; }
    }
}

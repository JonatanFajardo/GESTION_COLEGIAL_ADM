using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Vista de mes del calendario escolar (cuadrícula + "Qué hay este mes").</summary>
    public class CalendarioMesViewModel
    {
        public static readonly CultureInfo Cultura = new CultureInfo("es-HN");

        public DateTime Mes { get; set; }
        public DateTime Hoy { get; set; }
        /// <summary>Días de la cuadrícula: de lunes a domingo, cubriendo el mes completo.</summary>
        public List<DateTime> Dias { get; set; } = new List<DateTime>();
        /// <summary>Días del calendario ya filtrados (tipo / borradores / permiso) en el rango de la cuadrícula.</summary>
        public List<EventoCalendarioViewModel> Eventos { get; set; } = new List<EventoCalendarioViewModel>();
        /// <summary>Feriados y cierres publicados del mes (sin filtro): "El colegio cierra el…".</summary>
        public List<EventoCalendarioViewModel> Cierres { get; set; } = new List<EventoCalendarioViewModel>();
        public string Tipo { get; set; }
        public bool SoloBorradores { get; set; }
        public bool PuedeEditar { get; set; }
        /// <summary>Borradores por mes (solo para quien edita).</summary>
        public List<BorradoresCalendarioViewModel> BorradoresPorMes { get; set; } = new List<BorradoresCalendarioViewModel>();
        public int Borradores => BorradoresPorMes.Sum(b => b.Borradores);
        public int BorradoresDelMes => BorradoresPorMes.Where(b => b.Mes == Clave(Mes)).Sum(b => b.Borradores);
        public string Aviso { get; set; }

        public DateTime MesAnterior => Mes.AddMonths(-1);
        public DateTime MesSiguiente => Mes.AddMonths(1);
        public bool HayFiltro => !string.IsNullOrEmpty(Tipo) || SoloBorradores;

        /// <summary>Los del mes mostrado (la cuadrícula trae también días de los meses vecinos).</summary>
        public List<EventoCalendarioViewModel> DelMes =>
            Eventos.Where(e => e.Eve_Inicio.Date <= Mes.AddMonths(1).AddDays(-1) && e.UltimoDia >= Mes).ToList();

        public IEnumerable<EventoCalendarioViewModel> EventosDe(DateTime dia) => Eventos.Where(e => e.Cubre(dia));

        public bool CierraEl(DateTime dia) => Cierres.Any(e => e.Cubre(dia) && e.EsParaTodos);

        public static string NombreMes(DateTime d) => Cultura.TextInfo.ToTitleCase(d.ToString("MMMM yyyy", Cultura));
        public static string MesCorto(DateTime d) => Cultura.TextInfo.ToTitleCase(d.ToString("MMM", Cultura).TrimEnd('.'));
        public static string Clave(DateTime d) => d.ToString("yyyy-MM");

        /// <summary>"15 sep" o "7–9 oct" / "30 mar – 1 abr".</summary>
        public static string Rango(EventoCalendarioViewModel e)
        {
            string ini = e.Eve_Inicio.ToString("d MMM", Cultura).TrimEnd('.');
            if (!e.VariosDias) return ini;
            if (e.Eve_Inicio.Month == e.UltimoDia.Month)
                return e.Eve_Inicio.Day + "–" + e.UltimoDia.ToString("d MMM", Cultura).TrimEnd('.');
            return ini + " – " + e.UltimoDia.ToString("d MMM", Cultura).TrimEnd('.');
        }

        public static string Horario(EventoCalendarioViewModel e) =>
            e.Eve_TodoElDia ? "Todo el día" : e.Eve_Inicio.ToString("HH:mm") + " a " + e.Eve_Fin.ToString("HH:mm");

        /// <summary>"El colegio cierra el 15 sep, 7–9 oct y 25 dic."</summary>
        public string FraseCierres()
        {
            if (Cierres.Count == 0) return "El colegio abre todos los días este mes.";
            var partes = Cierres.Select(c => Rango(c) + (c.EsParaTodos ? "" : " (algunas secciones)")).ToList();
            string lista = partes.Count == 1
                ? partes[0]
                : string.Join(", ", partes.Take(partes.Count - 1)) + " y " + partes.Last();
            return "El colegio cierra el " + lista + ".";
        }
    }

    /// <summary>Editor (o ficha de solo lectura) de un día del calendario.</summary>
    public class CalendarioDiaViewModel
    {
        public EventoCalendarioViewModel Evento { get; set; }
        public List<EventoAudienciaViewModel> Audiencia { get; set; } = new List<EventoAudienciaViewModel>();
        public List<SeccionAudienciaViewModel> Secciones { get; set; } = new List<SeccionAudienciaViewModel>();
        public List<RolAudienciaViewModel> Roles { get; set; } = new List<RolAudienciaViewModel>();
        public bool EsNuevo { get; set; }
        public bool PuedeEditar { get; set; }
        public string Aviso { get; set; }

        public IEnumerable<int> Ids(string tipo) => Audiencia.Where(a => a.Eva_Tipo == tipo).Select(a => a.Eva_RefId);

        public int AnioSecciones => Secciones.Select(s => s.Ani_Anio).DefaultIfEmpty(Evento?.Eve_Inicio.Year ?? DateTime.Today.Year).Max();
    }
}

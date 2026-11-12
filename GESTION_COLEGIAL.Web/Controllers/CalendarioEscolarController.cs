using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Calendario escolar (espejo de Skuul "Calendar"). Todos los roles lo leen; crean, publican y
    /// quitan días quienes tienen "Editar calendario escolar" (admin, dirección, secretaría).
    /// Un Feriado o Cierre publicado cierra el colegio: ese día la sección no tiene pase de lista.
    /// </summary>
    [SessionManager("Calendario escolar")]
    public class CalendarioEscolarController : BaseController
    {
        private const string PantallaEditar = "Editar calendario escolar";
        private readonly CalendarioEscolarService _service = new CalendarioEscolarService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool PuedeEditar() => EsAdmin() || TienePantalla(PantallaEditar);

        private int? SesionEntero(string clave) =>
            int.TryParse(Session[clave]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        private static DateTime? Mes(string mes) =>
            DateTime.TryParseExact(mes ?? "", "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)
                ? d : (DateTime?)null;

        private static DateTime? Dia(string dia) =>
            DateTime.TryParseExact(dia ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)
                ? d : (DateTime?)null;

        private static string TextoAviso(string aviso) => aviso switch
        {
            "borrador" => "Guardado como borrador. Publíquelo cuando esté listo.",
            "guardado" => "Guardado.",
            "publicado" => "Publicado. El colegio ya lo puede ver.",
            "despublicado" => "Es borrador otra vez: nadie más lo ve.",
            "quitado" => "Se quitó del calendario.",
            _ => null
        };

        // ───────────────────────────── Vista de mes ─────────────────────────────

        /// <summary>/calendarioescolar?mes=2026-11&amp;tipo=feriado&amp;borradores=true</summary>
        public async Task<ActionResult> Index(string mes, string tipo, bool borradores = false, string aviso = null)
        {
            DateTime hoy = HoraHonduras.Hoy;
            DateTime inicioMes = Mes(mes) ?? new DateTime(hoy.Year, hoy.Month, 1);
            bool puedeEditar = PuedeEditar();
            if (!puedeEditar) borradores = false;
            if (!TipoDiaCalendario.Existe(tipo)) tipo = null;

            DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);
            DateTime desde = inicioMes.AddDays(-(((int)inicioMes.DayOfWeek + 6) % 7));
            DateTime hasta = finMes.AddDays(6 - (((int)finMes.DayOfWeek + 6) % 7));

            // Quien edita ve todo (también borradores); el resto, lo de todo el colegio y lo que es para él.
            List<EventoCalendarioViewModel> todos = await _service.ListRangoAsync(
                desde, hasta, puedeEditar, puedeEditar, SesionEntero(SesionUsuario.EmpId), SesionEntero(SesionUsuario.RolId));

            var modelo = new CalendarioMesViewModel
            {
                Mes = inicioMes,
                Hoy = hoy,
                Tipo = tipo,
                SoloBorradores = borradores,
                PuedeEditar = puedeEditar,
                BorradoresPorMes = puedeEditar ? await _service.BorradoresAsync() : new List<BorradoresCalendarioViewModel>(),
                Aviso = TextoAviso(aviso),
                Eventos = todos
                    .Where(e => tipo == null || e.Eve_Tipo == tipo)
                    .Where(e => !borradores || !e.Eve_EsPublicado)
                    .ToList(),
                Cierres = todos
                    .Where(e => e.Eve_EsPublicado && e.Eve_CierraColegio && e.Eve_Inicio.Date <= finMes && e.Eve_Fin.Date >= inicioMes)
                    .ToList()
            };
            for (DateTime d = desde; d <= hasta; d = d.AddDays(1)) modelo.Dias.Add(d);

            return View(modelo);
        }

        // ───────────────────────────── Un día ─────────────────────────────

        /// <summary>Agregar un día (editor vacío). ?dia=2026-11-20 lo arranca en esa fecha.</summary>
        public async Task<ActionResult> Nuevo(string dia)
        {
            if (!PuedeEditar()) return RedirectToAction("SinAcceso", "Account");

            DateTime fecha = Dia(dia) ?? HoraHonduras.Hoy;
            var modelo = new CalendarioDiaViewModel
            {
                EsNuevo = true,
                PuedeEditar = true,
                Evento = new EventoCalendarioViewModel
                {
                    Eve_Tipo = TipoDiaCalendario.Todos[0].Codigo,
                    Eve_TodoElDia = true,
                    Eve_Inicio = fecha,
                    Eve_Fin = fecha
                },
                Secciones = await _service.SeccionesAudienciaAsync(fecha, null),
                Roles = await _service.RolesAudienciaAsync()
            };
            return View("Dia", modelo);
        }

        /// <summary>/calendarioescolar/dia/5: editor para quien edita, ficha de solo lectura para el resto.</summary>
        public async Task<ActionResult> Dia(int id, string aviso = null)
        {
            bool puedeEditar = PuedeEditar();
            // Para quien no edita, la API solo lo devuelve si está publicado y es para esa persona.
            EventoCalendarioViewModel evento = await _service.FindAsync(
                id, puedeEditar, SesionEntero(SesionUsuario.EmpId), SesionEntero(SesionUsuario.RolId));
            if (evento == null || evento.Eve_Id == 0 || (!puedeEditar && !evento.Eve_EsPublicado))
                return RedirectToAction("Index");

            var modelo = new CalendarioDiaViewModel
            {
                Evento = evento,
                PuedeEditar = puedeEditar,
                Aviso = TextoAviso(aviso),
                Audiencia = await _service.AudienciaAsync(id)
            };
            if (puedeEditar)
            {
                modelo.Secciones = await _service.SeccionesAudienciaAsync(evento.Eve_Inicio.Date, id);
                modelo.Roles = await _service.RolesAudienciaAsync();
            }
            return View("Dia", modelo);
        }

        /// <summary>Secciones del año de una fecha (el editor las recarga si la fecha cambia de año).</summary>
        public async Task<ActionResult> SeccionesAsync(string fecha, int? eveId)
        {
            if (!PuedeEditar()) return Json(new List<SeccionAudienciaViewModel>());
            return Json(await _service.SeccionesAudienciaAsync(Dia(fecha) ?? HoraHonduras.Hoy, eveId));
        }

        /// <summary>Buscar un empleado o alumno para nombrarlo (mínimo 2 letras).</summary>
        public async Task<ActionResult> BuscarPersonasAsync(string texto)
        {
            if (!PuedeEditar() || (texto ?? "").Trim().Length < 2) return Json(new List<PersonaAudienciaViewModel>());
            return Json(await _service.BuscarPersonasAsync(texto.Trim()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarAsync([FromBody] EventoCalendarioGuardarViewModel model)
        {
            if (!PuedeEditar()) return Json(new { success = false, message = "No tiene permiso para editar el calendario." });
            if (model == null) return Json(new { success = false, message = "No llegaron datos." });

            // Feriado y cierre cierran el día completo.
            if (TipoDiaCalendario.Cierra(model.Eve_Tipo)) model.Eve_TodoElDia = true;

            string problema = Validar(model);
            if (problema != null) return Json(new { success = false, message = problema });

            model.Eve_ClaveOrigen = null;   // la clave solo la ponen los feriados precargados
            model.Publicar = null;          // todo día nuevo nace borrador
            model.Usuario = GetUsuarioId();

            EventoGuardarResultadoViewModel r = await _service.GuardarAsync(model);
            if (r == null || r.Eve_Id == 0)
                return Json(new { success = false, message = r?.Mensaje ?? "No se pudo guardar el día en el calendario." });

            bool esNuevo = model.Eve_Id == 0;
            return Json(new
            {
                success = true,
                id = r.Eve_Id,
                creado = esNuevo,
                message = esNuevo ? "Guardado como borrador. Publíquelo cuando esté listo." : "Guardado.",
                redirect = esNuevo ? Url.Action("Dia", new { id = r.Eve_Id, aviso = "borrador" }) : null
            });
        }

        /// <summary>Mismas reglas que Skuul: título, tipo, fin después del inicio y no más de un año.</summary>
        private static string Validar(EventoCalendarioGuardarViewModel m)
        {
            if (string.IsNullOrWhiteSpace(m.Eve_Titulo)) return "Escriba un título.";
            if (m.Eve_Titulo.Trim().Length > 150) return "El título admite hasta 150 caracteres.";
            if (!TipoDiaCalendario.Existe(m.Eve_Tipo)) return "Elija el tipo de día.";
            if ((m.Eve_Lugar ?? "").Trim().Length > 150) return "El lugar admite hasta 150 caracteres.";
            if ((m.Eve_Descripcion ?? "").Trim().Length > 2000) return "La descripción admite hasta 2000 caracteres.";
            if (m.Eve_Inicio == default || m.Eve_Fin == default) return "Indique cuándo empieza y cuándo termina.";
            if (m.Eve_Inicio.Year < 2000 || m.Eve_Inicio.Year > 2100 || m.Eve_Fin.Year < 2000 || m.Eve_Fin.Year > 2100)
                return "Revise el año de las fechas.";
            if (TipoDiaCalendario.Cierra(m.Eve_Tipo)
                && ((m.Roles?.Count ?? 0) > 0 || (m.Empleados?.Count ?? 0) > 0 || (m.Alumnos?.Count ?? 0) > 0))
                return "Un feriado o cierre es para todo el colegio o para secciones: quite los roles y las personas.";

            DateTime ini = m.Eve_TodoElDia ? m.Eve_Inicio.Date : m.Eve_Inicio;
            DateTime fin = m.Eve_TodoElDia ? m.Eve_Fin.Date.AddDays(1).AddSeconds(-1) : m.Eve_Fin;
            if (fin < ini) return "Un día del calendario no puede terminar antes de empezar.";
            if (ini.AddYears(1) < fin) return "Un día del calendario no puede durar más de un año. Revise el año.";
            return null;
        }

        /// <summary>publicar = true: el colegio lo ve; false: vuelve a borrador.</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PublicarAsync(int id, bool publicar = true)
        {
            if (!PuedeEditar()) return Json(new { success = false, message = "No tiene permiso para publicar." });
            EventoPublicarResultadoViewModel r = await _service.PublicarAsync(id, publicar, GetUsuarioId());
            if (r == null || r.Eve_Id == 0) return Json(new { success = false, message = "No se pudo cambiar la publicación." });
            return Json(new
            {
                success = true,
                publicado = r.Eve_EsPublicado,
                message = r.Eve_EsPublicado
                    ? "Publicado. El colegio ya lo puede ver."
                    : "Es borrador otra vez: nadie más lo ve."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EliminarAsync(int id)
        {
            if (!PuedeEditar()) return Json(new { success = false, message = "No tiene permiso para quitar días." });
            EventoEliminarResultadoViewModel r = await _service.EliminarAsync(id, GetUsuarioId());
            if (r == null || r.Eve_Id == 0) return Json(new { success = false, message = "No se pudo quitar del calendario." });
            return Json(new { success = true, redirect = Url.Action("Index", new { mes = r.Mes, aviso = "quitado" }) });
        }

        // ───────────────────────────── Feriados de Honduras ─────────────────────────────

        public async Task<ActionResult> FeriadosAsync(int anio)
        {
            if (!PuedeEditar()) return Json(new List<FeriadoPropuestoViewModel>());
            return Json(await _service.FeriadosHondurasAsync(anio));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CargarFeriadosAsync([FromBody] CargarFeriadosViewModel model)
        {
            if (!PuedeEditar()) return Json(new { success = false, message = "No tiene permiso para editar el calendario." });
            if (model == null || model.Items == null || model.Items.Count == 0)
                return Json(new { success = false, message = "Marque al menos un día." });

            model.Usuario = GetUsuarioId();
            CargarFeriadosResultadoViewModel r = await _service.CargarFeriadosHondurasAsync(model);
            if (r == null) return Json(new { success = false, creados = 0, yaEstaban = 0, message = "No se pudieron cargar los feriados." });
            // Si falló a medias, se avisa qué faltó (lo que sí se cargó queda en el calendario).
            return Json(new { success = string.IsNullOrEmpty(r.Mensaje), creados = r.Creados, yaEstaban = r.YaEstaban, message = r.Mensaje });
        }

        // ───────────────────────────── Tarjeta de los dashboards ─────────────────────────────

        /// <summary>
        /// Hoy y los próximos 7 días. Quien edita el calendario ve todo lo publicado; el resto ve lo de
        /// todo el colegio, su rol, su nombre y (docentes) sus secciones del año.
        /// </summary>
        public async Task<ActionResult> ProximosAsync()
        {
            DateTime hoy = HoraHonduras.Hoy;
            List<ProximoEventoViewModel> lista = await _service.ProximosAsync(
                hoy, 7, PuedeEditar(), SesionEntero(SesionUsuario.EmpId), SesionEntero(SesionUsuario.RolId));

            var cultura = CalendarioMesViewModel.Cultura;
            return Json(lista.Select(e => new
            {
                id = e.Eve_Id,
                titulo = e.Eve_Titulo,
                tipo = TipoDiaCalendario.De(e.Eve_Tipo).Etiqueta,
                clase = TipoDiaCalendario.De(e.Eve_Tipo).Clase,
                icono = TipoDiaCalendario.De(e.Eve_Tipo).Icono,
                lugar = e.Eve_Lugar,
                cierra = e.Eve_CierraColegio,
                esHoy = e.EsHoy,
                fecha = e.EsHoy ? "Hoy" : cultura.TextInfo.ToTitleCase(e.Eve_Inicio.ToString("ddd d MMM", cultura).Replace(".", "")),
                hora = e.Eve_TodoElDia ? "Todo el día" : e.Eve_Inicio.ToString("HH:mm")
            }));
        }
    }
}

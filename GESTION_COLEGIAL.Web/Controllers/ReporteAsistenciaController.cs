using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Reporte de asistencia de alumnos, a partir del pase de lista: resumen por sección y,
    /// de una sección, el resumen por alumno con su planilla día por día.
    /// Permisos: "Reporte de asistencia de alumnos" (ver) y "Ver asistencia de todas las secciones".
    /// Sin el segundo (el docente) solo se ven las secciones del propio empleado.
    /// </summary>
    [SessionManager("Reporte de asistencia de alumnos")]
    public class ReporteAsistenciaController : BaseController
    {
        private const string PantallaTodas = "Ver asistencia de todas las secciones";
        private readonly ReporteAsistenciaService _service = new ReporteAsistenciaService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool VerTodas() => EsAdmin() || TienePantalla(PantallaTodas);

        private int? EmpId() =>
            int.TryParse(Session[SesionUsuario.EmpId]?.ToString(), out int v) && v > 0 ? v : (int?)null;

        /// <summary>
        /// Arregla el rango pedido: por defecto los últimos 30 días y nunca más allá de hoy.
        /// Devuelve un mensaje si el rango no sirve.
        /// </summary>
        private static string NormalizarRango(ref DateTime? desde, ref DateTime? hasta, DateTime hoy)
        {
            DateTime h = (hasta ?? hoy).Date;
            if (h > hoy) h = hoy;
            DateTime d = (desde ?? h.AddDays(-29)).Date;
            hasta = h;
            desde = d;
            if (d > h) return "La fecha «Desde» tiene que ser anterior o igual a la fecha «Hasta».";
            if ((h - d).TotalDays + 1 > FormatoAsistencia.MaxDias)
                return $"Elija un rango de {FormatoAsistencia.MaxDias} días como máximo (un año escolar).";
            return null;
        }

        private static List<AtajoAsistencia> Atajos(List<AsistenciaPeriodoViewModel> periodos, DateTime hoy, DateTime desde, DateTime hasta)
        {
            var atajos = periodos
                .Where(p => p.Pan_FechaInicio.Date <= hoy)
                .OrderBy(p => p.Pan_FechaInicio)
                .Select(p => new AtajoAsistencia
                {
                    Nombre = p.Pan_Nombre,
                    Desde = p.Pan_FechaInicio.Date,
                    Hasta = p.Pan_FechaFin.Date > hoy ? hoy : p.Pan_FechaFin.Date
                })
                .ToList();
            atajos.Add(new AtajoAsistencia { Nombre = "Este mes", Desde = new DateTime(hoy.Year, hoy.Month, 1), Hasta = hoy });
            atajos.Add(new AtajoAsistencia { Nombre = "Últimos 30 días", Desde = hoy.AddDays(-29), Hasta = hoy });
            foreach (var a in atajos) a.Activo = a.Desde == desde && a.Hasta == hasta;
            return atajos;
        }

        // ───────────────────────────── Resumen por sección ─────────────────────────────

        /// <summary>/reporteasistencia?desde=2026-10-01&amp;hasta=2026-10-31&amp;nivId=2&amp;cunId=8&amp;seaId=15</summary>
        public async Task<ActionResult> Index(DateTime? desde, DateTime? hasta, int? nivId, int? cunId, int? seaId)
        {
            DateTime hoy = HoraHonduras.Hoy;
            string error = NormalizarRango(ref desde, ref hasta, hoy);
            bool verTodas = VerTodas();
            int? empId = verTodas ? null : EmpId();

            var modelo = new ReporteAsistenciaIndexViewModel
            {
                Hoy = hoy,
                Desde = desde.Value,
                Hasta = hasta.Value,
                Error = error,
                VerTodas = verTodas,
                Anio = await _service.AnioAsync(hoy)
            };
            if (modelo.Anio == null || modelo.Anio.Ani_Id == 0) { modelo.Anio = null; return View(modelo); }

            int aniId = modelo.Anio.Ani_Id;
            modelo.Atajos = Atajos(await _service.PeriodosAsync(aniId), hoy, modelo.Desde, modelo.Hasta);

            // El docente sin empleado asociado no tiene secciones.
            if (!verTodas && !empId.HasValue) return View(modelo);
            modelo.Filtros = await _service.FiltrosAsync(aniId, empId);

            // Cascada estricta: un hijo solo vale si pertenece a su padre.
            if (!modelo.Filtros.Any(f => f.Niv_Id == nivId)) nivId = null;
            if (!nivId.HasValue || !modelo.Filtros.Any(f => f.Niv_Id == nivId && f.Cun_Id == cunId)) cunId = null;
            if (!cunId.HasValue || !modelo.Filtros.Any(f => f.Cun_Id == cunId && f.Sea_Id == seaId)) seaId = null;
            modelo.NivId = nivId;
            modelo.CunId = cunId;
            modelo.SeaId = seaId;

            if (error == null && modelo.Filtros.Count > 0)
                modelo.Secciones = await _service.SeccionesAsync(aniId, modelo.Desde, modelo.Hasta, nivId, cunId, seaId, empId);

            return View(modelo);
        }

        // ───────────────────────────── Una sección ─────────────────────────────

        /// <summary>/reporteasistencia/seccion/15?desde=…&amp;hasta=…</summary>
        public async Task<ActionResult> Seccion(int id, DateTime? desde, DateTime? hasta, int? nivId, int? cunId, int? seaId)
        {
            DateTime hoy = HoraHonduras.Hoy;
            string error = NormalizarRango(ref desde, ref hasta, hoy);
            var volver = new { desde = FormatoAsistencia.Iso(desde.Value), hasta = FormatoAsistencia.Iso(hasta.Value), nivId, cunId, seaId };
            if (error != null) return RedirectToAction("Index", volver);

            AsistenciaSeccionInfoViewModel info = await _service.SeccionInfoAsync(id);
            if (info == null || info.Sea_Id == 0) return RedirectToAction("Index", volver);

            // Sin permiso para todas: solo las secciones del propio docente.
            if (!VerTodas())
            {
                int? empId = EmpId();
                if (!empId.HasValue) return RedirectToAction("Index", volver);
                var propias = await _service.FiltrosAsync(info.Ani_Id, empId);
                if (!propias.Any(f => f.Sea_Id == id)) return RedirectToAction("Index", volver);
            }

            var planilla = await _service.PlanillaAsync(id, desde.Value, hasta.Value);
            var modelo = new ReporteAsistenciaSeccionViewModel
            {
                Info = info,
                Desde = desde.Value,
                Hasta = hasta.Value,
                NivId = nivId,
                CunId = cunId,
                SeaIdFiltro = seaId,
                Alumnos = await _service.AlumnosAsync(id, desde.Value, hasta.Value),
                Dias = (await _service.DiasAsync(id, desde.Value, hasta.Value)).OrderBy(d => d.Fecha).ToList(),
                Planilla = planilla
                    .Where(p => !string.IsNullOrWhiteSpace(p.Estado))
                    .GroupBy(p => (p.Alu_Id, p.Fecha.Date))
                    .ToDictionary(g => g.Key, g => g.First().Estado.Trim().ToUpperInvariant())
            };
            return View(modelo);
        }
    }
}

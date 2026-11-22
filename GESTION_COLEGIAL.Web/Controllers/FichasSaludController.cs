using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Compat;
using GESTION_COLEGIAL.UI.Filters;
using GESTION_COLEGIAL.UI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Ficha de salud del alumno (espejo de Skuul "health records"): directorio con contadores y filtros,
    /// la ficha de un alumno (bloque de emergencia, formulario, historial, impresión) y la pestaña "Salud"
    /// de la ficha 360. Permisos: "Fichas de salud" (ver) y "Editar ficha de salud" (crear y cambiar).
    /// La alerta del pase de lista vive en AsistenciaAdmController (permiso "Pase de lista").
    /// </summary>
    [SessionManager("Fichas de salud")]
    public class FichasSaludController : BaseController
    {
        private const string PantallaEditar = "Editar ficha de salud";
        private readonly FichasSaludService _service = new FichasSaludService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool PuedeEditar() => EsAdmin() || TienePantalla(PantallaEditar);

        private static string TextoAviso(string aviso) => aviso switch
        {
            "creada" => "Se creó la ficha de salud.",
            "editada" => "Se guardaron los cambios de la ficha.",
            "sin_cambios" => "No había cambios que guardar.",
            _ => null
        };

        // ───────────────────────────── Directorio ─────────────────────────────

        public ActionResult Index()
        {
            ViewBag.PuedeEditar = PuedeEditar();
            return View();
        }

        /// <summary>Todos los alumnos activos con su ficha; la pantalla filtra y cuenta en el navegador.</summary>
        public async Task<ActionResult> DirectorioAsync()
        {
            try
            {
                return AjaxResult(await _service.DirectorioAsync());
            }
            catch
            {
                return AjaxResult(new List<FichaSaludDirectorioViewModel>());
            }
        }

        // ───────────────────────────── Ficha del alumno ─────────────────────────────

        private async Task<FichaSaludEmergenciaViewModel> Emergencia(int aluId)
        {
            try { return await _service.EmergenciaAsync(aluId); }
            catch { return null; }
        }

        /// <summary>/FichasSalud/Ficha/5</summary>
        public async Task<ActionResult> Ficha(int id, string aviso = null)
        {
            var emergencia = await Emergencia(id);
            if (emergencia == null) return RedirectToAction("Index");

            List<FichaSaludBitacoraViewModel> historial;
            try { historial = await _service.BitacoraAsync(id); }
            catch { historial = new List<FichaSaludBitacoraViewModel>(); }

            return View(new FichaSaludPaginaViewModel
            {
                Emergencia = emergencia,
                Historial = historial,
                PuedeEditar = PuedeEditar(),
                Aviso = TextoAviso(aviso)
            });
        }

        /// <summary>Hoja tamaño carta para imprimir (excursiones): /FichasSalud/Imprimir/5</summary>
        public async Task<ActionResult> Imprimir(int id)
        {
            var emergencia = await Emergencia(id);
            if (emergencia == null) return RedirectToAction("Index");
            try
            {
                var inst = new InstitutoConfigService().Load(RutasApp.MapPath("~/App_Data/instituto-config.json"));
                ViewBag.Institucion = inst?.Nombre;
                ViewBag.Direccion = inst?.Direccion;
            }
            catch { /* sin membrete: la hoja sigue sirviendo */ }
            ViewBag.Impreso = HoraHonduras.Ahora;
            return View(emergencia);
        }

        /// <summary>Pestaña "Salud" de la ficha 360 del alumno: solo el resumen de emergencia.</summary>
        public async Task<ActionResult> TabAsync(int id)
        {
            return PartialView("_TabSalud", await Emergencia(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GuardarAsync([FromBody] FichaSaludGuardarViewModel model)
        {
            if (!PuedeEditar()) return Json(new { success = false, message = "No tiene permiso para editar fichas de salud." });
            if (model == null || model.Alu_Id <= 0) return Json(new { success = false, message = "No llegaron datos." });

            // Mismas reglas que la API; el campo con problema vuelve para marcarlo en el formulario.
            if (!string.IsNullOrWhiteSpace(model.Fsa_TipoSangre) && Array.IndexOf(FormatoSalud.TiposSangre, model.Fsa_TipoSangre.Trim().ToUpperInvariant()) < 0)
                return Json(new { success = false, field = "TipoSangre", message = "Elija un tipo de sangre de la lista." });
            if (model.Fsa_TieneAlerta && string.IsNullOrWhiteSpace(model.Fsa_Alerta))
                return Json(new { success = false, field = "Alerta", message = "Escriba la alerta que verá el docente." });
            if ((model.Fsa_Alerta ?? "").Trim().Length > 150)
                return Json(new { success = false, field = "Alerta", message = "La alerta admite hasta 150 caracteres." });
            if (!FormatoSalud.EsTelefono(model.Fsa_MedicoTelefono))
                return Json(new { success = false, field = "MedicoTelefono", message = "Escriba 8 dígitos (ej. 9999-9999)." });
            if (!FormatoSalud.EsTelefono(model.Fsa_ContactoEmergenciaTelefono))
                return Json(new { success = false, field = "ContactoEmergenciaTelefono", message = "Escriba 8 dígitos (ej. 9999-9999)." });
            bool hayNombre = !string.IsNullOrWhiteSpace(model.Fsa_ContactoEmergenciaNombre);
            bool hayTelefono = !string.IsNullOrWhiteSpace(model.Fsa_ContactoEmergenciaTelefono);
            if (!hayTelefono && (hayNombre || !string.IsNullOrWhiteSpace(model.Fsa_ContactoEmergenciaParentesco)))
                return Json(new { success = false, field = "ContactoEmergenciaTelefono", message = "Escriba el teléfono del contacto adicional." });
            if (!hayNombre && hayTelefono)
                return Json(new { success = false, field = "ContactoEmergenciaNombre", message = "Escriba el nombre del contacto adicional." });

            model.Usuario = GetUsuarioId();
            FichaSaludResultadoViewModel r = await _service.GuardarAsync(model);
            if (r == null || !r.Ok)
                return Json(new { success = false, message = r?.Mensaje ?? "No se pudo guardar la ficha de salud." });

            return Json(new
            {
                success = true,
                accion = r.Accion,
                message = TextoAviso(r.Accion),
                redirect = Url.Action("Ficha", new { id = model.Alu_Id, aviso = r.Accion })
            });
        }
    }
}

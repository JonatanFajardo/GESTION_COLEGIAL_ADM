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
    /// Constancias y expediente (espejo de Skuul "Transcripts" + "Report cards"): documentos oficiales
    /// congelados de un alumno. Tres pantallas: listado, Emitir un documento y el documento imprimible.
    /// Permisos: "Constancias y expediente" (ver) y "Emitir constancias" (vista previa y emisión).
    /// Un documento emitido no cambia: para corregirlo se emite una revisión nueva con su motivo.
    /// </summary>
    [SessionManager("Constancias y expediente")]
    public class ConstanciasController : BaseController
    {
        private const string PantallaEmitir = "Emitir constancias";
        private const int MaxMotivo = 500;

        private readonly DocumentosEmitidosService _service = new DocumentosEmitidosService();
        private readonly AniosLectivosService _anios = new AniosLectivosService();

        private bool EsAdmin() =>
            (Session[SesionUsuario.RoleName] as string ?? "").Equals("admin", StringComparison.OrdinalIgnoreCase);

        private bool PuedeEmitir() => EsAdmin() || TienePantalla(PantallaEmitir);

        private string RutaInstituto => RutasApp.MapPath("~/App_Data/instituto-config.json");

        private async Task<List<AnioLectivoViewModel>> AniosAsync() =>
            (await _anios.ListAsync())?.OrderByDescending(a => a.Ani_Anio).ToList() ?? new List<AnioLectivoViewModel>();

        /// <summary>Completa el documento con el membrete del colegio.</summary>
        private DocumentoImpreso ConMembrete(DocumentoImpreso d)
        {
            var inst = new InstitutoConfigService().Load(RutaInstituto);
            d.Institucion = inst.Nombre;
            d.Direccion = inst.Direccion;
            return d;
        }

        private static string TextoAviso(string aviso) => aviso switch
        {
            "emitido" => "Se emitió el documento. Ya no cambia: si hay que corregirlo, emita una revisión nueva.",
            _ => null
        };

        // ───────────────────────────── Listado ─────────────────────────────

        /// <summary>/constancias?tipo=BOLETIN&amp;aniId=2&amp;aluId=15&amp;q=CE-26&amp;pagina=2</summary>
        public async Task<ActionResult> Index(string tipo, int? aniId, int? aluId, string q, int pagina = 1, string aviso = null)
        {
            tipo = CatalogoDocumentos.Normalizar(tipo);
            if (aniId <= 0) aniId = null;
            if (aluId <= 0) aluId = null;
            q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();
            if (pagina < 1) pagina = 1;

            var anios = await AniosAsync();
            if (aniId.HasValue && !anios.Any(a => a.Ani_Id == aniId)) aniId = null;

            var todos = await _service.ListAsync(tipo, aluId, aniId);
            if (q != null)
            {
                todos = todos.Where(d =>
                        (d.Alu_Nombre ?? "").Contains(q, StringComparison.OrdinalIgnoreCase)
                        || (d.Alu_Identidad ?? "").Contains(q, StringComparison.OrdinalIgnoreCase)
                        || (d.Doc_Referencia ?? "").Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            todos = todos.OrderByDescending(d => d.Doc_FechaEmision).ThenByDescending(d => d.Doc_Id).ToList();

            var modelo = new ConstanciasIndexViewModel
            {
                Tipo = tipo,
                AniId = aniId,
                AluId = aluId,
                Buscar = q,
                Pagina = pagina,
                Aviso = aviso == "noencontrado" ? "Documento no encontrado. Puede que la referencia esté mal escrita." : null,
                Anios = anios,
                Total = todos.Count,
                PuedeEmitir = PuedeEmitir()
            };
            if (pagina > modelo.Paginas) return RedirectToAction("Index", new { tipo, aniId, aluId, q, pagina = 1 });
            modelo.Documentos = todos.Skip((pagina - 1) * ConstanciasIndexViewModel.PorPagina).Take(ConstanciasIndexViewModel.PorPagina).ToList();

            if (aluId.HasValue)
            {
                modelo.AlumnoFiltro = todos.FirstOrDefault()?.Alu_Nombre;
                if (string.IsNullOrWhiteSpace(modelo.AlumnoFiltro))
                    modelo.AlumnoFiltro = (await new AlumnosService().Find(aluId.Value))?.Alu_Nombre;
            }
            return View(modelo);
        }

        // ───────────────────────────── Emitir ─────────────────────────────

        /// <summary>/constancias/emitir?tipo=BOLETIN&amp;aluId=15&amp;aniId=2&amp;pacId=1 (todo opcional: llega precargado).</summary>
        public async Task<ActionResult> Emitir(string tipo, int? aluId, int? aniId, int? pacId)
        {
            if (!PuedeEmitir()) return RedirectToAction("SinAcceso", "Account");

            var anios = await AniosAsync();
            var modelo = new ConstanciaEmitirPaginaViewModel
            {
                Tipo = CatalogoDocumentos.Normalizar(tipo),
                AluId = aluId > 0 ? aluId : null,
                Alumnos = await new CasosDisciplinaService().AlumnosAsync(),
                Anios = anios,
                Parciales = (await new ParcialesService().ListAsync())?.OrderBy(p => p.Pac_Id).ToList() ?? new List<ParcialViewModel>(),
                PacId = pacId > 0 ? pacId : null,
                AniIdSugerido = anios.Any(a => a.Ani_Id == aniId)
                    ? aniId
                    : (anios.FirstOrDefault(a => a.Ani_EsActual) ?? anios.FirstOrDefault(a => a.Ani_EsActivo))?.Ani_Id
            };
            return View(modelo);
        }

        /// <summary>Valida la solicitud y la deja limpia; devuelve (campo, mensaje) si algo no sirve.</summary>
        private static (string campo, string mensaje)? Validar(ConstanciaSolicitudViewModel s)
        {
            if (s == null) return (null, "No llegaron datos.");
            s.Tipo = CatalogoDocumentos.Normalizar(s.Tipo);
            if (s.Tipo == null) return ("tipo", "Elija qué documento quiere emitir.");
            if (s.AluId <= 0) return ("alumno", "Elija el alumno.");

            var tipo = CatalogoDocumentos.Tipo(s.Tipo);
            if (!tipo.PideAnio) s.AniId = null;
            else if (!(s.AniId > 0)) return ("anio", "Elija el año lectivo.");
            if (!tipo.PideParcial || !(s.PacId > 0)) s.PacId = null;

            s.Motivo = string.IsNullOrWhiteSpace(s.Motivo) ? null : s.Motivo.Trim();
            if (s.Motivo != null && s.Motivo.Length > MaxMotivo)
                return ("motivo", $"El motivo admite hasta {MaxMotivo} caracteres.");
            return null;
        }

        /// <summary>
        /// Vista previa: el documento tal como quedaría (mismo partial que el emitido).
        /// Devuelve HTML; si no se puede emitir, JSON con el mensaje.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> VistaPreviaAsync(string tipo, int aluId, int? aniId, int? pacId)
        {
            if (!PuedeEmitir()) return Json(new { success = false, message = "No tiene permiso para emitir documentos." });

            var s = new ConstanciaSolicitudViewModel { Tipo = tipo, AluId = aluId, AniId = aniId, PacId = pacId };
            var error = Validar(s);
            if (error != null) return Json(new { success = false, field = error.Value.campo, message = error.Value.mensaje });

            DocumentoVistaPreviaViewModel vp = await _service.VistaPreviaAsync(s.Tipo, s.AluId, s.AniId, s.PacId);
            if (vp == null)
                return Json(new { success = false, message = "El servicio de documentos no respondió. Intente de nuevo en un momento." });
            if (vp.Codigo < 0)
                return Json(new { success = false, codigo = vp.Codigo, message = CatalogoDocumentos.Mensaje(vp.Codigo, s.Tipo) });

            var contenido = DocumentosEmitidosService.LeerContenido(vp.Doc_Contenido);
            if (contenido == null)
                return Json(new { success = false, message = "La vista previa llegó vacía. Intente de nuevo." });

            var impreso = ConMembrete(DocumentoImpreso.Desde(s.Tipo, contenido));
            impreso.EsVistaPrevia = true;
            impreso.Revision = Math.Max(1, vp.RevisionSiguiente);
            impreso.FechaEmision = HoraHonduras.Ahora;
            impreso.EmitidoPor = Session[SesionUsuario.Username] as string;
            return PartialView("_Documento", impreso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmitirAsync([FromBody] ConstanciaSolicitudViewModel model)
        {
            if (!PuedeEmitir()) return Json(new { success = false, message = "No tiene permiso para emitir documentos." });

            var error = Validar(model);
            if (error != null) return Json(new { success = false, field = error.Value.campo, message = error.Value.mensaje });

            int r = await _service.EmitirAsync(new DocumentoEmitirViewModel
            {
                Doc_Tipo = model.Tipo,
                Alu_Id = model.AluId,
                Ani_Id = model.AniId,
                Pac_Id = model.PacId,
                Doc_Motivo = model.Motivo,
                Doc_UsuarioEmite = GetUsuarioId()
            });

            if (r == 0)
                return Json(new { success = false, message = "El servicio de documentos no respondió. No se emitió nada; intente de nuevo." });
            if (r < 0)
                return Json(new { success = false, codigo = r, field = r == -4 ? "motivo" : null, message = CatalogoDocumentos.Mensaje(r, model.Tipo) });

            return Json(new
            {
                success = true,
                id = r,
                message = "Se emitió el documento.",
                redirect = Url.Action("Ver", new { id = r, aviso = "emitido" })
            });
        }

        // ───────────────────────────── El documento ─────────────────────────────

        /// <summary>/constancias/ver/5</summary>
        public async Task<ActionResult> Ver(int id, string aviso = null)
        {
            DocumentoEmitidoViewModel doc = await _service.FindAsync(id);
            if (doc == null || doc.Doc_Id == 0) return RedirectToAction("Index", new { aviso = "noencontrado" });

            var contenido = DocumentosEmitidosService.LeerContenido(doc.Doc_Contenido);
            var impreso = ConMembrete(DocumentoImpreso.Desde(doc.Doc_Tipo, contenido));
            impreso.Referencia = doc.Doc_Referencia;
            impreso.Revision = doc.Doc_Revision;
            impreso.Motivo = doc.Doc_Motivo;
            impreso.FechaEmision = doc.Doc_FechaEmision;
            impreso.EmitidoPor = doc.Doc_EmitidoPor;
            impreso.AnioNombre = doc.Ani_Nombre;
            if (string.IsNullOrWhiteSpace(impreso.Parcial)) impreso.Parcial = doc.Pac_Descripcion;
            if (string.IsNullOrWhiteSpace(impreso.Alumno.Nombre)) impreso.Alumno.Nombre = doc.Alu_Nombre;
            if (string.IsNullOrWhiteSpace(impreso.Alumno.Identidad)) impreso.Alumno.Identidad = doc.Alu_Identidad;

            // Todas las revisiones del mismo documento (mismo tipo, alumno, año y parcial).
            var revisiones = (await _service.ListAsync(doc.Doc_Tipo, doc.Alu_Id, doc.Ani_Id))
                .Where(d => d.Doc_Tipo == doc.Doc_Tipo && d.Alu_Id == doc.Alu_Id
                            && (d.Ani_Id ?? 0) == (doc.Ani_Id ?? 0) && (d.Pac_Id ?? 0) == (doc.Pac_Id ?? 0))
                .OrderByDescending(d => d.Doc_Revision)
                .ToList();
            if (!revisiones.Any(d => d.Doc_Id == doc.Doc_Id)) revisiones.Add(doc);

            var modelo = new ConstanciaVerViewModel
            {
                Documento = doc,
                Impreso = impreso,
                Revisiones = revisiones.OrderByDescending(d => d.Doc_Revision).ToList(),
                Ultima = revisiones.OrderByDescending(d => d.Doc_Revision).First(),
                PuedeEmitir = PuedeEmitir(),
                Aviso = TextoAviso(aviso),
                ContenidoIlegible = contenido == null
            };
            return View(modelo);
        }
    }
}

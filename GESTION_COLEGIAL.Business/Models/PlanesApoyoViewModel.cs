using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Una opción con código (el de la DB) y texto (el de pantalla).</summary>
    public class OpcionPlan
    {
        public string Codigo { get; set; }
        public string Etiqueta { get; set; }
        /// <summary>Solo en transiciones: el cambio pide motivo (pausar, cancelar, reabrir).</summary>
        public bool PideMotivo { get; set; }
        /// <summary>Solo en transiciones: texto del botón ("Activar", "Pausar"…).</summary>
        public string Accion { get; set; }
    }

    /// <summary>
    /// Categorías, estados y transiciones del plan de apoyo (espejo de Skuul SupportCategory y
    /// SupportPlanStatus; mismas transiciones que FN_PlanApoyo_Siguientes del script 072).
    /// </summary>
    public static class CatalogoPlanes
    {
        public static readonly IReadOnlyList<OpcionPlan> Categorias = new List<OpcionPlan>
        {
            new OpcionPlan { Codigo = "refuerzo",    Etiqueta = "Refuerzo académico" },
            new OpcionPlan { Codigo = "orientacion", Etiqueta = "Orientación" },
            new OpcionPlan { Codigo = "conducta",    Etiqueta = "Conducta" },
            new OpcionPlan { Codigo = "salud",       Etiqueta = "Salud" },
            new OpcionPlan { Codigo = "adecuacion",  Etiqueta = "Adecuación curricular" },
        };

        public static readonly IReadOnlyList<OpcionPlan> Estados = new List<OpcionPlan>
        {
            new OpcionPlan { Codigo = "borrador",   Etiqueta = "Borrador" },
            new OpcionPlan { Codigo = "activo",     Etiqueta = "Activo" },
            new OpcionPlan { Codigo = "en_pausa",   Etiqueta = "En pausa" },
            new OpcionPlan { Codigo = "completado", Etiqueta = "Completado" },
            new OpcionPlan { Codigo = "cancelado",  Etiqueta = "Cancelado" },
        };

        /// <summary>Categorías que se marcan confidenciales al elegirlas (como Skuul: salud y orientación).</summary>
        public static readonly string[] ConfidencialesPorDefecto = { "salud", "orientacion" };

        private static readonly Dictionary<string, OpcionPlan[]> Siguientes = new Dictionary<string, OpcionPlan[]>
        {
            ["borrador"] = new[]
            {
                new OpcionPlan { Codigo = "activo",    Accion = "Activar" },
                new OpcionPlan { Codigo = "cancelado", Accion = "Cancelar", PideMotivo = true },
            },
            ["activo"] = new[]
            {
                new OpcionPlan { Codigo = "en_pausa",   Accion = "Pausar", PideMotivo = true },
                new OpcionPlan { Codigo = "completado", Accion = "Completar" },
                new OpcionPlan { Codigo = "cancelado",  Accion = "Cancelar", PideMotivo = true },
            },
            ["en_pausa"] = new[]
            {
                new OpcionPlan { Codigo = "activo",     Accion = "Reanudar" },
                new OpcionPlan { Codigo = "completado", Accion = "Completar" },
                new OpcionPlan { Codigo = "cancelado",  Accion = "Cancelar", PideMotivo = true },
            },
            ["completado"] = new[]
            {
                new OpcionPlan { Codigo = "activo", Accion = "Reabrir", PideMotivo = true },
            },
            ["cancelado"] = new OpcionPlan[0],
        };

        private static string Etiqueta(IReadOnlyList<OpcionPlan> lista, string codigo) =>
            lista.FirstOrDefault(x => x.Codigo == codigo)?.Etiqueta ?? codigo ?? "";

        public static string Categoria(string codigo) => Etiqueta(Categorias, codigo);
        public static string Estado(string codigo) => Etiqueta(Estados, codigo);

        public static bool ExisteCategoria(string codigo) => Categorias.Any(x => x.Codigo == codigo);
        public static bool ExisteEstado(string codigo) => Estados.Any(x => x.Codigo == codigo);

        /// <summary>¿El plan todavía acepta trabajo? (completado y cancelado, no)</summary>
        public static bool EstaAbierto(string estado) => estado != "completado" && estado != "cancelado";

        public static List<OpcionPlan> SiguientesDe(string estado) =>
            (Siguientes.TryGetValue(estado ?? "", out OpcionPlan[] s) ? s : new OpcionPlan[0])
                .Select(x => new OpcionPlan { Codigo = x.Codigo, Etiqueta = Estado(x.Codigo), Accion = x.Accion, PideMotivo = x.PideMotivo })
                .ToList();

        public static OpcionPlan Transicion(string desde, string hacia) =>
            SiguientesDe(desde).FirstOrDefault(x => x.Codigo == hacia);

        /// <summary>Insignia CORK del estado.</summary>
        public static string InsigniaEstado(string estado) => estado switch
        {
            "borrador" => "badge-secondary",
            "activo" => "badge-success",
            "en_pausa" => "badge-warning",
            "completado" => "badge-info",
            "cancelado" => "badge-dark",
            _ => "badge-secondary"
        };

        /// <summary>Insignia suave (CORK outline-badge) de la categoría.</summary>
        public static string InsigniaCategoria(string categoria) => categoria switch
        {
            "refuerzo" => "outline-badge-primary",
            "orientacion" => "outline-badge-info",
            "conducta" => "outline-badge-warning",
            "salud" => "outline-badge-danger",
            "adecuacion" => "outline-badge-success",
            _ => "outline-badge-secondary"
        };

        /// <summary>Ícono (Bootstrap Icons) de cada transición.</summary>
        public static string IconoAccion(string hacia, string desde) => hacia switch
        {
            "activo" when desde == "completado" => "bi-arrow-counterclockwise",
            "activo" when desde == "en_pausa" => "bi-play-fill",
            "activo" => "bi-play-fill",
            "en_pausa" => "bi-pause-fill",
            "completado" => "bi-check2-circle",
            "cancelado" => "bi-x-circle",
            _ => "bi-arrow-right"
        };
    }

    public class PlanApoyoListaViewModel
    {
        public int Apo_Id { get; set; }
        public int Alu_Id { get; set; }
        public string Alumno { get; set; }
        public string Identidad { get; set; }
        public string Foto { get; set; }
        public int? Niv_Id { get; set; }
        public string Nivel { get; set; }
        public int? Cun_Id { get; set; }
        public string Grado { get; set; }
        public int? Sec_Id { get; set; }
        public string Seccion { get; set; }
        public string Apo_Categoria { get; set; }
        public string Apo_Titulo { get; set; }
        public string Apo_Estado { get; set; }
        public bool Apo_EsConfidencial { get; set; }
        public DateTime Apo_FechaInicio { get; set; }
        public DateTime? Apo_FechaRevision { get; set; }
        public DateTime? Apo_FechaFin { get; set; }
        public int? Usu_IdResponsable { get; set; }
        public string Responsable { get; set; }
        public int PasosTotal { get; set; }
        public int PasosHechos { get; set; }
        public int PasosAtrasados { get; set; }
        public int MisPasosPendientes { get; set; }
        public bool RevisionVencida { get; set; }
        public bool EsResponsable { get; set; }
        public bool TienePasos { get; set; }
        public DateTime Apo_FechaRegistra { get; set; }
    }

    public class PlanApoyoResumenViewModel
    {
        public int Activos { get; set; }
        public int Abiertos { get; set; }
        public int RevisionVencida { get; set; }
        public int PasosAtrasados { get; set; }
        public int CompletadosAnio { get; set; }
        public string AnioNombre { get; set; }
    }

    public class PlanApoyoViewModel
    {
        public int Apo_Id { get; set; }
        public int Alu_Id { get; set; }
        public string Alumno { get; set; }
        public string Identidad { get; set; }
        public string Foto { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public string Curso { get; set; }
        public int? Ani_Id { get; set; }
        public string Ani_Nombre { get; set; }
        public string Apo_Categoria { get; set; }
        public string Apo_Titulo { get; set; }
        public string Apo_Resumen { get; set; }
        public string Apo_Estado { get; set; }
        public bool Apo_EsConfidencial { get; set; }
        public DateTime Apo_FechaInicio { get; set; }
        public DateTime? Apo_FechaRevision { get; set; }
        public DateTime? Apo_FechaFin { get; set; }
        public int Apo_Version { get; set; }
        public int? Usu_IdResponsable { get; set; }
        public string Responsable { get; set; }
        public int? Apo_UsuarioRegistra { get; set; }
        public string Creador { get; set; }
        public DateTime Apo_FechaRegistra { get; set; }
        public DateTime? Apo_FechaModifica { get; set; }
        public int PasosTotal { get; set; }
        public int PasosHechos { get; set; }
        public bool RevisionVencida { get; set; }
        public bool EsResponsable { get; set; }
        public bool EsCreador { get; set; }

        public bool EstaAbierto => CatalogoPlanes.EstaAbierto(Apo_Estado);
        public string Ubicacion => string.Join(" · ", new[] { Grado, Seccion }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public class PlanApoyoPasoViewModel
    {
        public int Apa_Id { get; set; }
        public string Apa_Descripcion { get; set; }
        public DateTime? Apa_FechaLimite { get; set; }
        public DateTime? Apa_FechaHecha { get; set; }
        public int? Usu_IdResponsable { get; set; }
        public string Responsable { get; set; }
        public string HechaPor { get; set; }
        public bool Atrasado { get; set; }
        public bool EsMio { get; set; }

        public bool Hecho => Apa_FechaHecha.HasValue;
    }

    public class PlanApoyoNotaViewModel
    {
        public int Apn_Id { get; set; }
        public string Apn_Texto { get; set; }
        public DateTime Apn_Fecha { get; set; }
        public int? Apn_Usuario { get; set; }
        public string Usuario { get; set; }
    }

    public class PlanApoyoEstadoViewModel
    {
        public int Ape_Id { get; set; }
        public string Ape_De { get; set; }
        public string Ape_A { get; set; }
        public string Ape_Motivo { get; set; }
        public DateTime Ape_Fecha { get; set; }
        public string Usuario { get; set; }
    }

    public class PlanApoyoUsuarioViewModel
    {
        public int Usu_Id { get; set; }
        public string Usuario { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }

    public class PlanApoyoAlumnoViewModel
    {
        public int Alu_Id { get; set; }
        public string Nombre { get; set; }
        public string Identidad { get; set; }
        public string Nivel { get; set; }
        public string Ubicacion { get; set; }
    }

    public class PlanApoyoSeccionViewModel
    {
        public int Sec_Id { get; set; }
        public string Sec_Descripcion { get; set; }
    }

    public class PlanApoyoAbiertoViewModel
    {
        public int Alu_Id { get; set; }
        public int Apo_Id { get; set; }
        public string Apo_Titulo { get; set; }
        public string Apo_Estado { get; set; }
        public string Apo_Categoria { get; set; }
        public DateTime? Apo_FechaRevision { get; set; }
    }

    // ───────────────────────────── Escrituras (lo que se manda a la API) ─────────────────────────────

    /// <summary>Quién escribe: lo llena el controller con la sesión, nunca el navegador.</summary>
    public abstract class PlanApoyoEscritorViewModel
    {
        public int? Usuario { get; set; }
        public bool PuedeGestionar { get; set; }
        public bool VerConfidenciales { get; set; }
    }

    public class PlanApoyoGuardarViewModel : PlanApoyoEscritorViewModel
    {
        public int Apo_Id { get; set; }
        public int Alu_Id { get; set; }
        public string Apo_Categoria { get; set; }
        public string Apo_Titulo { get; set; }
        public string Apo_Resumen { get; set; }
        public DateTime? Apo_FechaInicio { get; set; }
        public DateTime? Apo_FechaRevision { get; set; }
        public int? Usu_IdResponsable { get; set; }
        public bool Apo_EsConfidencial { get; set; }
        public bool Activar { get; set; }
        public int? Apo_Version { get; set; }
    }

    public class PlanApoyoCambiarEstadoViewModel : PlanApoyoEscritorViewModel
    {
        public int Apo_Id { get; set; }
        public string Estado { get; set; }
        public string Motivo { get; set; }
    }

    public class PlanApoyoAgregarPasoViewModel : PlanApoyoEscritorViewModel
    {
        public int Apo_Id { get; set; }
        public string Apa_Descripcion { get; set; }
        public DateTime? Apa_FechaLimite { get; set; }
        public int? Usu_IdResponsable { get; set; }
    }

    public class PlanApoyoPasoAccionViewModel : PlanApoyoEscritorViewModel
    {
        public int Apo_Id { get; set; }
        public int Apa_Id { get; set; }
    }

    public class PlanApoyoAgregarNotaViewModel : PlanApoyoEscritorViewModel
    {
        public int Apo_Id { get; set; }
        public string Apn_Texto { get; set; }
    }

    public class PlanApoyoEliminarViewModel : PlanApoyoEscritorViewModel
    {
        public int Apo_Id { get; set; }
    }

    public class PlanApoyoResultadoViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public string Estado { get; set; }
        public int Version { get; set; }
        public string Mensaje { get; set; }
    }
}

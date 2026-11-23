using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Temario y avance del programa (API Temarios): biblioteca, temas, secciones con su avance, marcas,
    /// resumen de Dirección, materias sin temario, opciones de los filtros y las escrituras. Cada pedido
    /// lleva el usuario de la sesión: la API decide con él qué puede ver y hacer.
    /// </summary>
    public class TemariosService
    {
        private const string Base = "Temarios/";

        private static TemarioResultadoViewModel Fallo(string mensaje) => new TemarioResultadoViewModel { Ok = false, Mensaje = mensaje };

        private static async Task<List<T>> Lista<T>(string url)
        {
            try { return (await ApiRequests.ListAsync<T>(url))?.ToList() ?? new List<T>(); }
            catch { return new List<T>(); }
        }

        /// <summary>"&amp;nombre=valor" solo si hay valor.</summary>
        private static string Q(string nombre, int? v) => v.HasValue ? "&" + nombre + "=" + v.Value : "";

        public Task<List<TemarioViewModel>> ListAsync(int usuId) => Lista<TemarioViewModel>($"{Base}ListAsync?usuId={usuId}");

        /// <summary>null si no existe o la persona no lo puede leer.</summary>
        public async Task<TemarioViewModel> FindAsync(int id, int usuId)
        {
            try
            {
                var r = await ApiRequests.GetSingleAsync<TemarioViewModel>($"{Base}FindAsync?value={id}&usuId={usuId}");
                return r == null || r.Tem_Id == 0 ? null : r;
            }
            catch { return null; }
        }

        public Task<List<TemarioTemaViewModel>> TemasAsync(int id, int usuId) =>
            Lista<TemarioTemaViewModel>($"{Base}TemasAsync?value={id}&usuId={usuId}");

        public Task<List<TemarioGrupoViewModel>> GruposAsync(int id, int usuId) =>
            Lista<TemarioGrupoViewModel>($"{Base}GruposAsync?value={id}&usuId={usuId}");

        public Task<List<TemarioAvanceViewModel>> AvanceAsync(int id, int usuId) =>
            Lista<TemarioAvanceViewModel>($"{Base}AvanceAsync?value={id}&usuId={usuId}");

        public Task<List<TemarioResumenViewModel>> ResumenAsync(int usuId, int? anio, int? semId) =>
            Lista<TemarioResumenViewModel>($"{Base}ResumenAsync?usuId={usuId}{Q("anio", anio)}{Q("semId", semId)}");

        public Task<List<TemarioSinTemarioViewModel>> SinTemarioAsync(int usuId, int? anio, int? semId) =>
            Lista<TemarioSinTemarioViewModel>($"{Base}SinTemarioAsync?usuId={usuId}{Q("anio", anio)}{Q("semId", semId)}");

        /// <summary>tipo = nivel | grado | curso | materia | semestre | anio.</summary>
        public Task<List<TemarioOpcionViewModel>> OpcionesAsync(int usuId, string tipo, int? nivId = null, int? cunId = null, int? curId = null, int? anio = null) =>
            Lista<TemarioOpcionViewModel>($"{Base}OpcionesAsync?usuId={usuId}&tipo={Uri.EscapeDataString(tipo ?? "")}{Q("nivId", nivId)}{Q("cunId", cunId)}{Q("curId", curId)}{Q("anio", anio)}");

        public Task<List<TemarioTemaHorarioViewModel>> TemasDeHorarioAsync(int horId, int semana, int usuId) =>
            Lista<TemarioTemaHorarioViewModel>($"{Base}TemasDeHorarioAsync?horId={horId}&semana={semana}&usuId={usuId}");

        private static async Task<TemarioResultadoViewModel> Post(string accion, object model, string general)
        {
            try { return await ApiRequests.PostAsyncWithResponse<TemarioResultadoViewModel>(Base + accion, model) ?? Fallo(general); }
            catch { return Fallo("No se pudo conectar con el servidor. Intente de nuevo."); }
        }

        public Task<TemarioResultadoViewModel> CrearAsync(TemarioGuardarViewModel m) => Post("CrearAsync", m, "No se pudo crear el temario.");
        public Task<TemarioResultadoViewModel> EditarAsync(TemarioGuardarViewModel m) => Post("EditarAsync", m, "No se pudieron guardar los cambios del temario.");
        public Task<TemarioResultadoViewModel> EliminarAsync(TemarioEliminarViewModel m) => Post("EliminarAsync", m, "No se pudo quitar el temario.");
        public Task<TemarioResultadoViewModel> CopiarAsync(TemarioCopiarViewModel m) => Post("CopiarAsync", m, "No se pudo copiar el temario.");
        public Task<TemarioResultadoViewModel> GuardarTemaAsync(TemarioTemaGuardarViewModel m) => Post("GuardarTemaAsync", m, "No se pudo guardar el tema.");
        public Task<TemarioResultadoViewModel> MoverTemaAsync(TemarioTemaAccionViewModel m) => Post("MoverTemaAsync", m, "No se pudo mover el tema.");
        public Task<TemarioResultadoViewModel> QuitarTemaAsync(TemarioTemaAccionViewModel m) => Post("QuitarTemaAsync", m, "No se pudo quitar el tema.");
        public Task<TemarioResultadoViewModel> MarcarAvanceAsync(TemarioAvanceGuardarViewModel m) => Post("MarcarAvanceAsync", m, "No se pudo guardar el avance.");
        public Task<TemarioResultadoViewModel> QuitarAvanceAsync(TemarioAvanceGuardarViewModel m) => Post("QuitarAvanceAsync", m, "No se pudo quitar la marca de avance.");
    }
}

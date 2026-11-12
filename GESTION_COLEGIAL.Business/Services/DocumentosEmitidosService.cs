using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Constancias y expediente (API DocumentosEmitidos): listar, abrir, vista previa y emitir.
    /// No hay editar ni borrar: un documento emitido no cambia.
    /// </summary>
    public class DocumentosEmitidosService
    {
        private const string Base = "DocumentosEmitidos/";

        private static string Parametro(string nombre, int? valor) =>
            valor.HasValue && valor.Value > 0 ? $"&{nombre}={valor.Value}" : "";

        public async Task<List<DocumentoEmitidoViewModel>> ListAsync(string tipo, int? aluId, int? aniId)
        {
            string url = Base + "ListAsync?tipo=" + Uri.EscapeDataString(tipo ?? "")
                + Parametro("aluId", aluId) + Parametro("aniId", aniId);
            return (await ApiRequests.ListAsync<DocumentoEmitidoViewModel>(url))?.ToList() ?? new List<DocumentoEmitidoViewModel>();
        }

        /// <summary>null si no existe.</summary>
        public async Task<DocumentoEmitidoViewModel> FindAsync(int id) =>
            await ApiRequests.GetSingleAsync<DocumentoEmitidoViewModel>($"{Base}FindAsync?value={id}");

        /// <summary>null si la API no respondió.</summary>
        public async Task<DocumentoVistaPreviaViewModel> VistaPreviaAsync(string tipo, int aluId, int? aniId, int? pacId) =>
            await ApiRequests.GetSingleAsync<DocumentoVistaPreviaViewModel>(
                $"{Base}VistaPreviaAsync?tipo={Uri.EscapeDataString(tipo ?? "")}&aluId={aluId}"
                + Parametro("aniId", aniId) + Parametro("pacId", pacId));

        /// <summary>Id del documento (&gt;0), un código negativo, o 0 si la API no respondió.</summary>
        public async Task<int> EmitirAsync(DocumentoEmitirViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<int>(Base + "EmitirAsync", model);

        /// <summary>Lee el contenido congelado; null si viene vacío o roto.</summary>
        public static DocumentoContenido LeerContenido(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                var c = JsonConvert.DeserializeObject<DocumentoContenido>(json);
                if (c != null && c.Notas == null) c.Notas = new List<DocumentoContenidoNota>();
                return c;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}

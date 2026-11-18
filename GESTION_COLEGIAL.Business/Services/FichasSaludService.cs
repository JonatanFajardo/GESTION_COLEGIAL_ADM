using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Ficha de salud del alumno (API FichasSalud): directorio, ficha, a quién llamar,
    /// historial de cambios, guardar y alertas críticas para el pase de lista.
    /// </summary>
    public class FichasSaludService
    {
        private const string Base = "FichasSalud/";

        private static FichaSaludResultadoViewModel Fallo(string mensaje) => new FichaSaludResultadoViewModel { Ok = false, Mensaje = mensaje };

        /// <summary>Todos los alumnos activos con el estado de su ficha (los filtros se aplican en la pantalla).</summary>
        public async Task<List<FichaSaludDirectorioViewModel>> DirectorioAsync(int? nivId = null, int? cunId = null, int? secId = null,
                                                                               string texto = null, bool soloSinFicha = false, bool soloConAlerta = false)
        {
            string url = $"{Base}DirectorioAsync?soloSinFicha={(soloSinFicha ? "true" : "false")}&soloConAlerta={(soloConAlerta ? "true" : "false")}"
                + (nivId.HasValue ? "&nivId=" + nivId.Value : "")
                + (cunId.HasValue ? "&cunId=" + cunId.Value : "")
                + (secId.HasValue ? "&secId=" + secId.Value : "")
                + (string.IsNullOrWhiteSpace(texto) ? "" : "&texto=" + Uri.EscapeDataString(texto.Trim()));
            var r = await ApiRequests.ListAsync<FichaSaludDirectorioViewModel>(url);
            return r?.ToList() ?? new List<FichaSaludDirectorioViewModel>();
        }

        /// <summary>El alumno y su ficha; null si el alumno no existe.</summary>
        public async Task<FichaSaludViewModel> FindAsync(int aluId) =>
            await ApiRequests.GetSingleAsync<FichaSaludViewModel>($"{Base}FindAsync?value={aluId}");

        public async Task<List<FichaSaludContactoViewModel>> ContactosAsync(int aluId) =>
            (await ApiRequests.ListAsync<FichaSaludContactoViewModel>($"{Base}ContactosAsync?value={aluId}"))?.ToList()
            ?? new List<FichaSaludContactoViewModel>();

        public async Task<List<FichaSaludBitacoraViewModel>> BitacoraAsync(int aluId) =>
            (await ApiRequests.ListAsync<FichaSaludBitacoraViewModel>($"{Base}BitacoraAsync?value={aluId}"))?.ToList()
            ?? new List<FichaSaludBitacoraViewModel>();

        /// <summary>Alertas críticas de los alumnos del pase de lista de un horario (fecha yyyy-MM-dd).</summary>
        public async Task<List<FichaSaludAlertaViewModel>> AlertasSeccionAsync(int horId, string fecha) =>
            (await ApiRequests.ListAsync<FichaSaludAlertaViewModel>(
                $"{Base}AlertasSeccionAsync?horId={horId}" + (string.IsNullOrWhiteSpace(fecha) ? "" : "&fecha=" + Uri.EscapeDataString(fecha))))?.ToList()
            ?? new List<FichaSaludAlertaViewModel>();

        /// <summary>La ficha y a quién llamar, juntos (bloque de emergencia).</summary>
        public async Task<FichaSaludEmergenciaViewModel> EmergenciaAsync(int aluId)
        {
            var fichaTask = FindAsync(aluId);
            var contactosTask = ContactosAsync(aluId);
            await Task.WhenAll(fichaTask, contactosTask);
            return fichaTask.Result == null
                ? null
                : new FichaSaludEmergenciaViewModel { Ficha = fichaTask.Result, Contactos = contactosTask.Result };
        }

        public async Task<FichaSaludResultadoViewModel> GuardarAsync(FichaSaludGuardarViewModel model) =>
            await ApiRequests.PostAsyncWithResponse<FichaSaludResultadoViewModel>(Base + "GuardarAsync", model)
            ?? Fallo("No se pudo guardar la ficha de salud.");
    }
}

using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services.ModuloFinanzas
{
    /// <summary>
    /// Saldo a favor de los alumnos y registro de correcciones de su cuenta.
    /// </summary>
    public class SaldoFavorService
    {
        /// <summary>Saldo a favor actual del alumno.</summary>
        public async Task<SaldoFavorViewModel> Obtener(int alumnoId)
        {
            return await ApiRequests.GetSingleAsync<SaldoFavorViewModel>($"SaldoFavor/ObtenerAsync?alumnoId={alumnoId}");
        }

        /// <summary>Devuelve dinero del saldo a favor al encargado.</summary>
        public async Task<RespuestaApi> Devolver(DevolverSaldoViewModel model)
        {
            return await ApiRequests.PostConMensajeAsync("SaldoFavor/DevolverAsync", model);
        }

        /// <summary>Aplica el saldo a favor a los cargos pendientes, del más viejo al más nuevo.</summary>
        public async Task<RespuestaApi> Aplicar(AplicarSaldoViewModel model)
        {
            return await ApiRequests.PostConMensajeAsync("SaldoFavor/AplicarAsync", model);
        }

        /// <summary>Correcciones hechas a la cuenta del alumno (anulaciones, perdones, devoluciones…).</summary>
        public async Task<IEnumerable<CorreccionCuentaViewModel>> Correcciones(int alumnoId)
        {
            return await ApiRequests.ListAsync<CorreccionCuentaViewModel>($"Correcciones/ListByAlumnoAsync?alumnoId={alumnoId}");
        }
    }
}

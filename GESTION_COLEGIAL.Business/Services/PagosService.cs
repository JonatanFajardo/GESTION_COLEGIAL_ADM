using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los pagos.
    /// </summary>
    public class PagosService
    {
        /// <summary>
        /// Obtiene una lista de pagos de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos PagoListViewModel.</returns>
        public async Task<IEnumerable<PagoListViewModel>> ListAsync()
        {
            string url = "Pagos/ListAsync";
            IEnumerable<PagoListViewModel> apiUrl = await ApiRequests.ListAsync<PagoListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene una lista de pagos de un alumno específico de forma asíncrona.
        /// </summary>
        /// <param name="alumnoId">El identificador del alumno.</param>
        /// <returns>Una colección de objetos PagoListViewModel.</returns>
        public async Task<IEnumerable<PagoListViewModel>> ListByAlumnoAsync(int alumnoId)
        {
            string url = $"Pagos/ListByAlumnoAsync?alumnoId={alumnoId}";
            IEnumerable<PagoListViewModel> apiUrl = await ApiRequests.ListAsync<PagoListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene una lista de pagos de una fecha específica de forma asíncrona.
        /// </summary>
        /// <param name="fecha">La fecha para filtrar los pagos.</param>
        /// <returns>Una colección de objetos PagoListViewModel.</returns>
        public async Task<IEnumerable<PagoListViewModel>> ListByFechaAsync(DateTime fecha)
        {
            string url = $"Pagos/ListByFechaAsync?fecha={fecha:yyyy-MM-dd}";
            IEnumerable<PagoListViewModel> apiUrl = await ApiRequests.ListAsync<PagoListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del pago.</param>
        /// <returns>El objeto PagoDetailViewModel encontrado.</returns>
        public async Task<PagoDetailViewModel> Find(int id)
        {
            string url = "Pagos/FindAsync";
            PagoDetailViewModel apiUrl = await ApiRequests.FindAsync<PagoDetailViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo pago de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto PagoViewModel a crear.</param>
        /// <returns>True si el pago se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(PagoViewModel model)
        {
            string url = "Pagos/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un pago existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto PagoViewModel a editar.</param>
        /// <returns>True si el pago se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(PagoViewModel model)
        {
            string url = "Pagos/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Elimina un pago por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del pago a eliminar.</param>
        /// <returns>True si el pago se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Pagos/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        /// <summary>
        /// Obtiene el recibo de un pago específico de forma asíncrona.
        /// </summary>
        /// <param name="pagoId">El identificador del pago.</param>
        /// <returns>El objeto con información del recibo.</returns>
        public async Task<PagoDetailViewModel> GetReciboAsync(int pagoId)
        {
            string url = $"Pagos/GetReciboAsync?pagoId={pagoId}";
            PagoDetailViewModel resultado = await ApiRequests.GetSingleAsync<PagoDetailViewModel>(url);
            return resultado;
        }
    }
}
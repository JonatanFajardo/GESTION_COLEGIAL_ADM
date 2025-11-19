using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con las cuentas por cobrar.
    /// </summary>
    public class CuentasCobrarService
    {
        /// <summary>
        /// Obtiene una lista de cuentas por cobrar de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos CuentaCobrarListViewModel.</returns>
        public async Task<IEnumerable<CuentaCobrarListViewModel>> ListAsync()
        {
            string url = "CuentasCobrar/ListAsync";
            IEnumerable<CuentaCobrarListViewModel> apiUrl = await ApiRequests.ListAsync<CuentaCobrarListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene cuentas por cobrar de un alumno específico.
        /// </summary>
        /// <param name="alumnoId">El identificador del alumno.</param>
        /// <returns>Una colección de objetos CuentaCobrarListViewModel.</returns>
        public async Task<IEnumerable<CuentaCobrarListViewModel>> ListByAlumnoAsync(int alumnoId)
        {
            string url = $"CuentasCobrar/ListByAlumnoAsync?alumnoId={alumnoId}";
            IEnumerable<CuentaCobrarListViewModel> apiUrl = await ApiRequests.ListAsync<CuentaCobrarListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene cuentas por cobrar pendientes (no vencidas).
        /// </summary>
        /// <returns>Una colección de objetos CuentaCobrarListViewModel.</returns>
        public async Task<IEnumerable<CuentaCobrarListViewModel>> ListPendientesAsync()
        {
            string url = "CuentasCobrar/ListAsync";
            IEnumerable<CuentaCobrarListViewModel> cuentas = await ApiRequests.ListAsync<CuentaCobrarListViewModel>(url);

            // Filtrar cargos no vencidos (fecha de vencimiento >= hoy) con monto pendiente
            var pendientes = cuentas
                .Where(c => c.EstadoPago == "Pendiente")
                .OrderBy(c => c.FechaVence)
                .ToList();

            return pendientes;
        }

        /// <summary>
        /// Obtiene cuentas por cobrar vencidas.
        /// </summary>
        /// <returns>Una colección de objetos CuentaCobrarListViewModel.</returns>
        public async Task<IEnumerable<CuentaCobrarListViewModel>> ListVencidasAsync()
        {
            string url = "CuentasCobrar/ListVencidasAsync";
            IEnumerable<CuentaCobrarListViewModel> apiUrl = await ApiRequests.ListAsync<CuentaCobrarListViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Obtiene lista de deudores (cuentas vencidas no pagadas).
        /// </summary>
        /// <returns>Una colección de objetos CuentaCobrarListViewModel filtrados por vencimiento.</returns>
        public async Task<IEnumerable<CuentaCobrarListViewModel>> ListDeudoresAsync()
        {
            string url = "CuentasCobrar/ListAsync";
            IEnumerable<CuentaCobrarListViewModel> cuentas = await ApiRequests.ListAsync<CuentaCobrarListViewModel>(url);

            // Filtrar cuentas vencidas (fecha de vencimiento menor a hoy) con monto pendiente
            var deudores = cuentas
                .Where(c => c.FechaVence < DateTime.Now && c.Pendiente > 0)
                .OrderByDescending(c => c.Pendiente)
                .ToList();

            return deudores;
        }

        /// <summary>
        /// Busca una cuenta por cobrar por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la cuenta por cobrar.</param>
        /// <returns>El objeto CuentaCobrarFindViewModel encontrado.</returns>
        public async Task<CuentaCobrarFindViewModel> Find(int id)
        {
            string url = "CuentasCobrar/FindAsync";
            CuentaCobrarFindViewModel apiUrl = await ApiRequests.FindAsync<CuentaCobrarFindViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea una nueva cuenta por cobrar de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto CuentaCobrarFindViewModel a crear.</param>
        /// <returns>True si la cuenta por cobrar se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(CuentaCobrarFindViewModel model)
        {
            string url = "CuentasCobrar/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita una cuenta por cobrar existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto CuentaCobrarFindViewModel a editar.</param>
        /// <returns>True si la cuenta por cobrar se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(CuentaCobrarFindViewModel model)
        {
            string url = "CuentasCobrar/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Elimina una cuenta por cobrar por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador de la cuenta por cobrar a eliminar.</param>
        /// <returns>True si la cuenta por cobrar se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "CuentasCobrar/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        /// <summary>
        /// Genera cargos automáticos para un alumno en un año específico.
        /// </summary>
        /// <param name="alumnoId">El identificador del alumno.</param>
        /// <param name="anio">El año para el cual generar los cargos.</param>
        /// <returns>True si hubo error, False si fue exitoso.</returns>
        public async Task<Boolean> GenerarCargosAlumno(int alumnoId, int anio)
        {
            string url = $"CuentasCobrar/GenerarCargosAlumnoAsync?alumnoId={alumnoId}&anio={anio}";
            return await ApiRequests.CreateAsync(url, new { alumnoId, anio });
        }

        /// <summary>
        /// Aplica un descuento a una cuenta por cobrar.
        /// </summary>
        /// <param name="cuentaCobrarId">El identificador de la cuenta por cobrar.</param>
        /// <param name="descuentoId">El identificador del descuento a aplicar.</param>
        /// <param name="monto">El monto del descuento.</param>
        /// <param name="justificacion">La justificación del descuento.</param>
        /// <returns>True si hubo error, False si fue exitoso.</returns>
        public async Task<Boolean> AplicarDescuento(int cuentaCobrarId, int descuentoId, decimal monto, string justificacion)
        {
            string url = "CuentasCobrar/AplicarDescuentoAsync";
            var model = new { cuentaCobrarId, descuentoId, monto, justificacion };
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Genera mensualidades para un mes específico.
        /// </summary>
        /// <param name="mes">El mes (1-12)</param>
        /// <param name="anio">El año</param>
        /// <returns>Resultado de la generación con totales</returns>
        public async Task<dynamic> GenerarMensualidad(int mes, int anio)
        {
            string url = $"CuentasCobrar/GenerarMensualidadAsync?mes={mes}&anio={anio}";
            return await ApiRequests.PostAsyncWithResponse<dynamic>(url, new { mes, anio });
        }

        /// <summary>
        /// Genera mensualidades para un rango de meses.
        /// </summary>
        /// <param name="mesInicio">Mes inicial del rango</param>
        /// <param name="mesFin">Mes final del rango</param>
        /// <param name="anio">El año</param>
        /// <returns>Resultado de la generación con totales</returns>
        public async Task<dynamic> GenerarMensualidadesRango(int mesInicio, int mesFin, int anio)
        {
            string url = $"CuentasCobrar/GenerarMensualidadesRangoAsync?mesInicio={mesInicio}&mesFin={mesFin}&anio={anio}";
            return await ApiRequests.PostAsyncWithResponse<dynamic>(url, new { mesInicio, mesFin, anio });
        }

        /// <summary>
        /// Obtiene los meses pendientes de pago de un alumno.
        /// </summary>
        /// <param name="alumnoId">Identificador del alumno</param>
        /// <param name="anio">Año a consultar (opcional, por defecto año actual)</param>
        /// <returns>Lista de meses con su estado de pago</returns>
        public async Task<IEnumerable<dynamic>> MesesPendientesPorAlumno(int alumnoId, int? anio = null)
        {
            string url = $"CuentasCobrar/MesesPendientesPorAlumnoAsync?alumnoId={alumnoId}";
            if (anio.HasValue)
                url += $"&anio={anio.Value}";

            return await ApiRequests.ListAsync<dynamic>(url);
        }

        /// <summary>
        /// Obtiene el estado de cuenta completo de un alumno.
        /// </summary>
        /// <param name="alumnoId">Identificador del alumno</param>
        /// <returns>Estado de cuenta con información del alumno, resumen y detalle</returns>
        public async Task<dynamic> EstadoCuentaAlumnoAsync(int alumnoId)
        {
            string url = $"CuentasCobrar/EstadoCuentaAlumnoAsync?alumnoId={alumnoId}";
            return await ApiRequests.FindAsync<dynamic>(url, 0);
        }
    }
}

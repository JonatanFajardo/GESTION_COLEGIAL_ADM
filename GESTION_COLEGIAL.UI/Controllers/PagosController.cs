using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de pagos.
    /// </summary>
    public class PagosController : BaseController
    {
        private readonly PagosService pagosService = new PagosService();

        /// <summary>
        /// Acción para mostrar la vista principal de pagos.
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de registro de pago.
        /// </summary>
        public ActionResult NuevoPago()
        {
            return View();
        }

        /// <summary>
        /// Acción para mostrar la vista de pagos del día.
        /// </summary>
        public ActionResult ListaDia()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de pagos.
        /// </summary>
        public async Task<ActionResult> ListAsync()
        {
            var result = await pagosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener pagos de un alumno.
        /// </summary>
        public async Task<ActionResult> ListByAlumnoAsync(int alumnoId)
        {
            var result = await pagosService.ListByAlumnoAsync(alumnoId);
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener pagos del día.
        /// </summary>
        public async Task<ActionResult> ListByFechaAsync(DateTime fecha)
        {
            var result = await pagosService.ListByFechaAsync(fecha);
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para crear un pago.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(PagoViewModel model)
        {
            bool result = await pagosService.Create(model);
            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessInsert);
        }

        /// <summary>
        /// Acción asincrónica para obtener el recibo de un pago.
        /// </summary>
        public async Task<ActionResult> GetReciboAsync(int pagoId)
        {
            var result = await pagosService.GetReciboAsync(pagoId);
            return AjaxResult(result, true);
        }

        /// <summary>
        /// Acción para mostrar la vista del recibo de pago en formato imprimible.
        /// </summary>
        public async Task<ActionResult> Recibo(int pagoId)
        {
            try
            {
                // Obtener información completa del recibo desde el endpoint específico
                var reciboInfo = await pagosService.GetReciboAsync(pagoId);

                if (reciboInfo == null)
                {
                    return HttpNotFound("No se encontró el pago especificado.");
                }

                // Pasar directamente el PagoDetailViewModel que ya tiene toda la información
                return View(reciboInfo);
            }
            catch (Exception ex)
            {
                return Content("Error al cargar el recibo: " + ex.Message);
            }
        }
    }
}

using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de horarios de alumnos.
    /// </summary>
    public class HorarioAlumnosController : BaseController
    {
        private readonly HorarioAlumnosService horarioAlumnosService = new HorarioAlumnosService();
        private readonly CursosService cursosService = new CursosService();
        private readonly CursosNivelesService cursosNivelesService = new CursosNivelesService();
        private readonly MateriasService materiasService = new MateriasService();
        private readonly DiasService diasService = new DiasService();
        private readonly HorasService horasService = new HorasService();

        /// <summary>
        /// Acción para mostrar la vista principal de los horarios de alumnos.
        /// </summary>
        /// <returns>Vista principal de los horarios.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Acción asincrónica para obtener la lista de horarios.
        /// </summary>
        /// <returns>Resultado de la operación con la lista de horarios.</returns>
        public async Task<ActionResult> ListAsync()
        {
            var result = await horarioAlumnosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción asincrónica para obtener los detalles de un horario específico.
        /// </summary>
        /// <param name="id">ID del horario.</param>
        /// <returns>Resultado de la operación con los detalles del horario.</returns>
        public async Task<ActionResult> FindAsync(int id)
        {
            var result = await horarioAlumnosService.Find(id);
            return AjaxResult(result);
        }

        /// <summary>
        /// Acción para crear un nuevo horario.
        /// </summary>
        /// <param name="model">Modelo de vista del horario.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateAsync(HorarioAlumnoViewModel model)
        {
            bool result = await horarioAlumnosService.Create(model);

            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.Success);
        }

        /// <summary>
        /// Acción para editar un horario existente.
        /// </summary>
        /// <param name="model">Modelo de vista del horario.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> EditAsync(HorarioAlumnoViewModel model)
        {
            bool result = await horarioAlumnosService.Edit(model);

            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessEdit);
        }

        /// <summary>
        /// Acción asincrónica para eliminar un horario.
        /// </summary>
        /// <param name="model">Modelo de vista del horario.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(HorarioAlumnoViewModel model)
        {
            bool result = await horarioAlumnosService.Delete(model.HoAl_Id);

            if (result)
            {
                return AjaxResult(false, AlertMessage.AlertMessageCustomType.Error);
            }
            return AjaxResult(true, AlertMessage.AlertMessageCustomType.SuccessDelete);
        }

        /// <summary>
        /// Obtiene la lista de cursos para el dropdown.
        /// </summary>
        /// <returns>Lista de cursos.</returns>
        public async Task<ActionResult> GetCursosAsync()
        {
            var result = await cursosService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene la lista de niveles para el dropdown.
        /// </summary>
        /// <returns>Lista de niveles.</returns>
        public async Task<ActionResult> GetCursosNivelesAsync()
        {
            var result = await cursosNivelesService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene la lista de materias para el dropdown.
        /// </summary>
        /// <returns>Lista de materias.</returns>
        public async Task<ActionResult> GetMateriasAsync()
        {
            var result = await materiasService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene la lista de días para el dropdown.
        /// </summary>
        /// <returns>Lista de días.</returns>
        public async Task<ActionResult> GetDiasAsync()
        {
            var result = await diasService.ListAsync();
            return AjaxResult(result);
        }

        /// <summary>
        /// Obtiene la lista de horas para el dropdown.
        /// </summary>
        /// <returns>Lista de horas.</returns>
        public async Task<ActionResult> GetHorasAsync()
        {
            var result = await horasService.ListAsync();
            return AjaxResult(result);
        }
    }
}
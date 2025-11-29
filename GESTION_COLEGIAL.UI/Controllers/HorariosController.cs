using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    /// <summary>
    /// Controlador para gestionar la entidad Horarios.
    /// </summary>
    public class HorariosController : BaseController
    {
        private readonly HorariosService horariosService = new HorariosService();

        /// <summary>
        /// Muestra la vista para crear horario semanal completo.
        /// </summary>
        /// <returns>La vista Index con el formulario de horario semanal.</returns>
        public async Task<ActionResult> Index()
        {
            var model = new HorarioViewModel
            {
                Hor_Año = DateTime.Now.Year
            };

            var drop = await Dropdown(model);
            return View(drop);
        }

        /// <summary>
        /// Obtiene los datos para llenar los dropdowns de opciones relacionadas con los horarios.
        /// </summary>
        /// <param name="model">El modelo del horario.</param>
        /// <returns>El modelo del horario actualizado con los datos del dropdown.</returns>
        public async Task<HorarioViewModel> Dropdown(HorarioViewModel model)
        {
            return await horariosService.Dropdown(model);
        }

        /// <summary>
        /// Obtiene los cursos y niveles relacionados con un curso.
        /// </summary>
        /// <param name="id">ID del curso.</param>
        /// <returns>El resultado en formato Ajax.</returns>
        [HttpGet]
        public async Task<ActionResult> GetCursosNiveles(int id)
        {
            var result = await horariosService.CursosNivelesDropdown(id);
            IList<SelectListItem> resultToSelectListItem = result.Select(x => new SelectListItem()
            {
                Value = x.Cun_Id.ToString(),
                Text = x.Cun_Descripcion
            }).ToList();
            return AjaxResult(resultToSelectListItem);
        }
    }
}

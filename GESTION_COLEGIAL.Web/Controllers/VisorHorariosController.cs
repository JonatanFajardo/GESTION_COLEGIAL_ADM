using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services;
using GESTION_COLEGIAL.UI.Extensions;
using GESTION_COLEGIAL.UI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.UI.Controllers
{
    [SessionManager("Horario semanal")]
    public class VisorHorariosController : BaseController
    {
        private readonly HorariosService _service  = new HorariosService();
        private readonly UsuariosService _usuarios = new UsuariosService();

        public async Task<ActionResult> Index()
        {
            // Un docente solo debe ver SU propio horario. Para el resto de roles
            // (admin, director, etc.) el visor funciona igual que siempre.
            bool esDocente = EsDocente();
            ViewBag.EsDocente = esDocente;
            ViewBag.EmpId = esDocente ? await ResolverEmpIdAsync() : 0;
            return View();
        }

        public async Task<ActionResult> ListAsync()
        {
            IEnumerable<HorarioViewModel> result = await _service.ListAsync()
                                                   ?? Enumerable.Empty<HorarioViewModel>();

            // Filtrado server-side: si el usuario logueado es docente, solo devolvemos
            // las clases donde él es el profesor. Si no tiene Emp_Id válido, el filtro
            // deja la lista vacía (la vista muestra el mensaje correspondiente).
            if (EsDocente())
            {
                int empId = await ResolverEmpIdAsync();
                result = result.Where(h => h.Emp_Id == empId).ToList();
            }

            return AjaxResult(result);
        }

        /// <summary>
        /// Determina si el usuario logueado tiene rol docente (case-insensitive).
        /// </summary>
        private bool EsDocente()
        {
            var rol = Session["RoleName"] as string ?? "";
            return rol.IndexOf("docente", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Resuelve el Emp_Id del usuario logueado. Prioriza la sesión (seteada en
        /// login) y, como respaldo para sesiones previas al cambio, lo resuelve vía
        /// Usuarios/FindAsync. Devuelve 0 si no hay un empleado asociado.
        /// </summary>
        private async Task<int> ResolverEmpIdAsync()
        {
            if (Session["EmpId"] != null &&
                int.TryParse(Session["EmpId"].ToString(), out int empSesion) &&
                empSesion > 0)
            {
                return empSesion;
            }

            var usuario = await _usuarios.Find(GetUsuarioId());
            int empId = usuario?.Emp_Id ?? 0;
            if (empId > 0) Session["EmpId"] = empId;
            return empId;
        }
    }
}

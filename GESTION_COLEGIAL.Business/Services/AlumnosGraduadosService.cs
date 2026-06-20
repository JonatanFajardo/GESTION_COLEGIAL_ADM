using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models.Graduacion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Servicio MVC del directorio de graduados. Consume Graduados/ListAsync y FindAsync.
    /// </summary>
    public class AlumnosGraduadosService
    {
        public async Task<IEnumerable<AlumnoGraduadoViewModel>> ListAsync()
        {
            string url = "Graduados/ListAsync";
            var result = await ApiRequests.ListAsync<AlumnoGraduadoViewModel>(url);
            return result ?? new List<AlumnoGraduadoViewModel>();
        }

        public async Task<AlumnoGraduadoViewModel> FindAsync(int id)
        {
            string url = $"Graduados/FindAsync?id={id}";
            return await ApiRequests.GetSingleAsync<AlumnoGraduadoViewModel>(url);
        }
    }
}

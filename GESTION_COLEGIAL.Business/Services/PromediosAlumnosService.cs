using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class PromediosAlumnosService
    {
        public async Task<List<PromedioAlumnoViewModel>> ListAsync(int anio, int umbralBajo = 70)
        {
            return await SendHttpClient.GetAsync<PromedioAlumnoViewModel>(
                $"PromediosAlumnos/ListAsync?Anio={anio}&UmbralBajo={umbralBajo}");
        }
    }
}

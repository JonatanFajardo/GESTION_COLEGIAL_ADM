using GESTION_COLEGIAL.Business.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class DocumentosAlumnoAdmService
    {
        public async Task<List<dynamic>> ListAsync(int Alu_Id)
            => await SendHttpClient.GetAsync<dynamic>($"DocumentosAlumno/ListAsync?Alu_Id={Alu_Id}");

        public async Task<List<dynamic>> PendientesAsync()
            => await SendHttpClient.GetAsync<dynamic>("DocumentosAlumno/PendientesAsync");

        public async Task<bool> UpdateAsync(dynamic model)
            => await SendHttpClient.PutAsync("DocumentosAlumno/UpdateAsync", model);
    }
}

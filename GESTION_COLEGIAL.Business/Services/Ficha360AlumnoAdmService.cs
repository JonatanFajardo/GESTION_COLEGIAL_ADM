using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class Ficha360AlumnoAdmService
    {
        public async Task<dynamic> GetAsync(int Alu_Id)
            => await SendHttpClient.GetSingleAsync<dynamic>($"Ficha360Alumno/ResumenAsync?Alu_Id={Alu_Id}");
    }
}

using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class Ficha360AlumnoAdmService
    {
        private readonly string _url = "Ficha360Alumno";

        public async Task<dynamic> GetResumenAsync(int Alu_Id)
        {
            return await SendHttpClient.GetSingleAsync<dynamic>(
                $"{_url}/ResumenAsync?Alu_Id={Alu_Id}");
        }
    }
}

using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class Ficha360EmpleadoAdmService
    {
        public async Task<dynamic> GetAsync(int Emp_Id)
            => await SendHttpClient.GetSingleAsync<dynamic>($"Ficha360Empleado/ResumenAsync?Emp_Id={Emp_Id}");
    }
}

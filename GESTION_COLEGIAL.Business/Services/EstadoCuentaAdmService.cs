using GESTION_COLEGIAL.Business.Helpers;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class EstadoCuentaAdmService
    {
        public async Task<dynamic> GetAsync(int Alu_Id, int? Anio = null)
        {
            string url = $"EstadoCuenta/FindAsync?Alu_Id={Alu_Id}";
            if (Anio.HasValue) url += $"&Anio={Anio}";
            return await SendHttpClient.GetSingleAsync<dynamic>(url);
        }
    }
}

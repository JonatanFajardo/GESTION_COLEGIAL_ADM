using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class AsistenciaAdmService
    {
        public async Task<List<dynamic>> ListAsync(int Hor_Id, string Fecha)
            => await SendHttpClient.GetAsync<dynamic>($"Asistencia/ListAsync?Hor_Id={Hor_Id}&Fecha={Fecha}");

        /// <summary>Horarios que ve el usuario en una fecha (dirección: todos del año; docente: los suyos y los que cubre).</summary>
        public async Task<List<PaseListaHorarioViewModel>> HorariosAsync(int Usu_Id, string Fecha, int? Niv_Id, int? Cun_Id, int? Sec_Id)
            => await SendHttpClient.GetAsync<PaseListaHorarioViewModel>(
                $"Asistencia/HorariosAsync?Usu_Id={Usu_Id}&Fecha={Fecha}"
                + (Niv_Id > 0 ? $"&Niv_Id={Niv_Id}" : "") + (Cun_Id > 0 ? $"&Cun_Id={Cun_Id}" : "") + (Sec_Id > 0 ? $"&Sec_Id={Sec_Id}" : ""))
               ?? new List<PaseListaHorarioViewModel>();

        /// <summary>¿Puede este usuario pasar lista de este horario en esa fecha? Si la API no responde, no.</summary>
        public async Task<bool> PuedeAsync(int Usu_Id, int Hor_Id, string Fecha)
        {
            var r = await SendHttpClient.GetSingleAsync<PaseListaPuedeViewModel>(
                $"Asistencia/PuedeAsync?Usu_Id={Usu_Id}&Hor_Id={Hor_Id}&Fecha={Fecha}");
            return r != null && r.Puede;
        }

        public async Task<List<dynamic>> ByAlumnoAsync(int Alu_Id, int Anio)
            => await SendHttpClient.GetAsync<dynamic>($"Asistencia/ByAlumnoAsync?Alu_Id={Alu_Id}&Anio={Anio}");

        public async Task<bool> CreateAsync(dynamic model)
            => await SendHttpClient.PostAsync("Asistencia/CreateAsync", model);

        public async Task<bool> EditAsync(dynamic model)
            => await SendHttpClient.PutAsync("Asistencia/EditAsync", model);
    }
}

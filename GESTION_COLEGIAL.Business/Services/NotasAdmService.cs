using GESTION_COLEGIAL.Business.Helpers;
using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    public class NotasAdmService
    {
        // GET /api/v1/Notas/HorariosAsync?Usu_Id= : horarios (materia + sección + semestre) del año activo
        // que el usuario puede capturar (un docente solo los suyos).
        public async Task<List<dynamic>> HorariosAsync(int usuId)
            => await SendHttpClient.GetAsync<dynamic>($"Notas/HorariosAsync?Usu_Id={usuId}");

        // GET /api/v1/Notas/ListAsync?Hor_Id=&Par_Id=&Usu_Id= : alumnos activos con su nota.
        // Devuelve el JSON tal cual: una lista, o { Error, Message } si el usuario no puede ver ese horario.
        public async Task<string> ListRawAsync(int horId, int parId, int usuId)
            => await SendHttpClient.GetRawAsync($"Notas/ListAsync?Hor_Id={horId}&Par_Id={parId}&Usu_Id={usuId}");

        // POST /api/v1/Notas/CreateAsync : crea la nota o actualiza la que ya existe.
        public async Task<RespuestaApi> GuardarAsync(object model)
            => await SendHttpClient.PostConMensajeAsync("Notas/CreateAsync", model);
    }
}

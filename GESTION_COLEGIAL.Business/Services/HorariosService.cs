using GESTION_COLEGIAL.Business.Extensions;
using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Clase que proporciona servicios relacionados con los horarios.
    /// </summary>
    public class HorariosService
    {
        private readonly CursosService cursosService = new CursosService();
        private readonly MateriasService materiasService = new MateriasService();
        private readonly EmpleadosService empleadosService = new EmpleadosService();
        private readonly SeccionesService seccionesService = new SeccionesService();
        private readonly AulasService aulasService = new AulasService();
        private readonly DiasService diasService = new DiasService();
        private readonly HorasService horasService = new HorasService();
        private readonly SemestresService semestresService = new SemestresService();
        private readonly ModalidadesService modalidadesService = new ModalidadesService();
        private readonly CursosNivelesService cursosNivelesService = new CursosNivelesService();

        /// <summary>
        /// Obtiene una lista de horarios de forma asíncrona.
        /// </summary>
        /// <returns>Una colección de objetos HorarioViewModel.</returns>
        public async Task<IEnumerable<HorarioViewModel>> ListAsync()
        {
            string url = "Horarios/ListAsync";
            IEnumerable<HorarioViewModel> apiUrl = await ApiRequests.ListAsync<HorarioViewModel>(url);
            return apiUrl;
        }

        /// <summary>
        /// Busca un horario por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del horario.</param>
        /// <returns>El objeto HorarioViewModel encontrado.</returns>
        public async Task<HorarioViewModel> Find(int id)
        {
            string url = "Horarios/FindAsync";
            HorarioViewModel apiUrl = await ApiRequests.FindAsync<HorarioViewModel>(url, id);
            return apiUrl;
        }

        /// <summary>
        /// Crea un nuevo horario de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto HorarioViewModel a crear.</param>
        /// <returns>True si el horario se creó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Create(HorarioViewModel model)
        {
            string url = "Horarios/CreateAsync";
            return await ApiRequests.CreateAsync(url, model);
        }

        /// <summary>
        /// Edita un horario existente de forma asíncrona.
        /// </summary>
        /// <param name="model">El objeto HorarioViewModel a editar.</param>
        /// <returns>True si el horario se editó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Edit(HorarioViewModel model)
        {
            string url = "Horarios/EditAsync";
            return await ApiRequests.EditAsync(url, model);
        }

        /// <summary>
        /// Verifica si un valor de horario existe de forma asíncrona.
        /// </summary>
        /// <param name="value">El valor de horario a verificar.</param>
        /// <returns>El objeto HorarioViewModel si existe, de lo contrario null.</returns>
        public async Task<HorarioViewModel> Exist(string value)
        {
            string url = "Horarios/ExistAsync";
            return await ApiRequests.ExistAsync<HorarioViewModel>(url, value);
        }

        /// <summary>
        /// Elimina un horario por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">El identificador del horario a eliminar.</param>
        /// <returns>True si el horario se eliminó correctamente, de lo contrario False.</returns>
        public async Task<Boolean> Delete(int id)
        {
            string url = "Horarios/RemoveAsync";
            return await ApiRequests.DeleteAsync(url, id);
        }

        /// <summary>
        /// Obtiene un modelo HorarioViewModel con las listas desplegables (dropdowns) cargadas.
        /// </summary>
        /// <param name="model">El modelo HorarioViewModel al que se le cargarán las listas desplegables.</param>
        /// <returns>El modelo HorarioViewModel con las listas desplegables cargadas.</returns>
        public async Task<HorarioViewModel> Dropdown(HorarioViewModel model)
        {
            // Obtener las listas para los dropdowns de forma paralela
            // Nota: Usamos ListAsync() porque no todos los controladores del API tienen endpoints *Dropdown específicos
            var cursosTask = cursosService.ListAsync();
            var materiasTask = materiasService.ListAsync();
            var empleadosTask = empleadosService.ListAsync();
            var seccionesTask = seccionesService.ListAsync();
            var aulasTask = aulasService.ListAsync();
            var diasTask = diasService.ListAsync();
            var horasTask = horasService.ListAsync();
            var semestresTask = semestresService.ListAsync();
            var modalidadesTask = modalidadesService.ListAsync();

            // Esperar a que todas las tareas se completen
            await Task.WhenAll(
                cursosTask,
                materiasTask,
                empleadosTask,
                seccionesTask,
                aulasTask,
                diasTask,
                horasTask,
                semestresTask,
                modalidadesTask
            );

            // Obtener los resultados con manejo de nulls (usar colecciones vacías si alguna falla)
            var cursos = (await cursosTask) ?? Enumerable.Empty<CursoViewModel>();
            var materias = (await materiasTask) ?? Enumerable.Empty<MateriaViewModel>();
            var empleados = (await empleadosTask) ?? Enumerable.Empty<EmpleadoViewModel>();
            var secciones = (await seccionesTask) ?? Enumerable.Empty<SeccionViewModel>();
            var aulas = (await aulasTask) ?? Enumerable.Empty<AulaViewModel>();
            var dias = (await diasTask) ?? Enumerable.Empty<DiaViewModel>();
            var horas = (await horasTask) ?? Enumerable.Empty<HoraViewModel>();
            var semestres = (await semestresTask) ?? Enumerable.Empty<SemestreViewModel>();
            var modalidades = (await modalidadesTask) ?? Enumerable.Empty<ModalidadViewModel>();

            // Cargar las listas desplegables en el modelo
            // Orden según LoadDropDownList: Cursos, Materias, Empleados, Secciones, Aulas, Dias, Horas, Semestres, Modalidades
            model.LoadDropDownList(
                cursos,           // 1. IEnumerable<CursoViewModel>
                materias,         // 2. IEnumerable<MateriaViewModel>
                empleados,        // 3. IEnumerable<EmpleadoViewModel>
                secciones,        // 4. IEnumerable<SeccionViewModel>
                aulas,            // 5. IEnumerable<AulaViewModel>
                dias,             // 6. IEnumerable<DiaViewModel>
                horas,            // 7. IEnumerable<HoraViewModel>
                semestres,        // 8. IEnumerable<SemestreViewModel>
                modalidades       // 9. IEnumerable<ModalidadViewModel>
            );

            return model;
        }

        /// <summary>
        /// Obtiene una lista de cursos niveles desplegables para un ID de nivel educativo específico de forma asíncrona.
        /// </summary>
        /// <param name="nivId">El ID del nivel educativo.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de cursos niveles desplegables.</returns>
        public async Task<IEnumerable<CursoNivelViewModel>> CursosNivelesDropdown(int nivId)
        {
            string urlCursosNiveles = $"CursosNiveles/CursosNivelesDropdown?id={nivId}";
            var cursosNivelesDropdown = await ApiRequests.DropdownAsync<CursoNivelViewModel>(urlCursosNiveles);
            return cursosNivelesDropdown;
        }

        /// <summary>
        /// Obtiene una lista de secciones desplegables para un ID de curso específico de forma asíncrona.
        /// </summary>
        /// <param name="curId">El ID del curso.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene la lista de secciones desplegables.</returns>
        public async Task<IEnumerable<SeccionViewModel>> SeccionesDropdown(int curId)
        {
            string urlSecciones = $"Secciones/SeccionesDropdown?id={curId}";
            var seccionesDropdown = await ApiRequests.DropdownAsync<SeccionViewModel>(urlSecciones);
            return seccionesDropdown;
        }
    }
}

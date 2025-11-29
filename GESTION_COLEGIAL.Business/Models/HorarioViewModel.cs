using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista para la entidad Horario.
    /// </summary>
    public class HorarioViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del horario.
        /// </summary>
        [Key]
        public int Hor_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del curso.
        /// </summary>
        [Display(Name = "Curso")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cur_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del curso.
        /// </summary>
        public string Cur_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del nivel del curso.
        /// </summary>
        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cun_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del curso nivel.
        /// </summary>
        public string Cun_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la materia.
        /// </summary>
        [Display(Name = "Materia")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Mat_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la materia.
        /// </summary>
        public string Mat_Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del empleado (profesor).
        /// </summary>
        [Display(Name = "Profesor")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Emp_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre completo del empleado (profesor).
        /// </summary>
        public string Emp_NombreCompleto { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la sección.
        /// </summary>
        [Display(Name = "Sección")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sec_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la sección.
        /// </summary>
        public string Sec_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del aula.
        /// </summary>
        [Display(Name = "Aula")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Aul_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del aula.
        /// </summary>
        public string Aul_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del día.
        /// </summary>
        [Display(Name = "Día")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Dia_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del día.
        /// </summary>
        public string Dia_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de inicio del horario (formato HH:mm).
        /// </summary>
        [Display(Name = "Hora inicio")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Hor_HoraInicio { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la hora de inicio.
        /// </summary>
        public string Hor_HoraInicioDescripcion { get; set; }

        /// <summary>
        /// Obtiene o establece la hora de finalización del horario (formato HH:mm).
        /// </summary>
        [Display(Name = "Hora finaliza")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string Hor_HoraFinaliza { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la hora de finalización.
        /// </summary>
        public string Hor_HoraFinalizaDescripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del semestre.
        /// </summary>
        [Display(Name = "Semestre")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sem_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del semestre.
        /// </summary>
        public string Sem_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID de la modalidad.
        /// </summary>
        [Display(Name = "Modalidad")]
        public int? Mda_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de la modalidad.
        /// </summary>
        public string Mda_Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el año académico.
        /// </summary>
        [Display(Name = "Año Académico")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Range(2020, 2050, ErrorMessage = "El año debe estar entre 2020 y 2050")]
        public int Hor_Año { get; set; }

        /// <summary>
        /// Obtiene o establece la duración predeterminada de las clases en minutos.
        /// </summary>
        [Display(Name = "Duración de Clase (minutos)")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Range(10, 180, ErrorMessage = "La duración debe estar entre 10 y 180 minutos")]
        public int Hor_DuracionMinutos { get; set; } = 40;

        /// <summary>
        /// Obtiene o establece el ID del aula predeterminada para las clases.
        /// </summary>
        [Display(Name = "Aula Predeterminada")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Hor_AulaPredeterminada { get; set; }

        /// <summary>
        /// Obtiene o establece si el horario está eliminado.
        /// </summary>
        public bool Hor_EsEliminado { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que registró el horario.
        /// </summary>
        [Display(Name = "Usuario que registra ID")]
        public int Hor_UsuarioRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que registró el horario.
        /// </summary>
        [Display(Name = "Usuario que registra Nombre")]
        public string Hor_UsuarioRegistraNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de registro del horario.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime Hor_FechaRegistra { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del usuario que modificó el horario.
        /// </summary>
        [Display(Name = "Usuario que modifica ID")]
        public int? Hor_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del usuario que modificó el horario.
        /// </summary>
        [Display(Name = "Usuario que modifica Nombre")]
        public string Hor_UsuarioModificaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de modificación del horario.
        /// </summary>
        [Display(Name = "Fecha de modificación")]
        public DateTime? Hor_FechaModifica { get; set; }

        #region Dropdown

        /// <summary>
        /// Propiedad con listado de cursos.
        /// </summary>
        public IEnumerable<SelectListItem> CursosList { get; set; }

        /// <summary>
        /// Propiedad con listado de cursos niveles.
        /// </summary>
        public IEnumerable<SelectListItem> NivelesList { get; set; }

        /// <summary>
        /// Propiedad con listado de materias.
        /// </summary>
        public IEnumerable<SelectListItem> MateriasList { get; set; }

        /// <summary>
        /// Propiedad con listado de empleados (profesores).
        /// </summary>
        public IEnumerable<SelectListItem> EmpleadosList { get; set; }

        /// <summary>
        /// Propiedad con listado de secciones.
        /// </summary>
        public IEnumerable<SelectListItem> SeccionesList { get; set; }

        /// <summary>
        /// Propiedad con listado de aulas.
        /// </summary>
        public IEnumerable<SelectListItem> AulasList { get; set; }

        /// <summary>
        /// Propiedad con listado de días.
        /// </summary>
        public IEnumerable<SelectListItem> DiasList { get; set; }

        /// <summary>
        /// Propiedad con listado de horas.
        /// </summary>
        public IEnumerable<SelectListItem> HorasList { get; set; }

        /// <summary>
        /// Propiedad con listado de semestres.
        /// </summary>
        public IEnumerable<SelectListItem> SemestresList { get; set; }

        /// <summary>
        /// Propiedad con listado de modalidades.
        /// </summary>
        public IEnumerable<SelectListItem> ModalidadesList { get; set; }

        /// <summary>
        /// Carga los elementos desplegables para las listas desplegables.
        /// </summary>
        public void LoadDropDownList(
            IEnumerable<CursoViewModel> cursos,
            IEnumerable<MateriaViewModel> materias,
            IEnumerable<EmpleadoViewModel> empleados,
            IEnumerable<SeccionViewModel> secciones,
            IEnumerable<AulaViewModel> aulas,
            IEnumerable<DiaViewModel> dias,
            IEnumerable<HoraViewModel> horas,
            IEnumerable<SemestreViewModel> semestres,
            IEnumerable<ModalidadViewModel> modalidades)
        {
            // Usa colecciones vacías si los parámetros son null para evitar ArgumentNullException
            CursosList = new SelectList(cursos ?? Enumerable.Empty<CursoViewModel>(), "Cur_Id", "Cur_Nombre");
            MateriasList = new SelectList(materias ?? Enumerable.Empty<MateriaViewModel>(), "Mat_Id", "Mat_Nombre");
            EmpleadosList = new SelectList(empleados ?? Enumerable.Empty<EmpleadoViewModel>(), "Emp_Id", "Emp_Nombre");
            SeccionesList = new SelectList(secciones ?? Enumerable.Empty<SeccionViewModel>(), "Sec_Id", "Sec_Descripcion");
            AulasList = new SelectList(aulas ?? Enumerable.Empty<AulaViewModel>(), "Aul_Id", "Aul_Descripcion");
            DiasList = new SelectList(dias ?? Enumerable.Empty<DiaViewModel>(), "Dia_Id", "Dia_Descripcion");
            HorasList = new SelectList(horas ?? Enumerable.Empty<HoraViewModel>(), "Hor_Id", "Hor_Hora");
            SemestresList = new SelectList(semestres ?? Enumerable.Empty<SemestreViewModel>(), "Sem_Id", "Sem_Descripcion");
            ModalidadesList = new SelectList(modalidades ?? Enumerable.Empty<ModalidadViewModel>(), "Mda_Id", "Mda_Descripcion");
        }

        #endregion Dropdown
    }
}

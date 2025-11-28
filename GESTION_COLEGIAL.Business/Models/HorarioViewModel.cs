using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GESTION_COLEGIAL.Business.Models
{
    public class HorarioViewModel
    {
        // Contexto Académico
        [Required(ErrorMessage = "El año académico es requerido")]
        [Range(2020, 2030, ErrorMessage = "El año debe estar entre 2020 y 2030")]
        [Display(Name = "Año Académico")]
        public int HorAño { get; set; }

        [Required(ErrorMessage = "El semestre es requerido")]
        [Display(Name = "Semestre")]
        public int SemId { get; set; }

        [Display(Name = "Modalidad")]
        public int? MdaId { get; set; }

        [Required(ErrorMessage = "El curso es requerido")]
        [Display(Name = "Curso")]
        public int CurId { get; set; }

        [Required(ErrorMessage = "El nivel es requerido")]
        [Display(Name = "Nivel")]
        public int CunId { get; set; }

        [Required(ErrorMessage = "La sección es requerida")]
        [Display(Name = "Sección")]
        public int SecId { get; set; }

        // Lista de clases del horario semanal
        public List<ClaseHorarioViewModel> Clases { get; set; }

        public HorarioViewModel()
        {
            HorAño = DateTime.Now.Year;
            Clases = new List<ClaseHorarioViewModel>();
        }
    }

    public class ClaseHorarioViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El día es requerido")]
        [Display(Name = "Día")]
        public int DiaId { get; set; }

        [Required(ErrorMessage = "La materia es requerida")]
        [Display(Name = "Materia")]
        public int MatId { get; set; }

        [Required(ErrorMessage = "El profesor es requerido")]
        [Display(Name = "Profesor")]
        public int EmpId { get; set; }

        [Required(ErrorMessage = "El aula es requerida")]
        [Display(Name = "Aula")]
        public int AulId { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida")]
        [Display(Name = "Hora Inicio")]
        public int HorHoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de finalización es requerida")]
        [Display(Name = "Hora Finaliza")]
        public int HorHoraFinaliza { get; set; }
    }

    // ViewModels para los dropdowns
    public class HorarioFormDataViewModel
    {
        public HorarioViewModel Horario { get; set; }

        // Listas para los dropdowns
        public List<CursoDto> Cursos { get; set; }
        public List<NivelDto> Niveles { get; set; }
        public List<MateriaDto> Materias { get; set; }
        public List<EmpleadoDto> Empleados { get; set; }
        public List<SeccionDto> Secciones { get; set; }
        public List<AulaDto> Aulas { get; set; }
        public List<DiaDto> Dias { get; set; }
        public List<HoraDto> Horas { get; set; }
        public List<SemestreDto> Semestres { get; set; }
        public List<ModalidadDto> Modalidades { get; set; }

        public HorarioFormDataViewModel()
        {
            Horario = new HorarioViewModel();
            Cursos = new List<CursoDto>();
            Niveles = new List<NivelDto>();
            Materias = new List<MateriaDto>();
            Empleados = new List<EmpleadoDto>();
            Secciones = new List<SeccionDto>();
            Aulas = new List<AulaDto>();
            Dias = new List<DiaDto>();
            Horas = new List<HoraDto>();
            Semestres = new List<SemestreDto>();
            Modalidades = new List<ModalidadDto>();
        }
    }

    // DTOs para los dropdowns
    public class CursoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class NivelDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CurId { get; set; }
    }

    public class MateriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class EmpleadoDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
    }

    public class SeccionDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class AulaDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class DiaDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class HoraDto
    {
        public int Id { get; set; }
        public string Hora { get; set; }
    }

    public class SemestreDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

    public class ModalidadDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}

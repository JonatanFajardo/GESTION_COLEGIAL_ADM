using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Controllers
{
    public class HorariosController : BaseController
    {
        // GET: Horarios
        public ActionResult Index()
        {
            var model = new HorarioFormDataViewModel
            {
                Horario = new HorarioViewModel
                {
                    HorAño = DateTime.Now.Year
                },

                // Datos de ejemplo - estos deberán venir de servicios/API
                Cursos = new List<CursoDto>
                {
                    new CursoDto { Id = 1, Nombre = "Matemáticas" },
                    new CursoDto { Id = 2, Nombre = "Español" },
                    new CursoDto { Id = 3, Nombre = "Ciencias Naturales" },
                    new CursoDto { Id = 4, Nombre = "Estudios Sociales" },
                    new CursoDto { Id = 5, Nombre = "Inglés" }
                },

                Niveles = new List<NivelDto>
                {
                    new NivelDto { Id = 1, Descripcion = "Nivel I", CurId = 1 },
                    new NivelDto { Id = 2, Descripcion = "Nivel II", CurId = 1 },
                    new NivelDto { Id = 3, Descripcion = "Nivel III", CurId = 1 },
                    new NivelDto { Id = 4, Descripcion = "Nivel I", CurId = 2 },
                    new NivelDto { Id = 5, Descripcion = "Nivel II", CurId = 2 }
                },

                Materias = new List<MateriaDto>
                {
                    new MateriaDto { Id = 1, Nombre = "Álgebra" },
                    new MateriaDto { Id = 2, Nombre = "Geometría" },
                    new MateriaDto { Id = 3, Nombre = "Cálculo" },
                    new MateriaDto { Id = 4, Nombre = "Literatura" },
                    new MateriaDto { Id = 5, Nombre = "Gramática" },
                    new MateriaDto { Id = 6, Nombre = "Biología" },
                    new MateriaDto { Id = 7, Nombre = "Química" },
                    new MateriaDto { Id = 8, Nombre = "Física" }
                },

                Empleados = new List<EmpleadoDto>
                {
                    new EmpleadoDto { Id = 1, NombreCompleto = "Juan Pérez García" },
                    new EmpleadoDto { Id = 2, NombreCompleto = "María López Martínez" },
                    new EmpleadoDto { Id = 3, NombreCompleto = "Carlos Rodríguez Sánchez" },
                    new EmpleadoDto { Id = 4, NombreCompleto = "Ana Gómez Fernández" },
                    new EmpleadoDto { Id = 5, NombreCompleto = "Luis Martínez Díaz" }
                },

                Secciones = new List<SeccionDto>
                {
                    new SeccionDto { Id = 1, Descripcion = "A" },
                    new SeccionDto { Id = 2, Descripcion = "B" },
                    new SeccionDto { Id = 3, Descripcion = "C" },
                    new SeccionDto { Id = 4, Descripcion = "D" }
                },

                Aulas = new List<AulaDto>
                {
                    new AulaDto { Id = 1, Descripcion = "Aula 101" },
                    new AulaDto { Id = 2, Descripcion = "Aula 102" },
                    new AulaDto { Id = 3, Descripcion = "Aula 103" },
                    new AulaDto { Id = 4, Descripcion = "Laboratorio de Ciencias" },
                    new AulaDto { Id = 5, Descripcion = "Laboratorio de Computación" },
                    new AulaDto { Id = 6, Descripcion = "Gimnasio" },
                    new AulaDto { Id = 7, Descripcion = "Auditorio" }
                },

                Dias = new List<DiaDto>
                {
                    new DiaDto { Id = 1, Descripcion = "Lunes" },
                    new DiaDto { Id = 2, Descripcion = "Martes" },
                    new DiaDto { Id = 3, Descripcion = "Miércoles" },
                    new DiaDto { Id = 4, Descripcion = "Jueves" },
                    new DiaDto { Id = 5, Descripcion = "Viernes" }
                },

                Horas = new List<HoraDto>
                {
                    new HoraDto { Id = 1, Hora = "07:00 AM" },
                    new HoraDto { Id = 2, Hora = "07:45 AM" },
                    new HoraDto { Id = 3, Hora = "08:30 AM" },
                    new HoraDto { Id = 4, Hora = "09:15 AM" },
                    new HoraDto { Id = 5, Hora = "10:00 AM" },
                    new HoraDto { Id = 6, Hora = "10:45 AM" },
                    new HoraDto { Id = 7, Hora = "11:30 AM" },
                    new HoraDto { Id = 8, Hora = "12:15 PM" },
                    new HoraDto { Id = 9, Hora = "01:00 PM" },
                    new HoraDto { Id = 10, Hora = "01:45 PM" },
                    new HoraDto { Id = 11, Hora = "02:30 PM" },
                    new HoraDto { Id = 12, Hora = "03:15 PM" }
                },

                Semestres = new List<SemestreDto>
                {
                    new SemestreDto { Id = 1, Descripcion = "Primer Semestre" },
                    new SemestreDto { Id = 2, Descripcion = "Segundo Semestre" }
                },

                Modalidades = new List<ModalidadDto>
                {
                    new ModalidadDto { Id = 1, Descripcion = "Presencial" },
                    new ModalidadDto { Id = 2, Descripcion = "Virtual" },
                    new ModalidadDto { Id = 3, Descripcion = "Híbrida" }
                }
            };

            return View(model);
        }

        // POST: Horarios/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Guardar(HorarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor complete todos los campos obligatorios.";
                return RedirectToAction("Index");
            }

            try
            {
                // TODO: Aquí se implementará la lógica para guardar el horario
                // Llamar al servicio/API para guardar cada clase del horario

                // Por ahora, solo mostramos un mensaje de éxito
                TempData["Success"] = $"Horario guardado exitosamente. Total de clases: {model.Clases?.Count ?? 0}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al guardar el horario: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}

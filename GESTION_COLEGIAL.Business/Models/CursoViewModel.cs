using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
	/// <summary>
	/// Clase que representa el modelo de vista de un curso.
	/// </summary>
	public class CursoViewModel : BaseViewModel
	{
		/// <summary>
		/// Obtiene o establece el ID del curso.
		/// </summary>
		[Key]
		public int CursoId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del curso.
		/// </summary>
		[Display(Name = "Curso nombre")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string NombreCurso { get; set; }

		/// <summary>
		/// Obtiene o establece el ID de la materia asociada al curso.
		/// </summary>
		public int MateriaId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción de la materia asociada al curso.
		/// </summary>
		public string DescripcionMateria { get; set; }

		/// <summary>
		/// Obtiene o establece el ID de la sección del curso.
		/// </summary>
		[Display(Name = "Sección")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int SeccionId { get; set; }

		/// <summary>
		/// Obtiene o establece el ID del nivel educativo del curso.
		/// </summary>
		[Display(Name = "Nivel")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int NivelId { get; set; }

		/// <summary>
		/// Obtiene o establece la descripción del nivel educativo del curso.
		/// </summary>
		public string DescripcionNivel { get; set; }

		/// <summary>
		/// Obtiene o establece el ID de la modalidad del curso.
		/// </summary>
		[Display(Name = "Modalidad")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int ModalidadId { get; set; }

		/// <summary>
		/// Obtiene o establece el estado de activación del curso.
		/// </summary>
		[Display(Name = "Es activo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public string EsActivo { get; set; }

		/// <summary>
		/// Obtiene o establece un valor booleano que indica si el curso está activo.
		/// </summary>
		public bool EsActivoCurso { get; set; }

		/// <summary>
		/// Obtiene o establece el ID del usuario que registró el curso.
		/// </summary>
		[Display(Name = "Usuario registra Id")]
		public int UsuarioRegistraCursoId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que registró el curso.
		/// </summary>
		[Display(Name = "Usuario registra Nombre")]
		public string NombreUsuarioRegistraCurso { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha en que se registró el curso.
		/// </summary>
		[Column(TypeName = "datetime")]
		[Display(Name = "Fecha registra")]
		[Required(ErrorMessage = "El campo es requerido")]
		public DateTime FechaRegistroCurso { get; set; }

		/// <summary>
		/// Obtiene o establece el ID del usuario que modificó el curso.
		/// </summary>
		[Display(Name = "Usuario modifica Id")]
		[Required(ErrorMessage = "El campo es requerido")]
		public int? UsuarioModificaCursoId { get; set; }

		/// <summary>
		/// Obtiene o establece el nombre del usuario que modificó el curso.
		/// </summary>
		[Display(Name = "Usuario modifica Nombre")]
		public string NombreUsuarioModificaCurso { get; set; }

		/// <summary>
		/// Obtiene o establece la fecha en que se modificó el curso.
		/// </summary>
		[Column(TypeName = "datetime")]
		[Display(Name = "Fecha modifica")]
		[Required(ErrorMessage = "El campo es requerido")]
		public DateTime? FechaModificacionCurso { get; set; }

		/// <summary>
		/// Obtiene o establece los IDs de las modalidades asociadas al curso.
		/// </summary>
		public int[] Modalidades { get; set; }

		/// <summary>
		/// Obtiene o establece los IDs de los niveles de curso asociados al curso.
		/// </summary>
		public int[] CursoNiveles { get; set; }

		/// <summary>
		/// Obtiene o establece los IDs de las materias asociadas al curso.
		/// </summary>
		public int[] Materias { get; set; }

		/// <summary>
		/// Obtiene o establece los IDs de las secciones asociadas al curso.
		/// </summary>
		public int[] Secciones { get; set; }

		/// <summary>
		/// Obtiene o establece la lista de modalidades en formato de CheckBox.
		/// </summary>
		public IList<SelectListItem> ModalidadesCheckList { get; set; }

		/// <summary>
		/// Obtiene o establece la lista de secciones en formato de CheckBox.
		/// </summary>
		public IList<SelectListItem> SeccionesCheckList { get; set; }

		/// <summary>
		/// Obtiene o establece la lista de cursos niveles en formato de CheckBox.
		/// </summary>
		public IList<SelectListItem> CursoNivelesCheckList { get; set; }

		/// <summary>
		/// Obtiene o establece la lista de materias en formato de CheckBox.
		/// </summary>
		public IList<SelectListItem> MateriasCheckList { get; set; }

		/// <summary>
		/// Obtiene o establece la lista de niveles educativos para el dropdown.
		/// </summary>
		public SelectList NivelEducativoList { get; set; }

		/// <summary>
		/// Carga los datos solicitados para el formato de CheckBox.
		/// </summary>
		/// <param name="modalidades">La lista de modalidades.</param>
		/// <param name="secciones">La lista de secciones.</param>
		/// <param name="cursoNiveles">La lista de cursos niveles.</param>
		/// <param name="materias">La lista de materias.</param>
		public void LoadCheckList(IList<ModalidadViewModel> modalidades, IList<SeccionViewModel> secciones,
									List<CursoNivelViewModel> cursoNiveles, List<MateriaViewModel> materias)
		{
			// Evaluamos si la propiedad viene vacía.
			if (modalidades == null)
				return;

			ModalidadesCheckList = modalidades.Select(x => new SelectListItem()
			{
				Text = x.DescripcionModalidad,
				Value = x.ModalidadId.ToString(),
				Selected = x.IsSelected
			}).ToList();

			SeccionesCheckList = secciones.Select(x => new SelectListItem()
			{
				Text = x.DescripcionSeccion,
				Value = x.SeccionId.ToString(),
				Selected = x.IsSelected
			}).ToList();

			CursoNivelesCheckList = cursoNiveles.Select(x => new SelectListItem()
			{
				Text = x.DescripcionCursoNivel,
				Value = x.CursoNivelId.ToString(),
				Selected = x.IsSelected
			}).ToList();

			MateriasCheckList = materias.Select(x => new SelectListItem()
			{
				Text = x.NombreMateria,
				Value = x.MateriaId.ToString(),
				Selected = x.IsSelected
			}).ToList();
		}

		/// <summary>
		/// Carga los datos solicitados para el formato de dropdown.
		/// </summary>
		/// <param name="nivelEducativoDropdownResults">La lista de niveles educativos.</param>
		public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoDropdownResults)
		{
			NivelEducativoList = new SelectList(nivelEducativoDropdownResults, "NivelId", "DescripcionNivel");
		}
	}
}

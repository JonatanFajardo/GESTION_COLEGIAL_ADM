using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
	/// <summary>
	/// Representa el modelo de vista para un semestre.
	/// </summary>
	public class SemestreViewModel : BaseViewModel
	{
		/// <summary>
		/// Obtiene o establece el ID del semestre.
		/// </summary>
		[Key]
		public int SemestreId { get; set; }

		/// <summary>
		/// Descripción del semestre.
		/// </summary>
		[StringLength(100)]
		[Display(Name = "Descripción")]
		[Required(ErrorMessage = "El campo es requerido")]
		[Remote(action: "ExistAsync", controller: "Semestres", HttpMethod = "POST", AdditionalFields = nameof(SemestreId) + "," + nameof(DescripcionSemestre))]
		public string DescripcionSemestre { get; set; }

		/// <summary>
		/// Indica si el semestre está activo o no.
		/// </summary>
		[Display(Name = "Es activo")]
		//[Required(ErrorMessage = "El campo es requerido")]
		public string EsActivo { get; set; }

		/// <summary>
		/// Indica si el semestre está activo o no.
		/// </summary>
		public bool EsActivoSemestre { get; set; }

		/// <summary>
		/// Identificador del usuario que registra el semestre.
		/// </summary>
		[Display(Name = "Usuario registra Id")]
		public int UsuarioRegistraSemestreId { get; set; }

		/// <summary>
		/// Nombre del usuario que registra el semestre.
		/// </summary>
		[Display(Name = "Usuario registra Nombre")]
		public string NombreUsuarioRegistraSemestre { get; set; }

		/// <summary>
		/// Fecha de registro del semestre.
		/// </summary>
		[Display(Name = "Fecha registra")]
		public DateTime FechaRegistroSemestre { get; set; }

		/// <summary>
		/// Identificador del usuario que modifica el semestre.
		/// </summary>
		[Display(Name = "Usuario modifica Id")]
		public int? UsuarioModificaSemestreId { get; set; }

		/// <summary>
		/// Nombre del usuario que modifica el semestre.
		/// </summary>
		[Display(Name = "Usuario modifica Nombre")]
		public string NombreUsuarioModificaSemestre { get; set; }

		/// <summary>
		/// Fecha de modificación del semestre.
		/// </summary>
		[Display(Name = "Fecha modifica")]
		public DateTime? FechaModificacionSemestre { get; set; }
	}
}


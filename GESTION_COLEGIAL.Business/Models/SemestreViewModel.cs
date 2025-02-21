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
		public int Sem_Id { get; set; }

		/// <summary>
		/// Descripción del semestre.
		/// </summary>
		[StringLength(100)]
		[Display(Name = "Descripción")]
		[Required(ErrorMessage = "El campo es requerido")]
		[Remote(action: "ExistAsync", controller: "Semestres", HttpMethod = "POST", AdditionalFields = nameof(Sem_Id) + "," + nameof(Sem_Descripcion))]
		public string Sem_Descripcion { get; set; }

		/// <summary>
		/// Indica si el semestre está activo o no.
		/// </summary>
		[Display(Name = "Es activo")]
		//[Required(ErrorMessage = "El campo es requerido")]
		public string EsActivo { get; set; }

		/// <summary>
		/// Indica si el semestre está activo o no.
		/// </summary>
		public bool Sem_EsActivo { get; set; }

		/// <summary>
		/// Identificador del usuario que registra el semestre.
		/// </summary>
		[Display(Name = "Usuario registra Id")]
		public int Sem_UsuarioRegistra { get; set; }

		/// <summary>
		/// Nombre del usuario que registra el semestre.
		/// </summary>
		[Display(Name = "Usuario registra Nombre")]
		public string Sem_UsuarioRegistraNombre { get; set; }

		/// <summary>
		/// Fecha de registro del semestre.
		/// </summary>
		[Display(Name = "Fecha registra")]
		public DateTime Sem_FechaRegistra { get; set; }

		/// <summary>
		/// Identificador del usuario que modifica el semestre.
		/// </summary>
		[Display(Name = "Usuario modifica Id")]
		public int? Sem_UsuarioModifica { get; set; }

		/// <summary>
		/// Nombre del usuario que modifica el semestre.
		/// </summary>
		[Display(Name = "Usuario modifica Nombre")]
		public string Sem_UsuarioModificaNombre { get; set; }

		/// <summary>
		/// Fecha de modificación del semestre.
		/// </summary>
		[Display(Name = "Fecha modifica")]
		public DateTime? Sem_FechaModifica { get; set; }
	}
}

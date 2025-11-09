using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
	/// <summary>
	/// Representa el modelo de vista de un nivel educativo.
	/// </summary>
	public class NivelEducativoViewModel : BaseViewModel
	{
		/// <summary>
		/// Identificador del nivel educativo.
		/// </summary>
		[Key]
		public int NivelId { get; set; }

		/// <summary>
		/// Descripción del nivel educativo.
		/// </summary>
		[StringLength(100)]
		[Display(Name = "Descripción")]
		[Required(ErrorMessage = "El campo es requerido")]
		[Remote(action: "ExistAsync", controller: "NivelesEducativos", HttpMethod = "POST", AdditionalFields = nameof(NivelId) + "," + nameof(DescripcionNivel))]
		public string DescripcionNivel { get; set; }

		/// <summary>
		/// Indica si el nivel educativo está activo.
		/// </summary>
		[Display(Name = "Es activo")]
		[Required(ErrorMessage = "El campo es requerido")]
		public bool EsActivoNivel { get; set; }

		/// <summary>
		/// Representación en cadena del estado de activo.
		/// </summary>
		public string EsActivo { get; set; }

		/// <summary>
		/// Identificador del usuario que registra el nivel educativo.
		/// </summary>
		[Display(Name = "Usuario registra Id")]
		public int UsuarioRegistraNivelId { get; set; }

		/// <summary>
		/// Nombre del usuario que registra el nivel educativo.
		/// </summary>
		[Display(Name = "Usuario registra Nombre")]
		public string NombreUsuarioRegistraNivel { get; set; }

		/// <summary>
		/// Fecha de registro del nivel educativo.
		/// </summary>
		[Display(Name = "Fecha registra")]
		public DateTime FechaRegistroNivel { get; set; }

		/// <summary>
		/// Identificador del usuario que modifica el nivel educativo.
		/// </summary>
		[Display(Name = "Usuario modifica Id")]
		public int? UsuarioModificaNivelId { get; set; }

		/// <summary>
		/// Nombre del usuario que modifica el nivel educativo.
		/// </summary>
		[Display(Name = "Usuario modifica Nombre")]
		public string NombreUsuarioModificaNivel { get; set; }

		/// <summary>
		/// Fecha de modificación del nivel educativo.
		/// </summary>
		[Display(Name = "Fecha modifica")]
		public DateTime? FechaModificacionNivel { get; set; }

	}
}


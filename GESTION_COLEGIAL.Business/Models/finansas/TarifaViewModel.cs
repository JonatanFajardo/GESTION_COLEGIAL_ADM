using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// ViewModel base para las operaciones de Tarifa.
    /// </summary>
    public class TarifaViewModel
    {
        [Key]
        public int Tar_Id { get; set; }

        [Display(Name = "Concepto de Pago")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int ConceptoPago_Id { get; set; }

        public string ConceptoPago { get; set; }

        [Display(Name = "Nivel")]
        public int? Nivel_Id { get; set; }

        public string Nivel { get; set; }

        [Display(Name = "Curso")]
        public int? CursoNivel_Id { get; set; }

        public string Curso { get; set; }

        [Display(Name = "Monto")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que 0")]
        public decimal Tar_Monto { get; set; }

        [Display(Name = "Año de Vigencia")]
        [Required(ErrorMessage = "El campo es requerido")]
        [Range(2000, 2100, ErrorMessage = "El año debe estar entre 2000 y 2100")]
        public short Tar_AnioVigencia { get; set; }

        public bool Tar_EsEliminado { get; set; }
        public int Usu_RegistraId { get; set; }
        public DateTime Tar_FechaRegistro { get; set; }
        public int? Usu_ModificaId { get; set; }
        public DateTime? Tar_FechaModifica { get; set; }

        /// <summary>
        /// Lista desplegable de conceptos de pago.
        /// </summary>
        public SelectList ConceptosPagoList { get; set; }

        /// <summary>
        /// Lista desplegable de niveles educativos.
        /// </summary>
        public SelectList NivelesEducativosList { get; set; }

        /// <summary>
        /// Lista desplegable de cursos/niveles.
        /// </summary>
        public SelectList CursosNivelesList { get; set; }

        /// <summary>
        /// Carga las listas desplegables necesarias para el formulario.
        /// </summary>
        public void LoadDropDownList(IEnumerable<ConceptoPagoListViewModel> conceptosPago,
                                    IEnumerable<NivelEducativoViewModel> nivelesEducativos,
                                    IEnumerable<CursoNivelViewModel> cursosNiveles)
        {
            ConceptosPagoList = new SelectList(conceptosPago, "ConceptoPagoId", "Descripcion");
            NivelesEducativosList = new SelectList(nivelesEducativos, "Niv_Id", "Niv_Descripcion");
            CursosNivelesList = new SelectList(cursosNiveles, "Cun_Id", "Cun_Descripcion");
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;

namespace GESTION_COLEGIAL.UI.Models
{
    public class CursoViewModel : BaseViewModel
    {

        [Key]
        public int Cur_Id { get; set; }

        [Display(Name = "Curso nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Cno_Id { get; set; }
        public string Cno_Descripcion { get; set; }

        [Display(Name = "Aula")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Aul_Id { get; set; }
        public string Aul_Descripcion { get; set; }
        public int Mat_Id { get; set; }
        public string Mat_Descripcion { get; set; }

        [Display(Name = "Sección")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Sec_Id { get; set; }

        [Display(Name = "Nivel")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }

        [Display(Name = "Modalidad")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int Mda_Id { get; set; }

        [Display(Name = "Es activo")]
        [Required(ErrorMessage = "El campo es requerido")]
        public string EsActivo { get; set; }
        public bool Cur_EsActivo { get; set; }

        [Display(Name = "Usuario registra Id")]
        public int Cur_UsuarioRegistra { get; set; }

        [Display(Name = "Usuario registra Nombre")]
        public string Cur_UsuarioRegistraNombre { get; set; }
        [Column(TypeName = "datetime")]

        [Display(Name = "Fecha registra")]
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime Cur_FechaRegistra { get; set; }

        [Display(Name = "Usuario modifica Id")]
        [Required(ErrorMessage = "El campo es requerido")]
        public int? Cur_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modifica Nombre")]
        public string Cur_UsuarioModificaNombre { get; set; }
        [Column(TypeName = "datetime")]

        [Display(Name = "Fecha modifica")]
        [Required(ErrorMessage = "El campo es requerido")]
        public DateTime? Cur_FechaModifica { get; set; }

        public int[] Modalidades { get; set; }
        public int[] CursoNiveles { get; set; }
        public int[] Materias { get; set; }
        public int[] Secciones { get; set; }

        /// Check Box

        // Propiedad con la información de modalidades.
        public IList<SelectListItem> ModalidadesCheckList { get; set; }

        // Propiedad con la información de secciones.
        public IList<SelectListItem> SeccionesCheckList { get; set; }

        // Propiedad con la información de cursos niveles.
        public IList<SelectListItem> CursoNivelesCheckList { get; set; }

        // Propiedad con la información de cursos materias.
        public IList<SelectListItem> MateriasCheckList { get; set; }

        // Dropdown

        // Propiedad con listado de niveles educativos.
        public SelectList NivelEducativoList { get; set; }

        // Propiedad con listado de aulas.
        public SelectList AulaList { get; set; }

        // Propiedad con listado de curso nombre.
        public SelectList CursoNombreList { get; set; }

        #region CheckList
        /// <summary>
        /// Carga los datos solicitados.
        /// </summary>
        /// <remarks>
        /// Permite mostrar una colección de checkbox.
        /// </remarks>
        /// <param name="modalidades"></param>
        public void LoadCheckList( IList<ModalidadViewModel> modalidades, IList<SeccionViewModel> secciones,
                                    List<CursoNivelViewModel> cursoNiveles, List<MateriaViewModel> materias)
        {
            // Evaluamos si la propiedad viene vacia.
            if (modalidades == null)
                return;
            // Usamos SelectListItem para estandarizar la forma en la que se llamara en la vista.
            //ModalidadesCheckList = modalidades;
            ModalidadesCheckList = modalidades.Select(x => new SelectListItem()
            {
                Text = x.Mda_Descripcion, 
                Value = x.Mda_Id.ToString(),
                Selected = x.IsSelected
            }).ToList();
            SeccionesCheckList = secciones.Select(x => new SelectListItem()
            {
                Text = x.Sec_Descripcion,
                Value = x.Sec_Id.ToString(),
                Selected = x.IsSelected
            }).ToList();
            //CursoNiveles = cursoNiveles.Where(x => x.Niv_Id == Niv_Id).ToList();
            CursoNivelesCheckList = cursoNiveles.Select(x => new SelectListItem()
            {
                Text = x.Cun_Descripcion,
                Value = x.Cun_Id.ToString(),
                Selected = x.IsSelected
            }).ToList();
            MateriasCheckList = materias.Select(x => new SelectListItem()
            {
                Text = x.Mat_Nombre,
                Value = x.Mat_Id.ToString(),
                Selected = x.IsSelected
            }).ToList();
        }
        #endregion CheckList

        #region Dropdown
        /// <summary>
        /// Carga los datos solicitados.
        /// </summary>
        /// <remarks>
        /// Permite mostrar una colección de dropdown.
        /// </remarks>
        /// <param name="nivelEducativoDropdownResults"></param>
        /// <param name="aulaDropdownResults"></param>
        public void LoadDropDownList(IEnumerable<NivelEducativoViewModel> nivelEducativoDropdownResults,
                                    IEnumerable<AulaViewModel> aulaDropdownResults,
                                    IEnumerable<CursoNombreViewModel> cursoNombreDropdownResults)
        {
            NivelEducativoList = new SelectList(nivelEducativoDropdownResults, "Niv_Id", "Niv_Descripcion");
            AulaList = new SelectList(aulaDropdownResults, "Aul_Id", "Aul_Descripcion");
            CursoNombreList = new SelectList(cursoNombreDropdownResults, "Cno_Id", "Cno_Descripcion");
        }
        #endregion Dropdown
    }
}
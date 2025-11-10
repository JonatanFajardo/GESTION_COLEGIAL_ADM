

using System;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Administra la información de los cursos.
    /// </summary>
    public partial class tbCursos
    { 

        /// <summary>
        /// Identificador único del registro en la tabla.
        /// </summary>
        public int Cur_Id { get; set; }

        /// <summary>
        /// Identificador de un nombre en el curso.
        /// </summary>
        public string Cur_Nombre { get; set; }

        /// <summary>
        /// Identificador unico de la tabla tbNivelesEducativos.
        /// </summary>
        public int Niv_Id { get; set; }

        /// <summary>
        /// Estado en el que se encuentra el curso.
        /// </summary>
        public bool Cur_EsActivo { get; set; }

        public bool Cur_EsEliminado { get; set; }
        public int Cur_UsuarioRegistra { get; set; }
        public DateTime Cur_FechaRegistra { get; set; }
        public int? Cur_UsuarioModifica { get; set; }
        public DateTime? Cur_FechaModifica { get; set; }

        public int[] Modalidades { get; set; }
        public int[] CursoNiveles { get; set; }
        public int[] Secciones { get; set; }
        public int[] Materias { get; set; }

        public virtual tbUsuarios Cur_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Cur_UsuarioRegistraNavigation { get; set; }
        public virtual tbNivelesEducativos Niv { get; set; } 
    }
}
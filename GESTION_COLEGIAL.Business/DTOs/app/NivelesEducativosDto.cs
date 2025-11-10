
using System;

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Estos son los niveles de educación con los que se cuenta, en cada nivel se culmina un ciclo para el alumno.
    /// </summary>
    public partial class tbNivelesEducativos
    {
        public tbNivelesEducativos()
        {
            tbAlumnos = new HashSet<tbAlumnos>();
            tbCursos = new HashSet<tbCursos>();
            tbCursosNiveles = new HashSet<tbCursosNiveles>();
        }

        /// <summary>
        /// Identificador único de un nivel educativo.
        /// </summary>
        public int Niv_Id { get; set; }

        /// <summary>
        /// Información de el nivel del alumno.
        /// </summary>
        public string Niv_Descripcion { get; set; }

        public bool Niv_EsActivo { get; set; }
        public bool Niv_EsEliminado { get; set; }
        public int Niv_UsuarioRegistra { get; set; }
        public DateTime Niv_FechaRegistra { get; set; }
        public int? Niv_UsuarioModifica { get; set; }
        public DateTime? Niv_FechaModifica { get; set; }

        public virtual tbUsuarios Niv_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Niv_UsuarioRegistraNavigation { get; set; } 
    }
}
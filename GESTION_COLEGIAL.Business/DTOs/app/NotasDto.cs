#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Define las notas de cada alumno.
    /// </summary>
    public partial class tbNotas
    {
        /// <summary>
        /// Identificador único de la tabla.
        /// </summary>
        public int Not_Id { get; set; }

        /// <summary>
        /// Información sobre la nota dada al alumno.
        /// </summary>
        public int? Alu_Id { get; set; }

        /// <summary>
        /// Información sobre la nota dada al alumno.
        /// </summary>
        public int? Aul_Id { get; set; }

        /// <summary>
        /// Información sobre la nota dada al alumno.
        /// </summary>
        public int? Sec_Id { get; set; }

        /// <summary>
        /// Información sobre la nota dada al alumno.
        /// </summary>
        public int Not_Nota { get; set; }

        /// <summary>
        /// Identificador de la tabla tbMaterias.
        /// </summary>
        public int Mat_Id { get; set; }

        /// <summary>
        /// Identificador único de la tabla tbSemestres.
        /// </summary>
        public int Sem_Id { get; set; }

        /// <summary>
        /// Identificador único de la tabla tbParciales.
        /// </summary>
        public int Pac_Id { get; set; }

        /// <summary>
        /// Año en el que se aplico la nota.
        /// </summary>
        public DateTime Not_Año { get; set; }

        /// <summary>
        /// Estado de la nota.
        /// </summary>
        public bool Not_EsActivo { get; set; }

        public bool Not_EsEliminado { get; set; }
        public int Not_UsuarioRegistra { get; set; }
        public DateTime Not_FechaRegistra { get; set; }
        public int? Not_UsuarioModifica { get; set; }
        public DateTime? Not_FechaModifica { get; set; }

        public virtual tbMaterias Mat { get; set; }
        public virtual tbUsuarios Not_UsuarioModificaNavigation { get; set; }
        public virtual tbUsuarios Not_UsuarioRegistraNavigation { get; set; }
        public virtual tbParciales Pac { get; set; }
        public virtual tbSemestres Sem { get; set; }
    }
}
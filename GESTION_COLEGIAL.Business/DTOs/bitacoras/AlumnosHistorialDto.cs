#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbAlumnosHistorial
    {
        public int AluHistorialId { get; set; }

        /// <summary>
        /// Indica el identificador único de la tbPersonas.
        /// </summary>
        public int Per_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbNivelesEducativos.
        /// </summary>
        public int? Niv_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbCursoNiveles.
        /// </summary>
        public int? Cun_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de la tbModalidades.
        /// </summary>
        public int? Mda_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbCursos.
        /// </summary>
        public int Cur_Id { get; set; }

        /// <summary>
        /// Identificador unico de tbSecciones.
        /// </summary>
        public int? Sec_Id { get; set; }

        /// <summary>
        /// Indica el identificador único de tbEstados.
        /// </summary>
        public int Est_Id { get; set; }

        public int? AnioCursado { get; set; }
        public decimal? PromedioAnual { get; set; }
    }
}
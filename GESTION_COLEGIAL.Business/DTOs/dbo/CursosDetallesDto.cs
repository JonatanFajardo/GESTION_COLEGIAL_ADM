

using System;

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class CursoDetalleDto
    {
        public int CursoDetalleId { get; set; }

        /// <summary>
        /// Indica el identificador único de tbNivelesEducativos.
        /// </summary>
        public int? NivelId { get; set; }

        /// <summary>
        /// Indica el identificador único de tbCursoNiveles.
        /// </summary>
        public int? CursoNivelId { get; set; }

        /// <summary>
        /// Indica el identificador único de la tbModalidades.
        /// </summary>
        public int? ModalidadId { get; set; }

        /// <summary>
        /// Indica el identificador único de tbCursos.
        /// </summary>
        public int CursoId { get; set; }

        /// <summary>
        /// Identificador unico de tbSecciones.
        /// </summary>
        public int? SeccionId { get; set; }
    }
}
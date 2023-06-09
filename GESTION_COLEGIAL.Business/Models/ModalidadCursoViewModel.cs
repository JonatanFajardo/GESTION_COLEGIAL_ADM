namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de una modalidad y curso.
    /// </summary>
    internal partial class ModalidadCursoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del curso.
        /// </summary>
        public int Cur_Id { get; set; }
        /// <summary>
        /// Obtiene o establece el ID de la modalidad.
        /// </summary>
        public int Mda_Id { get; set; }
    }
}
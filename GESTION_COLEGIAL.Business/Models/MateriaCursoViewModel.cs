namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de una materia y curso.
    /// </summary>
    internal partial class MateriaCursoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del curso.
        /// </summary>
        public int Cur_Id { get; set; }
        /// <summary>
        /// Obtiene o establece el ID de la materia.
        /// </summary>
        public int Mat_Id { get; set; }
    }
}
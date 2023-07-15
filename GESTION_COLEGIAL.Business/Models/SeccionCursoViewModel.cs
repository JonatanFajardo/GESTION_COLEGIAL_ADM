namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Representa el modelo de vista de una secci�n y curso.
    /// </summary>
    internal partial class SeccionCursoViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del curso.
        /// </summary>
        public int Cur_Id { get; set; }
        /// <summary>
        /// Obtiene o establece el ID de la secci�n.
        /// </summary>
        public int Sec_Id { get; set; }
    }
}
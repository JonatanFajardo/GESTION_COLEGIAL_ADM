namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Clase que representa el modelo de vista de un elemento desplegable de curso nivel.
    /// </summary>
    public class CursoNivelDropViewModel
    {
        /// <summary>
        /// Obtiene o establece el ID del curso nivel.
        /// </summary>
        public int Cun_Id { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del curso nivel.
        /// </summary>
        public string Cun_Descripcion { get; set; }
    }
}
#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    /// <summary>
    /// Registra los datos del representante del alumno.
    /// </summary>
    public partial class tbEncargados
    {
        public tbEncargados()
        {
            tbEncargados_tbAlumnos = new HashSet<tbEncargados_tbAlumnos>();
        }

        /// <summary>
        /// Identificador único de un encargado.
        /// </summary>
        public int Enc_Id { get; set; }

        /// <summary>
        /// Identificador de la persona.
        /// </summary>
        public int Per_Id { get; set; }

        /// <summary>
        /// Trabajo que desempeña el padre del alumno.
        /// </summary>
        public string Enc_Ocupacion { get; set; }

        public virtual tbPersonas Per { get; set; }
        public virtual ICollection<tbEncargados_tbAlumnos> tbEncargados_tbAlumnos { get; set; }
    }
}
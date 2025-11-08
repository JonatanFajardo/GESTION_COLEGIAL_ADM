#nullable disable

namespace GESTION_COLEGIAL.Business.DTOs
{
    public partial class tbCursos_tbSecciones
    {
        public int Cur_Id { get; set; }
        public int Sec_Id { get; set; }
        public int Aul_Id { get; set; }

        public virtual tbAulas Aul { get; set; }
        public virtual tbCursos Cur { get; set; }
        public virtual tbSecciones Sec { get; set; }
    }
}
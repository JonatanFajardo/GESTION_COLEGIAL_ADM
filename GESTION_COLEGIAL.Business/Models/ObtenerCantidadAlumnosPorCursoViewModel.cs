using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Models
{
    public  class ObtenerCantidadAlumnosPorCursoViewModel : BaseViewModel
    {
        public string Cur_Nombre { get; set; }
        public int CantidadAlumnos { get; set; }
    }
}

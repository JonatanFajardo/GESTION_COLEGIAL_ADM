using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Models
{
    public class ObtenerPromedioCursoUltimosAniosViewModel:BaseViewModel
    {
        public string Curso { get; set; }
        public int AnioCursado { get; set; }
        public decimal PromedioAnual { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Models
{
    public  class HomeAndChartsViewModel: BaseViewModel
    {
        public string NombreCurso { get; set; }
        public int PorcentajeDiferencia { get; set; }
    }
}

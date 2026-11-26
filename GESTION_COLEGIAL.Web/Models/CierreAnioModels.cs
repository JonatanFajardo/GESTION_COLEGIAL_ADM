using GESTION_COLEGIAL.Business.Models;
using System.Collections.Generic;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Pantalla "Cierre del año": años abiertos o ya cerrados, resumen del elegido, paso y permisos.</summary>
    public class CierreAnioIndexViewModel
    {
        public List<AnioLectivoViewModel> Anios { get; set; } = new List<AnioLectivoViewModel>();
        public CierreAnioResumenViewModel Resumen { get; set; }
        public int Paso { get; set; } = 1;
        public int PasoMaximo { get; set; } = 1;
        public bool PuedeCerrar { get; set; }
        public bool PuedeReabrir { get; set; }

        /// <summary>Se pidió un año que no existe o no está abierto ni cerrado: se muestra el año de trabajo.</summary>
        public bool AnioNoEncontrado { get; set; }

        /// <summary>Pantallas a las que la revisión enlaza: solo se muestra el enlace a quien puede abrirlas.</summary>
        public bool VeParciales { get; set; }
        public bool VeInscripcion { get; set; }
        public bool VeAnios { get; set; }

        /// <summary>Con el año en cierre se puede escribir (calcular, recuperaciones, decisiones).</summary>
        public bool Editable => PuedeCerrar && Resumen != null && Resumen.Ani_Estado == 4;
    }
}

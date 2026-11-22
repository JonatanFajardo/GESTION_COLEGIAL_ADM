using GESTION_COLEGIAL.Business.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Pantalla "Lista de espera": año elegido, permisos y combos de filtro.</summary>
    public class ListaEsperaIndexViewModel
    {
        public List<AspiranteAnioViewModel> Anios { get; set; } = new List<AspiranteAnioViewModel>();
        public int AniId { get; set; }
        public bool PuedeGestionar { get; set; }
        public bool PuedeMatricular { get; set; }
        public IEnumerable<SelectListItem> Niveles { get; set; } = Enumerable.Empty<SelectListItem>();

        public AspiranteAnioViewModel Anio => Anios.FirstOrDefault(a => a.Ani_Id == AniId);
        public bool AdmiteRegistro => Anio?.AdmiteRegistro == true;
        public bool EsAnioActivo => Anio?.EsActivo == true;
    }

    /// <summary>Pantalla completa "Registro de aspirante" (nuevo o edición mientras siga abierto).</summary>
    public class AspiranteRegistroPaginaViewModel
    {
        public AspiranteViewModel Aspirante { get; set; }
        public List<AspiranteAnioViewModel> Anios { get; set; } = new List<AspiranteAnioViewModel>();
        public int AniId { get; set; }
        public IEnumerable<SelectListItem> Niveles { get; set; } = Enumerable.Empty<SelectListItem>();
        public List<ParentescoViewModel> Parentescos { get; set; } = new List<ParentescoViewModel>();
        public string Hoy { get; set; }

        public bool EsNuevo => Aspirante == null || Aspirante.Asp_Id == 0;
        public bool TieneOferta => Aspirante?.Asp_Estado == "OF";
    }

    /// <summary>Detalle del aspirante: datos, lugar en la cola, cupo, historial y acciones según el estado.</summary>
    public class AspirantePaginaViewModel
    {
        public AspiranteViewModel Aspirante { get; set; }
        public List<AspiranteHistorialViewModel> Historial { get; set; } = new List<AspiranteHistorialViewModel>();
        public bool PuedeGestionar { get; set; }
        public bool PuedeMatricular { get; set; }
        public string Aviso { get; set; }
    }
}

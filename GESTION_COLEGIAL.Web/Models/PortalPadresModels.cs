using GESTION_COLEGIAL.Business.Models;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Una página del portal de padres: el encargado, sus hijos, el hijo elegido y lo de la sección.</summary>
    public class PortalPaginaViewModel
    {
        public string Seccion { get; set; } = "inicio";
        public PortalEncargadoViewModel Encargado { get; set; }
        public List<PortalHijoViewModel> Hijos { get; set; } = new();
        public PortalHijoViewModel Hijo { get; set; }
        public string Aviso { get; set; }
        public string Error { get; set; }

        public List<PortalNotaViewModel> Notas { get; set; } = new();
        public List<PortalAsistenciaViewModel> Asistencia { get; set; } = new();
        public List<PortalCargoViewModel> Cargos { get; set; } = new();
        public List<PortalEventoViewModel> Eventos { get; set; } = new();
        public List<PortalAnuncioViewModel> Anuncios { get; set; } = new();
        public List<PortalSolicitudViewModel> Solicitudes { get; set; } = new();

        /// <summary>Primer día del mes que muestra el calendario.</summary>
        public DateTime Mes { get; set; }

        /// <summary>Valores iniciales del formulario de nueva solicitud (p. ej. "justificar" con la fecha de la falta).</summary>
        public PortalSolicitudCrearViewModel Nueva { get; set; } = new();
    }

    /// <summary>Página de Invitaciones al portal (Secretaría).</summary>
    public class InvitacionesPortalIndexViewModel
    {
        public List<PortalInvitacionFilaViewModel> Filas { get; set; } = new();
        public PortalInvitacionesResumenViewModel Resumen { get; set; } = new();
        public string Texto { get; set; }
        public string Estado { get; set; }
        public bool Todos { get; set; }
        public string Aviso { get; set; }
        public string Error { get; set; }
        /// <summary>Enlace recién generado (se muestra una sola vez para copiarlo).</summary>
        public string Enlace { get; set; }
        public string EnlaceNombre { get; set; }
        public string EnlaceVence { get; set; }
    }

    /// <summary>Bandeja de solicitudes del portal.</summary>
    public class SolicitudesPortalIndexViewModel
    {
        public List<PortalBandejaFilaViewModel> Filas { get; set; } = new();
        public PortalBandejaResumenViewModel Resumen { get; set; } = new();
        public string Estado { get; set; }
        public string Tipo { get; set; }
        public string Texto { get; set; }
    }

    public class SolicitudesPortalVerViewModel
    {
        public PortalBandejaDetalleViewModel Solicitud { get; set; }
        public List<PortalBandejaHistorialViewModel> Historial { get; set; } = new();
        public bool PuedeEmitirConstancias { get; set; }
        public string Aviso { get; set; }
        public string Error { get; set; }
        public string EstadoElegido { get; set; }
        public string Respuesta { get; set; }
    }

    /// <summary>Página pública para activar la cuenta con el enlace de invitación.</summary>
    public class InvitacionActivarViewModel
    {
        public string Codigo { get; set; }
        public PortalInvitacionInfoViewModel Info { get; set; } = new();
        public string Usu_Name { get; set; }
        public string Error { get; set; }
    }
}

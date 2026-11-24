using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Encargado ligado al usuario del portal.</summary>
    public class PortalEncargadoViewModel
    {
        public int Enc_Id { get; set; }
        public string Enc_Nombre { get; set; }
        public string Enc_PrimerNombre { get; set; }
        public string Enc_Correo { get; set; }
        public string Enc_Telefono { get; set; }
    }

    /// <summary>Un hijo con el resumen de su tarjeta de inicio.</summary>
    public class PortalHijoViewModel
    {
        public int Alu_Id { get; set; }
        public string Alu_Nombre { get; set; }
        public string Alu_PrimerNombre { get; set; }
        public string Alu_Imagen { get; set; }
        public int? Anio { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public string Curso { get; set; }
        public string Parentesco { get; set; }
        public string Estado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public decimal SaldoVencido { get; set; }
        public int Ausencias { get; set; }
        public int Tardanzas { get; set; }
        public int RegistrosAsistencia { get; set; }
        public decimal? PromedioPublicado { get; set; }
        public int SolicitudesAbiertas { get; set; }

        public string Ubicacion => string.Join(" · ", new[] { Grado, string.IsNullOrWhiteSpace(Seccion) ? null : "Sección " + Seccion }
            .Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    public class PortalNotaViewModel
    {
        public int Mat_Id { get; set; }
        public string Mat_Nombre { get; set; }
        public int Sem_Id { get; set; }
        public string Sem_Descripcion { get; set; }
        public int Pac_Id { get; set; }
        public string Pac_Descripcion { get; set; }
        public decimal Nota { get; set; }
        public string Concepto { get; set; }
        public bool Aprueba { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public decimal? NotaMinima { get; set; }
    }

    public class PortalAsistenciaViewModel
    {
        public int Asi_Id { get; set; }
        public DateTime Asi_Fecha { get; set; }
        public string Asi_Estado { get; set; }
        public string Asi_Observacion { get; set; }
        public string Mat_Nombre { get; set; }
        public string HoraInicio { get; set; }
    }

    /// <summary>Cargo del estado de cuenta (misma forma que CuentasCobrar/EstadoCuenta).</summary>
    public class PortalCargoViewModel
    {
        public int Cco_Id { get; set; }
        public int Cco_Mes { get; set; }
        public int Cco_Anio { get; set; }
        public string Cpa_Descripcion { get; set; }
        public decimal Cco_MontoTotal { get; set; }
        public decimal Cco_MontoPendiente { get; set; }
        public decimal Cco_MontoDescuento { get; set; }
        public decimal Cco_MontoMora { get; set; }
        public DateTime Cco_FechaVencimiento { get; set; }
        public string Estado { get; set; }
        public decimal MontoPagado { get; set; }
    }

    public class PortalEventoViewModel
    {
        public int Eve_Id { get; set; }
        public string Eve_Titulo { get; set; }
        public string Eve_Descripcion { get; set; }
        public string Eve_Tipo { get; set; }
        public string Eve_Lugar { get; set; }
        public bool Eve_TodoElDia { get; set; }
        public DateTime Eve_Inicio { get; set; }
        public DateTime Eve_Fin { get; set; }
        public bool Eve_CierraColegio { get; set; }
    }

    public class PortalAnuncioViewModel
    {
        public int Anu_Id { get; set; }
        public string Anu_Titulo { get; set; }
        public string Anu_Contenido { get; set; }
        public DateTime? Anu_FechaPublicacion { get; set; }
        public DateTime? Anu_FechaExpiracion { get; set; }
    }

    /// <summary>Solicitud vista por la familia.</summary>
    public class PortalSolicitudViewModel
    {
        public int Sol_Id { get; set; }
        public int Alu_Id { get; set; }
        public string Alu_PrimerNombre { get; set; }
        public string Sol_Tipo { get; set; }
        public string Sol_Asunto { get; set; }
        public string Sol_Detalle { get; set; }
        public DateTime? Sol_FechaFalta { get; set; }
        public string Sol_Estado { get; set; }
        public string Sol_Respuesta { get; set; }
        public DateTime? Sol_FechaAtiende { get; set; }
        public DateTime Sol_FechaRegistra { get; set; }
        public bool Abierta => Sol_Estado == "nueva" || Sol_Estado == "en_proceso";
    }

    /// <summary>Formulario de nueva solicitud ("Usuario" lo pone el controller desde la sesión).</summary>
    public class PortalSolicitudCrearViewModel
    {
        public int? Usuario { get; set; }
        public int Alu_Id { get; set; }
        public string Tipo { get; set; }
        public string Asunto { get; set; }
        public string Detalle { get; set; }
        public DateTime? FechaFalta { get; set; }
    }

    public class PortalSolicitudRetirarViewModel
    {
        public int? Usuario { get; set; }
        public int Sol_Id { get; set; }
    }

    /// <summary>Respuesta de las escrituras del portal.</summary>
    public class PortalResultadoViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public string Estado { get; set; }
        public string Mensaje { get; set; }
    }

    // ── Bandeja ──
    public class PortalBandejaFilaViewModel
    {
        public int Sol_Id { get; set; }
        public string Sol_Tipo { get; set; }
        public string Sol_Asunto { get; set; }
        public DateTime? Sol_FechaFalta { get; set; }
        public string Sol_Estado { get; set; }
        public DateTime Sol_FechaRegistra { get; set; }
        public DateTime? Sol_FechaAtiende { get; set; }
        public int Alu_Id { get; set; }
        public string Alu_Nombre { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public int Enc_Id { get; set; }
        public string Enc_Nombre { get; set; }
        public string AtendidoPor { get; set; }
    }

    public class PortalBandejaResumenViewModel
    {
        public int? Nuevas { get; set; }
        public int? EnProceso { get; set; }
        public int? AtendidasMes { get; set; }
        public int? RechazadasMes { get; set; }
    }

    public class PortalBandejaDetalleViewModel : PortalBandejaFilaViewModel
    {
        public string Sol_Detalle { get; set; }
        public string Sol_Respuesta { get; set; }
        public string Enc_Telefono { get; set; }
        public string Enc_Correo { get; set; }
        public string Parentesco { get; set; }
        public string SolicitadoPor { get; set; }
    }

    public class PortalBandejaHistorialViewModel
    {
        public int Soh_Id { get; set; }
        public string Soh_EstadoAnterior { get; set; }
        public string Soh_EstadoNuevo { get; set; }
        public string Soh_Comentario { get; set; }
        public DateTime Soh_Fecha { get; set; }
        public string Usu_Name { get; set; }
        public bool EsFamilia { get; set; }
    }

    public class PortalBandejaCambiarViewModel
    {
        public int? Usuario { get; set; }
        public int Sol_Id { get; set; }
        public string Estado { get; set; }
        public string Respuesta { get; set; }
    }

    // ── Invitaciones ──
    public class PortalInvitacionFilaViewModel
    {
        public int Enc_Id { get; set; }
        public string Enc_Nombre { get; set; }
        public string Enc_Identidad { get; set; }
        public string Enc_Telefono { get; set; }
        public string Enc_Correo { get; set; }
        public int Hijos { get; set; }
        public string HijosNombres { get; set; }
        public int? Usu_Id { get; set; }
        public string Usu_Name { get; set; }
        public bool CuentaActiva { get; set; }
        public int? Inv_Id { get; set; }
        public DateTime? Inv_FechaRegistra { get; set; }
        public DateTime? Inv_Vence { get; set; }
        public string InvitadoPor { get; set; }
        public string InvEstado { get; set; }
        public string EstadoAcceso { get; set; }
    }

    public class PortalInvitacionesResumenViewModel
    {
        public int ConHijos { get; set; }
        public int? Activos { get; set; }
        public int? Pendientes { get; set; }
        public int? SinAcceso { get; set; }
    }

    public class PortalInvitacionAccionViewModel
    {
        public int? Usuario { get; set; }
        public int Enc_Id { get; set; }
        public bool Suspender { get; set; }
    }

    public class PortalInvitacionCreadaViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public string Codigo { get; set; }
        public DateTime? Vence { get; set; }
        public string Nombre { get; set; }
        public string Mensaje { get; set; }
    }

    /// <summary>Lo que dice un enlace de invitación (página pública).</summary>
    public class PortalInvitacionInfoViewModel
    {
        public string Estado { get; set; }
        public string Enc_Nombre { get; set; }
        public string Hijos { get; set; }
        public string UsuarioExistente { get; set; }
        public DateTime? Inv_Vence { get; set; }
    }

    public class PortalActivarViewModel
    {
        public string Token { get; set; }
        public string Usu_Name { get; set; }
        public string Contrasena { get; set; }
    }

    /// <summary>Nombres en español de tipos y estados (una sola fuente para vistas y controllers).</summary>
    public static class PortalTextos
    {
        public static readonly string[] Tipos = { "justificar", "cita", "constancia", "otro" };

        public static string Tipo(string tipo) => tipo switch
        {
            "justificar" => "Justificar una falta",
            "cita" => "Pedir una cita",
            "constancia" => "Pedir una constancia",
            "otro" => "Otra solicitud",
            _ => tipo ?? ""
        };

        public static string Estado(string estado) => estado switch
        {
            "nueva" => "Enviada",
            "en_proceso" => "En proceso",
            "atendida" => "Atendida",
            "rechazada" => "Rechazada",
            "retirada" => "Retirada",
            _ => estado ?? ""
        };

        /// <summary>Clase de insignia CORK para el estado de una solicitud.</summary>
        public static string EstadoBadge(string estado) => estado switch
        {
            "nueva" => "badge-warning",
            "en_proceso" => "badge-info",
            "atendida" => "badge-success",
            "rechazada" => "badge-danger",
            _ => "badge-secondary"
        };

        public static string Acceso(string estado) => estado switch
        {
            "activo" => "Con acceso",
            "suspendido" => "Suspendido",
            "pendiente" => "Invitación enviada",
            "vencida" => "Invitación vencida",
            "revocada" => "Invitación detenida",
            "sin_invitar" => "Sin invitar",
            _ => estado ?? ""
        };

        public static string AccesoBadge(string estado) => estado switch
        {
            "activo" => "badge-success",
            "suspendido" => "badge-danger",
            "pendiente" => "badge-info",
            "vencida" => "badge-warning",
            _ => "badge-secondary"
        };

        public static string Asistencia(string estado) => estado switch
        {
            "P" => "Presente",
            "A" => "Ausente",
            "T" => "Tardanza",
            "J" => "Justificada",
            _ => estado ?? ""
        };

        public static string AsistenciaBadge(string estado) => estado switch
        {
            "P" => "badge-success",
            "A" => "badge-danger",
            "T" => "badge-warning",
            "J" => "badge-info",
            _ => "badge-secondary"
        };
    }
}

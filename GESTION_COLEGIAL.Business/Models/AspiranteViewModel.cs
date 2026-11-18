using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Un estado o una prioridad de la lista de espera: código de la tabla, texto e insignia CORK.</summary>
    public class OpcionAspirante
    {
        public string Codigo { get; set; }
        public string Etiqueta { get; set; }
        public string Insignia { get; set; }
    }

    /// <summary>
    /// Catálogos de la lista de espera (espejo de Skuul AdmissionWaitlistStatus).
    /// EE En espera → OF Ofrecido → MA Matriculado; RE Rechazado y DE Desistió se pueden reactivar.
    /// </summary>
    public static class CatalogoAspirantes
    {
        public static readonly IReadOnlyList<OpcionAspirante> Estados = new List<OpcionAspirante>
        {
            new OpcionAspirante { Codigo = "EE", Etiqueta = "En espera",   Insignia = "badge-warning" },
            new OpcionAspirante { Codigo = "OF", Etiqueta = "Ofrecido",    Insignia = "badge-info" },
            new OpcionAspirante { Codigo = "MA", Etiqueta = "Matriculado", Insignia = "badge-success" },
            new OpcionAspirante { Codigo = "RE", Etiqueta = "Rechazado",   Insignia = "badge-danger" },
            new OpcionAspirante { Codigo = "DE", Etiqueta = "Desistió",    Insignia = "badge-secondary" },
        };

        /// <summary>0 normal · 1 media · 2 alta (orden de la cola: mayor primero).</summary>
        public static readonly IReadOnlyList<OpcionAspirante> Prioridades = new List<OpcionAspirante>
        {
            new OpcionAspirante { Codigo = "0", Etiqueta = "Normal", Insignia = "outline-badge-secondary" },
            new OpcionAspirante { Codigo = "1", Etiqueta = "Media",  Insignia = "outline-badge-warning" },
            new OpcionAspirante { Codigo = "2", Etiqueta = "Alta",   Insignia = "outline-badge-danger" },
        };

        public static string Estado(string codigo) => Estados.FirstOrDefault(x => x.Codigo == codigo)?.Etiqueta ?? codigo ?? "";
        public static string InsigniaEstado(string codigo) => Estados.FirstOrDefault(x => x.Codigo == codigo)?.Insignia ?? "badge-secondary";
        public static string Prioridad(int p) => Prioridades.FirstOrDefault(x => x.Codigo == p.ToString())?.Etiqueta ?? "Normal";
        public static string InsigniaPrioridad(int p) => Prioridades.FirstOrDefault(x => x.Codigo == p.ToString())?.Insignia ?? "outline-badge-secondary";
        public static bool EstaAbierto(string codigo) => codigo == "EE" || codigo == "OF";
        public static string Sexo(string s) => s == "F" ? "Femenino" : s == "M" ? "Masculino" : "";
    }

    /// <summary>Año para el selector de la lista de espera.</summary>
    public class AspiranteAnioViewModel
    {
        public int Ani_Id { get; set; }
        public int Ani_Anio { get; set; }
        public string Ani_Nombre { get; set; }
        public int Ani_Estado { get; set; }
        public bool EsActivo { get; set; }
        public bool EsPorDefecto { get; set; }
        public bool AdmiteRegistro { get; set; }
        public int Abiertos { get; set; }
    }

    /// <summary>Cupo y demanda de un grado (lo que mira Dirección). Cupo/Libres null = cupo sin definir.</summary>
    public class AspiranteResumenViewModel
    {
        public int Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }
        public int Cun_Id { get; set; }
        public string Cun_Descripcion { get; set; }
        public int Cun_Orden { get; set; }
        public int Secciones { get; set; }
        public int SeccionesConCupo { get; set; }
        public int? Cupo { get; set; }
        public int Matriculados { get; set; }
        public int Ofertas { get; set; }
        public bool CupoDefinido { get; set; }
        public int? Libres { get; set; }
        public int EnEspera { get; set; }
        public int MatriculadosLista { get; set; }
        public int Cerrados { get; set; }
        public int? SiguienteId { get; set; }
        public string SiguienteNombre { get; set; }
    }

    /// <summary>Un aspirante en la tabla. Los textos calculados también se buscan y se exportan.</summary>
    public class AspiranteListaViewModel
    {
        public int Asp_Id { get; set; }
        public int Ani_Id { get; set; }
        public string Asp_Nombres { get; set; }
        public string Asp_Apellidos { get; set; }
        public string Aspirante { get; set; }
        public string Asp_Identidad { get; set; }
        public DateTime Asp_FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string Asp_Sexo { get; set; }
        public string Asp_EscuelaProcedencia { get; set; }
        public string Asp_EncargadoNombre { get; set; }
        public int? Par_Id { get; set; }
        public string Par_Descripcion { get; set; }
        public string Asp_EncargadoTelefono { get; set; }
        public string Asp_EncargadoCorreo { get; set; }
        public int Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }
        public int Cun_Id { get; set; }
        public string Cun_Descripcion { get; set; }
        public int? Mda_Id { get; set; }
        public string Mda_Descripcion { get; set; }
        public int? Cur_Id { get; set; }
        public string Cur_Nombre { get; set; }
        public DateTime Asp_FechaSolicitud { get; set; }
        public string Asp_Estado { get; set; }
        public int Asp_Prioridad { get; set; }
        public string Asp_MotivoPrioridad { get; set; }
        public int Asp_Posicion { get; set; }
        public long? PosicionCola { get; set; }
        public bool EsSiguiente { get; set; }
        public bool CupoDefinido { get; set; }
        public int? Libres { get; set; }
        public bool PuedeOfrecer { get; set; }
        public DateTime? Asp_FechaOferta { get; set; }
        public DateTime? Asp_FechaDecision { get; set; }
        public string Asp_MotivoDecision { get; set; }
        public int? Alu_Id { get; set; }
        public int Cun_Orden { get; set; }

        public string EstadoTexto => CatalogoAspirantes.Estado(Asp_Estado);
        public string PrioridadTexto => CatalogoAspirantes.Prioridad(Asp_Prioridad);
        /// <summary>"1", "2"… en la cola; "Oferta" si tiene cupo ofrecido; vacío si está cerrado.</summary>
        public string Lugar => Asp_Estado == "EE" && PosicionCola.HasValue ? PosicionCola.Value.ToString()
                             : Asp_Estado == "OF" ? "Oferta" : "";
        public string Grado => Cun_Descripcion;
        /// <summary>Encargado y teléfono en una sola columna (también para buscar y exportar).</summary>
        public string Encargado => Asp_EncargadoNombre + " · " + Asp_EncargadoTelefono;
        public string Solicitud => Asp_FechaSolicitud.ToString("dd/MM/yyyy");
    }

    /// <summary>Un aspirante completo con su lugar en la cola y el cupo de su grado.</summary>
    public class AspiranteViewModel
    {
        public int Asp_Id { get; set; }
        public int Ani_Id { get; set; }
        public string Ani_Nombre { get; set; }
        public bool EsAnioActivo { get; set; }
        public string Asp_Nombres { get; set; }
        public string Asp_Apellidos { get; set; }
        public string Aspirante { get; set; }
        public string Asp_Identidad { get; set; }
        public DateTime Asp_FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string Asp_Sexo { get; set; }
        public string Asp_EscuelaProcedencia { get; set; }
        public string Asp_EncargadoNombre { get; set; }
        public string Asp_EncargadoIdentidad { get; set; }
        public int? Par_Id { get; set; }
        public string Par_Descripcion { get; set; }
        public string Asp_EncargadoTelefono { get; set; }
        public string Asp_EncargadoCorreo { get; set; }
        public int Niv_Id { get; set; }
        public string Niv_Descripcion { get; set; }
        public int Cun_Id { get; set; }
        public string Cun_Descripcion { get; set; }
        public int? Mda_Id { get; set; }
        public string Mda_Descripcion { get; set; }
        public int? Cur_Id { get; set; }
        public string Cur_Nombre { get; set; }
        public DateTime Asp_FechaSolicitud { get; set; }
        public string Asp_Estado { get; set; }
        public int Asp_Prioridad { get; set; }
        public string Asp_MotivoPrioridad { get; set; }
        public int Asp_Posicion { get; set; }
        public long? PosicionCola { get; set; }
        public int EnCola { get; set; }
        public int? SiguienteId { get; set; }
        public string SiguienteNombre { get; set; }
        public int Secciones { get; set; }
        public int SeccionesConCupo { get; set; }
        public int? Cupo { get; set; }
        public int Matriculados { get; set; }
        public int Ofertas { get; set; }
        public bool CupoDefinido { get; set; }
        public int? Libres { get; set; }
        public bool? CupoModalidadDefinido { get; set; }
        public int? LibresModalidad { get; set; }
        public bool PuedeOfrecer { get; set; }
        public string Asp_MotivoDecision { get; set; }
        public DateTime? Asp_FechaOferta { get; set; }
        public string UsuarioOferta { get; set; }
        public DateTime? Asp_FechaDecision { get; set; }
        public string UsuarioDecision { get; set; }
        public int? Alu_Id { get; set; }
        public string Asp_Notas { get; set; }
        public DateTime Asp_FechaRegistra { get; set; }
        public string UsuarioRegistra { get; set; }
        public DateTime? Asp_FechaModifica { get; set; }

        public bool EstaAbierto => CatalogoAspirantes.EstaAbierto(Asp_Estado);
    }

    public class AspiranteHistorialViewModel
    {
        public int Ash_Id { get; set; }
        public int Asp_Id { get; set; }
        public string Ash_EstadoAnterior { get; set; }
        public string Ash_EstadoNuevo { get; set; }
        public string Ash_Motivo { get; set; }
        public int? Ash_Usuario { get; set; }
        public string Usuario { get; set; }
        public DateTime Ash_Fecha { get; set; }
    }

    public class AspiranteDuplicadoViewModel
    {
        public int? AspiranteId { get; set; }
        public string AspiranteNombre { get; set; }
        public string AspiranteEstado { get; set; }
        public string AspiranteGrado { get; set; }
        public int? AlumnoId { get; set; }
        public string AlumnoNombre { get; set; }
        public bool AlumnoMatriculadoEnAnio { get; set; }
    }

    public class AspiranteCursoViewModel
    {
        public int Cur_Id { get; set; }
        public string Cur_Nombre { get; set; }
    }

    /// <summary>Lo que manda el formulario "Registro de aspirante" (Asp_Id 0 = nuevo).</summary>
    public class AspiranteGuardarViewModel
    {
        public int Asp_Id { get; set; }
        public int Ani_Id { get; set; }
        public string Asp_Nombres { get; set; }
        public string Asp_Apellidos { get; set; }
        public string Asp_Identidad { get; set; }
        public DateTime? Asp_FechaNacimiento { get; set; }
        public string Asp_Sexo { get; set; }
        public string Asp_EscuelaProcedencia { get; set; }
        public string Asp_EncargadoNombre { get; set; }
        public string Asp_EncargadoIdentidad { get; set; }
        public int? Par_Id { get; set; }
        public string Asp_EncargadoTelefono { get; set; }
        public string Asp_EncargadoCorreo { get; set; }
        public int Niv_Id { get; set; }
        public int Cun_Id { get; set; }
        public int? Mda_Id { get; set; }
        public int? Cur_Id { get; set; }
        public DateTime? Asp_FechaSolicitud { get; set; }
        public int Asp_Prioridad { get; set; }
        public string Asp_MotivoPrioridad { get; set; }
        public string Asp_Notas { get; set; }
        public int? Usuario { get; set; }
    }

    /// <summary>ofrecer | rechazar | desistio | reactivar.</summary>
    public class AspiranteEstadoViewModel
    {
        public int Asp_Id { get; set; }
        public string Accion { get; set; }
        public string Motivo { get; set; }
        public int? Usuario { get; set; }
    }

    public class AspiranteMatricularViewModel
    {
        public int Asp_Id { get; set; }
        public int Alu_Id { get; set; }
        public int? Usuario { get; set; }
    }

    /// <summary>Respuesta de las escrituras de la API Aspirantes.</summary>
    public class AspiranteResultadoViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public string Estado { get; set; }
        public int Posicion { get; set; }
        public long? PosicionCola { get; set; }
        public int EnCola { get; set; }
        public string Grado { get; set; }
        public int? AluId { get; set; }
        public string Aviso { get; set; }
        public string Mensaje { get; set; }
    }
}

using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GESTION_COLEGIAL.UI.Models
{
    /// <summary>Un tipo de documento: código de la API, nombres y explicación corta.</summary>
    public class TipoDocumento
    {
        public string Codigo { get; set; }
        public string Etiqueta { get; set; }
        public string Titulo { get; set; }
        public string Explicacion { get; set; }
        public string Icono { get; set; }
        public bool PideAnio { get; set; }
        public bool PideParcial { get; set; }
    }

    /// <summary>Tipos de documento, formatos y mensajes de la API.</summary>
    public static class CatalogoDocumentos
    {
        public const string Estudio = "ESTUDIO";
        public const string Boletin = "BOLETIN";
        public const string Expediente = "EXPEDIENTE";

        public static readonly CultureInfo Cultura = new CultureInfo("es-HN");

        public static readonly IReadOnlyList<TipoDocumento> Tipos = new List<TipoDocumento>
        {
            new TipoDocumento
            {
                Codigo = Estudio, Etiqueta = "Constancia de estudio", Titulo = "Constancia de estudio",
                Explicacion = "Hace constar que el alumno está matriculado en un año lectivo.",
                Icono = "bi bi-patch-check", PideAnio = true
            },
            new TipoDocumento
            {
                Codigo = Boletin, Etiqueta = "Boletín", Titulo = "Boletín de calificaciones",
                Explicacion = "Las notas de un año: todos los parciales o uno solo.",
                Icono = "bi bi-card-checklist", PideAnio = true, PideParcial = true
            },
            new TipoDocumento
            {
                Codigo = Expediente, Etiqueta = "Expediente", Titulo = "Expediente académico",
                Explicacion = "El historial de notas de todos los años que cursó en el colegio.",
                Icono = "bi bi-journal-text"
            }
        };

        public static bool Existe(string codigo) => Tipos.Any(t => t.Codigo == codigo);

        public static TipoDocumento Tipo(string codigo) =>
            Tipos.FirstOrDefault(t => t.Codigo == codigo)
            ?? new TipoDocumento { Codigo = codigo, Etiqueta = codigo, Titulo = "Documento", Icono = "bi bi-file-earmark-text" };

        /// <summary>Normaliza el tipo que llega por la dirección (estudio, Boletin…); null si no vale.</summary>
        public static string Normalizar(string tipo)
        {
            string t = (tipo ?? "").Trim().ToUpperInvariant();
            return Existe(t) ? t : null;
        }

        /// <summary>Mensaje en español para los códigos negativos de la API.</summary>
        public static string Mensaje(int codigo, string tipo) => codigo switch
        {
            -1 => tipo == Expediente
                ? "El alumno no tiene notas registradas en ningún año. No hay nada que poner en el expediente."
                : "El alumno no tiene notas registradas para lo que eligió (año o parcial). Revise el cuaderno de notas.",
            -2 => "El alumno no está matriculado en ese año lectivo. Elija un año en el que haya estado inscrito.",
            -3 => "Faltan datos o no son válidos. Revise el tipo, el alumno y el año.",
            -4 => "Ya se emitió este documento antes. Para emitir una revisión nueva, escriba el motivo de la corrección.",
            _ => "No se pudo emitir el documento. Intente de nuevo en un momento."
        };

        public static string Fecha(DateTime d) => d.ToString("d MMM yyyy", Cultura).Replace(".", "");
        public static string FechaHora(DateTime d) => d.ToString("d MMM yyyy, h:mm tt", Cultura).Replace(".", "");

        /// <summary>"a los 12 días del mes de noviembre de 2026".</summary>
        public static string FechaLarga(DateTime d) =>
            $"a los {d.Day} {(d.Day == 1 ? "día" : "días")} del mes de {d.ToString("MMMM", Cultura)} de {d.Year}";

        public static string Nota(decimal? n) => n.HasValue ? n.Value.ToString("0.##", Cultura) : "—";
        public static string Promedio(decimal? n) => n.HasValue ? n.Value.ToString("0.0", Cultura) : "—";

        /// <summary>Primer tramo de la dirección ("San Pedro Sula, Cortés…" → "San Pedro Sula").</summary>
        public static string Ciudad(string direccion)
        {
            string c = (direccion ?? "").Split(',')[0].Trim();
            return string.IsNullOrEmpty(c) ? "Honduras" : c;
        }
    }

    /// <summary>Una materia del boletín: nota por parcial y promedio.</summary>
    public class FilaBoletin
    {
        public string Materia { get; set; }
        public Dictionary<string, decimal?> Notas { get; set; } = new Dictionary<string, decimal?>();
        public decimal? Promedio { get; set; }
    }

    /// <summary>Un año del expediente: grado, sección, materias con su promedio y promedio del año.</summary>
    public class AnioExpediente
    {
        public int? Anio { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public List<KeyValuePair<string, decimal?>> Materias { get; set; } = new List<KeyValuePair<string, decimal?>>();
        public decimal? Promedio { get; set; }
    }

    /// <summary>
    /// Lo que se dibuja en el papel. Lo usan el documento emitido y la vista previa (mismo partial),
    /// así la vista previa es igual al documento final.
    /// </summary>
    public class DocumentoImpreso
    {
        public string Tipo { get; set; }
        public TipoDocumento TipoInfo => CatalogoDocumentos.Tipo(Tipo);
        public bool EsVistaPrevia { get; set; }
        public string Referencia { get; set; }
        public int Revision { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaEmision { get; set; }
        public string EmitidoPor { get; set; }
        public string Institucion { get; set; }
        public string Direccion { get; set; }
        public string Ciudad => CatalogoDocumentos.Ciudad(Direccion);
        public DocumentoContenidoAlumno Alumno { get; set; } = new DocumentoContenidoAlumno();
        public int? Anio { get; set; }
        public string AnioNombre { get; set; }
        public string Parcial { get; set; }

        // Boletín
        public List<string> Parciales { get; set; } = new List<string>();
        public List<FilaBoletin> Filas { get; set; } = new List<FilaBoletin>();

        // Expediente
        public List<AnioExpediente> Anios { get; set; } = new List<AnioExpediente>();

        public decimal? PromedioGeneral { get; set; }

        private static decimal? Media(IEnumerable<decimal?> valores)
        {
            var v = valores.Where(x => x.HasValue).Select(x => x.Value).ToList();
            return v.Count == 0 ? (decimal?)null : Math.Round(v.Average(), 2);
        }

        private static string SemestreCorto(string s) => (s ?? "").Trim().ToLowerInvariant() switch
        {
            "primer" or "primero" or "i" or "1" => "I Sem",
            "segundo" or "ii" or "2" => "II Sem",
            "" => null,
            _ => s.Trim()
        };

        private static string ParcialCorto(string p) => (p ?? "").Trim().ToLowerInvariant() switch
        {
            "primero" or "primer" or "1" => "1er P",
            "segundo" or "2" => "2do P",
            "tercero" or "tercer" or "3" => "3er P",
            "cuarto" or "4" => "4to P",
            "" => "Parcial",
            _ => p.Trim()
        };

        /// <summary>Columna de una nota: semestre + parcial ("I Sem · 1er P"); sin semestre, solo el parcial.</summary>
        private static string Columna(DocumentoContenidoNota n)
        {
            string sem = SemestreCorto(n.Semestre) ?? (n.Sem_Id.HasValue ? "Sem " + n.Sem_Id.Value : null);
            string par = ParcialCorto(n.Parcial);
            return sem == null ? par : sem + " · " + par;
        }

        /// <summary>Arma el documento a partir del contenido congelado: agrupa y calcula los promedios.</summary>
        public static DocumentoImpreso Desde(string tipo, DocumentoContenido contenido)
        {
            var d = new DocumentoImpreso { Tipo = tipo };
            if (contenido == null) return d;
            d.Alumno = contenido.Alumno ?? new DocumentoContenidoAlumno();
            d.Anio = contenido.Anio;
            d.Parcial = contenido.Parcial;
            var notas = contenido.Notas ?? new List<DocumentoContenidoNota>();

            if (tipo == CatalogoDocumentos.Boletin)
            {
                // Columnas: semestre × parcial ("I Sem · 1er P"…), en orden.
                d.Parciales = notas
                    .GroupBy(Columna)
                    .OrderBy(g => g.Min(n => n.Sem_Id ?? int.MaxValue))
                    .ThenBy(g => g.Min(n => n.Pac_Id ?? int.MaxValue))
                    .Select(g => g.Key)
                    .ToList();
                d.Filas = notas
                    .GroupBy(n => n.Materia ?? "")
                    .OrderBy(g => g.Key, StringComparer.Create(CatalogoDocumentos.Cultura, true))
                    .Select(g => new FilaBoletin
                    {
                        Materia = g.Key,
                        Notas = g.GroupBy(Columna).ToDictionary(p => p.Key, p => Media(p.Select(n => n.Nota)))
                    })
                    .ToList();
                // Promedio de la materia = promedio de sus parciales (de los dos semestres).
                foreach (var f in d.Filas)
                    f.Promedio = Media(f.Notas.Values);
                d.PromedioGeneral = Media(d.Filas.Select(f => f.Promedio));
            }
            else if (tipo == CatalogoDocumentos.Expediente)
            {
                d.Anios = notas
                    .GroupBy(n => n.Anio)
                    .OrderBy(g => g.Key ?? 0)
                    .Select(g =>
                    {
                        var materias = g
                            .GroupBy(n => n.Materia ?? "")
                            .OrderBy(m => m.Key, StringComparer.Create(CatalogoDocumentos.Cultura, true))
                            // Promedio de la materia = promedio de sus parciales.
                            .Select(m => new KeyValuePair<string, decimal?>(m.Key,
                                Media(m.GroupBy(Columna).Select(p => Media(p.Select(n => n.Nota))))))
                            .ToList();
                        var primera = g.FirstOrDefault(n => !string.IsNullOrWhiteSpace(n.Grado)) ?? g.First();
                        return new AnioExpediente
                        {
                            Anio = g.Key,
                            Grado = primera.Grado,
                            Seccion = primera.Seccion,
                            Materias = materias,
                            Promedio = Media(materias.Select(m => m.Value))
                        };
                    })
                    .ToList();
                // Promedio general = promedio de todas las materias de todos los años.
                d.PromedioGeneral = Media(d.Anios.SelectMany(a => a.Materias.Select(m => m.Value)));
            }
            return d;
        }
    }

    /// <summary>Pantalla "Constancias y expediente": filtros y tabla de documentos emitidos.</summary>
    public class ConstanciasIndexViewModel
    {
        public const int PorPagina = 25;

        public string Tipo { get; set; }
        public int? AniId { get; set; }
        public int? AluId { get; set; }
        public string Buscar { get; set; }
        public int Pagina { get; set; } = 1;
        public bool PuedeEmitir { get; set; }
        public string AlumnoFiltro { get; set; }
        public string Aviso { get; set; }
        public List<AnioLectivoViewModel> Anios { get; set; } = new List<AnioLectivoViewModel>();
        public List<DocumentoEmitidoViewModel> Documentos { get; set; } = new List<DocumentoEmitidoViewModel>();
        public int Total { get; set; }

        public bool HayFiltros => !string.IsNullOrEmpty(Tipo) || AniId.HasValue || AluId.HasValue || !string.IsNullOrWhiteSpace(Buscar);
        public int Paginas => Math.Max(1, (int)Math.Ceiling(Total / (double)PorPagina));
        public int Desde => Total == 0 ? 0 : (Pagina - 1) * PorPagina + 1;
        public int Hasta => Math.Min(Total, Pagina * PorPagina);
    }

    /// <summary>Pantalla "Emitir un documento".</summary>
    public class ConstanciaEmitirPaginaViewModel
    {
        public string Tipo { get; set; }
        public int? AluId { get; set; }
        public List<CasoAlumnoViewModel> Alumnos { get; set; } = new List<CasoAlumnoViewModel>();
        public List<AnioLectivoViewModel> Anios { get; set; } = new List<AnioLectivoViewModel>();
        public List<ParcialViewModel> Parciales { get; set; } = new List<ParcialViewModel>();
        public int? AniIdSugerido { get; set; }
        public int? PacId { get; set; }
    }

    /// <summary>Lo que llega del formulario Emitir (vista previa y emisión).</summary>
    public class ConstanciaSolicitudViewModel
    {
        public string Tipo { get; set; }
        public int AluId { get; set; }
        public int? AniId { get; set; }
        public int? PacId { get; set; }
        public string Motivo { get; set; }
    }

    /// <summary>Pantalla "Ver" de un documento emitido.</summary>
    public class ConstanciaVerViewModel
    {
        public DocumentoEmitidoViewModel Documento { get; set; }
        public DocumentoImpreso Impreso { get; set; }
        public List<DocumentoEmitidoViewModel> Revisiones { get; set; } = new List<DocumentoEmitidoViewModel>();
        public DocumentoEmitidoViewModel Ultima { get; set; }
        public bool PuedeEmitir { get; set; }
        public string Aviso { get; set; }
        public bool ContenidoIlegible { get; set; }
    }
}

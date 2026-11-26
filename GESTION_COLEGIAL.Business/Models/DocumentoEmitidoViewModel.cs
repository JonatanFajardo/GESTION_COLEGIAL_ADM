using System;
using System.Collections.Generic;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>
    /// Documento oficial congelado de un alumno (API DocumentosEmitidos): constancia de estudio,
    /// boletín de un año o expediente de todos los años. No se edita ni se borra:
    /// para corregirlo se emite una revisión nueva (con motivo).
    /// </summary>
    public class DocumentoEmitidoViewModel
    {
        public int Doc_Id { get; set; }

        /// <summary>ESTUDIO, BOLETIN o EXPEDIENTE.</summary>
        public string Doc_Tipo { get; set; }

        /// <summary>CE-26-000001, BO-26-000001, EX-26-000001.</summary>
        public string Doc_Referencia { get; set; }

        public int Doc_Revision { get; set; }
        public int Alu_Id { get; set; }
        public string Alu_Nombre { get; set; }
        public string Alu_Identidad { get; set; }
        public int? Ani_Id { get; set; }
        public string Ani_Nombre { get; set; }
        public int? Pac_Id { get; set; }
        public string Pac_Descripcion { get; set; }
        public decimal? Doc_Promedio { get; set; }
        public string Doc_Motivo { get; set; }
        public DateTime Doc_FechaEmision { get; set; }
        public string Doc_EmitidoPor { get; set; }
        public bool Doc_EsUltima { get; set; }

        /// <summary>Contenido congelado (JSON). Solo viene en FindAsync.</summary>
        public string Doc_Contenido { get; set; }
    }

    /// <summary>Respuesta de VistaPreviaAsync: lo que se emitiría ahora, sin guardarlo.</summary>
    public class DocumentoVistaPreviaViewModel
    {
        /// <summary>0 o más = se puede emitir; negativo = motivo por el que no (ver códigos de EmitirAsync).</summary>
        public int Codigo { get; set; }
        public string Doc_Contenido { get; set; }
        public decimal? Doc_Promedio { get; set; }

        /// <summary>1 si es la primera emisión; más de 1 = será una revisión y exige motivo.</summary>
        public int RevisionSiguiente { get; set; }
    }

    /// <summary>Cuerpo de EmitirAsync.</summary>
    public class DocumentoEmitirViewModel
    {
        public string Doc_Tipo { get; set; }
        public int Alu_Id { get; set; }
        public int? Ani_Id { get; set; }
        public int? Pac_Id { get; set; }
        public string Doc_Motivo { get; set; }
        public int Doc_UsuarioEmite { get; set; }
    }

    /// <summary>Doc_Contenido deserializado.</summary>
    public class DocumentoContenido
    {
        public DocumentoContenidoAlumno Alumno { get; set; }
        public int? Anio { get; set; }
        public string Parcial { get; set; }
        public List<DocumentoContenidoNota> Notas { get; set; } = new List<DocumentoContenidoNota>();

        /// <summary>Notas finales del cierre del año (solo años con la decisión del alumno confirmada).</summary>
        public List<DocumentoContenidoFinal> Finales { get; set; } = new List<DocumentoContenidoFinal>();

        /// <summary>Promedio y decisión de cada año con el cierre confirmado.</summary>
        public List<DocumentoContenidoResultado> Resultados { get; set; } = new List<DocumentoContenidoResultado>();
    }

    /// <summary>Nota final de una materia en el cierre del año (con la recuperación ya aplicada).</summary>
    public class DocumentoContenidoFinal
    {
        public int? Anio { get; set; }
        public string Materia { get; set; }
        public decimal? NotaFinal { get; set; }

        /// <summary>"aprobado" / "reprobado" / "sin_notas".</summary>
        public string Resultado { get; set; }
        public decimal? NotaRecuperacion { get; set; }
        public bool? AproboRecuperacion { get; set; }
    }

    /// <summary>Resultado de un año en el cierre: promedio y decisión.</summary>
    public class DocumentoContenidoResultado
    {
        public int? Anio { get; set; }
        public decimal? Promedio { get; set; }

        /// <summary>"promovido" / "repite" / "gradua".</summary>
        public string Decision { get; set; }

        /// <summary>Aprobó el último grado de su nivel sin ser el último de su carrera (Noveno).</summary>
        public bool TerminaNivel { get; set; }
        public string Nivel { get; set; }
    }

    public class DocumentoContenidoAlumno
    {
        public int Alu_Id { get; set; }
        public string Nombre { get; set; }
        public string Identidad { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public string Curso { get; set; }
        public string Seccion { get; set; }
        public string Modalidad { get; set; }
    }

    public class DocumentoContenidoNota
    {
        public int? Anio { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public string Materia { get; set; }
        public int? Sem_Id { get; set; }

        /// <summary>"Primer" / "Segundo".</summary>
        public string Semestre { get; set; }
        public int? Pac_Id { get; set; }
        public string Parcial { get; set; }
        public decimal? Nota { get; set; }
    }
}

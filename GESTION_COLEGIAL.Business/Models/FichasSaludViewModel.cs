using System;
using System.Collections.Generic;
using System.Linq;

namespace GESTION_COLEGIAL.Business.Models
{
    /// <summary>Un alumno activo en el directorio de fichas de salud (API FichasSalud/DirectorioAsync).</summary>
    public class FichaSaludDirectorioViewModel
    {
        public int Alu_Id { get; set; }
        public string Alumno { get; set; }
        public string Identidad { get; set; }
        public string Foto { get; set; }
        public int? Niv_Id { get; set; }
        public string Nivel { get; set; }
        public int? Cun_Id { get; set; }
        public string Grado { get; set; }
        public int? Sec_Id { get; set; }
        public string Seccion { get; set; }
        public bool TieneFicha { get; set; }
        public bool TieneAlerta { get; set; }
        public string Alerta { get; set; }
        public string TipoSangre { get; set; }
        public string Encargado { get; set; }
        public string EncargadoTelefono { get; set; }
        public string ContactoNombre { get; set; }
        public string ContactoTelefono { get; set; }
        public bool TieneTelefono { get; set; }
        public DateTime? Actualizada { get; set; }
    }

    /// <summary>El alumno y su ficha de salud; las columnas Fsa_* vienen vacías si todavía no tiene ficha.</summary>
    public class FichaSaludViewModel
    {
        public int Alu_Id { get; set; }
        public string Alumno { get; set; }
        public string PrimerNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string Identidad { get; set; }
        public string Foto { get; set; }
        public string Sexo { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Nivel { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public string Curso { get; set; }
        public string Estado { get; set; }
        public int? Fsa_Id { get; set; }
        public string Fsa_TipoSangre { get; set; }
        public bool Fsa_TieneAlerta { get; set; }
        public string Fsa_Alerta { get; set; }
        public string Fsa_Alergias { get; set; }
        public string Fsa_Condiciones { get; set; }
        public string Fsa_Medicamentos { get; set; }
        public string Fsa_RestriccionesAlimentarias { get; set; }
        public string Fsa_MedicoNombre { get; set; }
        public string Fsa_MedicoTelefono { get; set; }
        public string Fsa_Hospital { get; set; }
        public string Fsa_Seguro { get; set; }
        public string Fsa_SeguroPoliza { get; set; }
        public string Fsa_ContactoEmergenciaNombre { get; set; }
        public string Fsa_ContactoEmergenciaParentesco { get; set; }
        public string Fsa_ContactoEmergenciaTelefono { get; set; }
        public string Fsa_Notas { get; set; }
        public int? Fsa_Version { get; set; }
        public DateTime? Fsa_FechaRegistra { get; set; }
        public DateTime? Actualizada { get; set; }
        public string ActualizadaPor { get; set; }

        public bool TieneFicha => Fsa_Id.HasValue && Fsa_Id.Value > 0;

        /// <summary>Iniciales para el avatar: primer nombre + primer apellido.</summary>
        public string Iniciales
        {
            get
            {
                string a = (PrimerNombre ?? "").Trim(), b = (ApellidoPaterno ?? "").Trim();
                if (a.Length == 0 && b.Length == 0)
                {
                    var partes = (Alumno ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    return string.Concat(partes.Take(2).Select(p => p.Substring(0, 1))).ToUpperInvariant();
                }
                return ((a.Length > 0 ? a.Substring(0, 1) : "") + (b.Length > 0 ? b.Substring(0, 1) : "")).ToUpperInvariant();
            }
        }

        /// <summary>"Séptimo · Sección A" (o lo que haya).</summary>
        public string Ubicacion =>
            string.Join(" · ", new[] { Grado, string.IsNullOrWhiteSpace(Seccion) ? null : "Sección " + Seccion }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
    }

    /// <summary>Un encargado del alumno con su teléfono ("a quién llamar").</summary>
    public class FichaSaludContactoViewModel
    {
        public int Enc_Id { get; set; }
        public string Nombre { get; set; }
        public string Parentesco { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public bool EsPrincipal { get; set; }
    }

    /// <summary>Una entrada del historial de cambios (qué campos, quién y cuándo; sin valores).</summary>
    public class FichaSaludBitacoraViewModel
    {
        public int Fsb_Id { get; set; }
        public string Fsb_Accion { get; set; }
        public string Fsb_Campos { get; set; }
        public DateTime Fsb_Fecha { get; set; }
        public int? Fsb_Usuario { get; set; }
        public string Usuario { get; set; }
    }

    /// <summary>Alerta crítica de un alumno para el pase de lista (solo el texto corto).</summary>
    public class FichaSaludAlertaViewModel
    {
        public int Alu_Id { get; set; }
        public string Fsa_Alerta { get; set; }
    }

    /// <summary>Lo que se envía al guardar (crear o cambiar) la ficha.</summary>
    public class FichaSaludGuardarViewModel
    {
        public int Alu_Id { get; set; }
        public string Fsa_TipoSangre { get; set; }
        public bool Fsa_TieneAlerta { get; set; }
        public string Fsa_Alerta { get; set; }
        public string Fsa_Alergias { get; set; }
        public string Fsa_Condiciones { get; set; }
        public string Fsa_Medicamentos { get; set; }
        public string Fsa_RestriccionesAlimentarias { get; set; }
        public string Fsa_MedicoNombre { get; set; }
        public string Fsa_MedicoTelefono { get; set; }
        public string Fsa_Hospital { get; set; }
        public string Fsa_Seguro { get; set; }
        public string Fsa_SeguroPoliza { get; set; }
        public string Fsa_ContactoEmergenciaNombre { get; set; }
        public string Fsa_ContactoEmergenciaParentesco { get; set; }
        public string Fsa_ContactoEmergenciaTelefono { get; set; }
        public string Fsa_Notas { get; set; }
        public int? Fsa_Version { get; set; }
        public int? Usuario { get; set; }
    }

    /// <summary>Respuesta de la API al guardar: { Ok, Id, Version, Accion, Campos, Mensaje }.</summary>
    public class FichaSaludResultadoViewModel
    {
        public bool Ok { get; set; }
        public int Id { get; set; }
        public int Version { get; set; }
        public string Accion { get; set; }
        public string Campos { get; set; }
        public string Mensaje { get; set; }
    }

    /// <summary>Bloque "En caso de emergencia": la ficha + a quién llamar. Lo usan la ficha y la pestaña Salud.</summary>
    public class FichaSaludEmergenciaViewModel
    {
        public FichaSaludViewModel Ficha { get; set; }
        public List<FichaSaludContactoViewModel> Contactos { get; set; } = new List<FichaSaludContactoViewModel>();
    }
}

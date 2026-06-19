using GESTION_COLEGIAL.Business.Models;
using GESTION_COLEGIAL.Business.Services.ModuloFinanzas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Agrega métricas por aula (sección) a partir de los endpoints existentes.
    /// La API externa no expone hoy un endpoint único agrupado por sección, por lo que
    /// este servicio compone los datos a partir de Alumnos + Deudores.
    /// </summary>
    public class DashboardAulasService
    {
        private readonly AlumnosService _alumnos = new AlumnosService();
        private readonly CuentasCobrarService _cuentas = new CuentasCobrarService();

        public async Task<DashboardAulasResponseViewModel> ObtenerAsync()
        {
            var alumnosTask = _alumnos.ListAsync();
            var deudoresTask = _cuentas.ListDeudoresAsync();
            await Task.WhenAll(alumnosTask, deudoresTask);

            var alumnos = (alumnosTask.Result ?? Enumerable.Empty<AlumnoViewModel>()).ToList();
            var deudores = (deudoresTask.Result ?? Enumerable.Empty<CuentaCobrarListViewModel>()).ToList();

            var deudoresPorNombre = new HashSet<string>(
                deudores
                    .Select(d => Normalizar(d.Alumno))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
            );

            // La API Alumnos/ListAsync NO expone Sec_Id ni Cur_Id, sólo descripciones.
            // Agrupamos por una clave compuesta de descripciones para identificar aulas únicas.
            string ClaveAula(AlumnoViewModel a) => string.Join("||", new[]
            {
                (a.Niv_Descripcion ?? string.Empty).Trim(),
                (a.Cno_Descripcion ?? string.Empty).Trim(),
                (a.Mda_Descripcion ?? string.Empty).Trim(),
                (a.Sec_Descripcion ?? string.Empty).Trim()
            }).ToUpperInvariant();

            var aulas = alumnos
                .Where(a => !string.IsNullOrWhiteSpace(a.Sec_Descripcion)
                            || !string.IsNullOrWhiteSpace(a.Cno_Descripcion)
                            || !string.IsNullOrWhiteSpace(a.Niv_Descripcion))
                .GroupBy(ClaveAula)
                .Select((g, index) =>
                {
                    var lista = g.ToList();
                    string nivel = ModaTexto(lista.Select(x => x.Niv_Descripcion));
                    string modalidad = ModaTexto(lista.Select(x => x.Mda_Descripcion));
                    int alumnosCount = lista.Count;
                    int capacidad = CapacidadPorNivel(nivel, alumnosCount);

                    int conDeuda = lista.Count(a => deudoresPorNombre.Contains(NombreCompleto(a)));

                    string seccion = lista.Select(a => a.Sec_Descripcion).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                                    ?? "Única";

                    string aulaNombre = ComponerNombreAula(lista, seccion);

                    return new DashboardAulaViewModel
                    {
                        Id = index + 1,
                        Nombre = aulaNombre,
                        Nivel = string.IsNullOrWhiteSpace(nivel) ? "Sin nivel" : nivel,
                        Modalidad = string.IsNullOrWhiteSpace(modalidad) ? "—" : modalidad,
                        Capacidad = capacidad,
                        Alumnos = alumnosCount,
                        Bajo = 0,
                        Deuda = conDeuda,
                        Asistencia = 0,
                        BajoDisponible = false,
                        AsistenciaDisponible = false
                    };
                })
                .OrderBy(a => a.Nivel)
                .ThenBy(a => a.Nombre)
                .ToList();

            var niveles = aulas.Select(a => a.Nivel).Distinct().OrderBy(n => n).ToList();

            return new DashboardAulasResponseViewModel
            {
                Aulas = aulas,
                Niveles = niveles,
                BajoDisponible = false,
                AsistenciaDisponible = false
            };
        }

        private static string ComponerNombreAula(IEnumerable<AlumnoViewModel> lista, string seccion)
        {
            var items = lista.ToList();
            string curso = items.Select(x => x.Cur_Descripcion).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            if (string.IsNullOrWhiteSpace(curso))
                curso = items.Select(x => x.Cno_Descripcion).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            if (!string.IsNullOrWhiteSpace(curso))
                return string.Format("{0} · {1}", curso.Trim(), seccion);
            return seccion;
        }

        private static int CapacidadPorNivel(string nivel, int alumnos)
        {
            string n = (nivel ?? string.Empty).ToLowerInvariant();
            int baseCap;
            if (n.Contains("primar")) baseCap = 35;
            else if (n.Contains("secund") || n.Contains("bachiller") || n.Contains("medio")) baseCap = 40;
            else baseCap = 35;
            return Math.Max(baseCap, alumnos);
        }

        private static string ModaTexto(IEnumerable<string> valores)
        {
            return valores
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .GroupBy(v => v)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();
        }

        private static string NombreCompleto(AlumnoViewModel a)
        {
            if (!string.IsNullOrWhiteSpace(a.Alu_Nombre)) return Normalizar(a.Alu_Nombre);
            string nombre = string.Format("{0} {1} {2} {3}",
                a.Per_PrimerNombre, a.Per_SegundoNombre, a.Per_ApellidoPaterno, a.Per_ApellidoMaterno);
            return Normalizar(nombre);
        }

        private static string Normalizar(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var partes = s.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", partes).ToUpperInvariant();
        }
    }
}

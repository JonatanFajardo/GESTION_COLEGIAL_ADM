using GESTION_COLEGIAL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GESTION_COLEGIAL.Business.Services
{
    /// <summary>
    /// Calcula la nómina mensual de los docentes a partir de los horarios
    /// programados (tbHorarios) y un catálogo interno de tarifas por área y
    /// modalidad. Implementación basada en código laboral de Honduras 2025.
    ///
    /// Tarifas hardcodeadas — para edición dinámica migrar a tbTarifasHora.
    /// </summary>
    public class NominaDocentesService
    {
        private const int SEMANAS_POR_MES = 4;
        private const int MAX_PERIODOS_DIA_REGULAR = 9;
        private const decimal RECARGO_EXTRA_DIURNO = 1.25m;
        private const decimal RECARGO_EXTRA_NOCTURNO = 1.50m;
        private const decimal IHSS_PORCENTAJE = 0.05m;
        private const decimal IHSS_TECHO_MENSUAL = 11038.22m;
        private const decimal COLEGIO_PROFESIONAL_MENSUAL = 200m;
        private const decimal EXENCION_GASTOS_MEDICOS_ANUAL = 40000m;

        private readonly HorariosService _horariosService = new HorariosService();
        private readonly EmpleadosService _empleadosService = new EmpleadosService();

        /// Tarifa por período (40 min) en Lempiras según (Area, Mda_Id).
        /// Mda_Id: 1=Matutina, 2=Vespertina, 3=Nocturna.
        private static readonly Dictionary<string, Dictionary<int, decimal>> TARIFAS = new Dictionary<string, Dictionary<int, decimal>>
        {
            { "MAT", new Dictionary<int, decimal> { {1, 85m}, {2, 90m}, {3, 103m} } },
            { "CIE", new Dictionary<int, decimal> { {1, 85m}, {2, 90m}, {3, 103m} } },
            { "LEN", new Dictionary<int, decimal> { {1, 72m}, {2, 77m}, {3, 87m} } },
            { "SOC", new Dictionary<int, decimal> { {1, 72m}, {2, 77m}, {3, 87m} } },
            { "ING", new Dictionary<int, decimal> { {1, 90m}, {2, 97m}, {3, 110m} } },
            { "CON", new Dictionary<int, decimal> { {1, 95m}, {2, 100m}, {3, 115m} } },
            { "TEC", new Dictionary<int, decimal> { {1, 80m}, {2, 85m}, {3, 97m} } },
            { "ART", new Dictionary<int, decimal> { {1, 60m}, {2, 65m}, {3, 75m} } },
            { "OTR", new Dictionary<int, decimal> { {1, 65m}, {2, 70m}, {3, 80m} } }
        };

        private static readonly Dictionary<int, string> AREA_POR_MATERIA = new Dictionary<int, string>
        {
            {2, "MAT"}, {15, "MAT"}, {16, "MAT"}, {17, "MAT"}, {19, "MAT"},
            {3, "LEN"}, {22, "LEN"}, {24, "LEN"}, {34, "LEN"},
            {4, "CIE"}, {6, "CIE"}, {10, "CIE"}, {18, "CIE"},
            {8, "ING"}, {26, "ING"}, {36, "ING"}, {47, "ING"},
            {14, "CON"}, {32, "CON"}, {33, "CON"}, {35, "CON"}, {37, "CON"},
            {38, "CON"}, {39, "CON"}, {41, "CON"}, {42, "CON"}, {43, "CON"},
            {44, "CON"}, {45, "CON"}, {46, "CON"},
            {1, "SOC"}, {5, "SOC"}, {7, "SOC"}, {11, "SOC"}, {12, "SOC"},
            {13, "SOC"}, {20, "SOC"}, {23, "SOC"}, {30, "SOC"}, {31, "SOC"},
            {9, "TEC"}, {25, "TEC"}, {27, "TEC"}, {28, "TEC"}, {29, "TEC"},
            {21, "ART"},
            {40, "OTR"}
        };

        public string AreaDe(int matId) =>
            AREA_POR_MATERIA.TryGetValue(matId, out var a) ? a : "OTR";

        public decimal TarifaDe(string area, int? mdaId)
        {
            var mda = (mdaId ?? 1);
            if (!TARIFAS.ContainsKey(area)) area = "OTR";
            return TARIFAS[area].TryGetValue(mda, out var t) ? t : TARIFAS[area][1];
        }

        /// <summary>
        /// Resumen de nómina para TODOS los docentes (un registro por Emp_Id)
        /// del periodo @anio + @semestreId. Usa los horarios programados.
        /// </summary>
        public async Task<IEnumerable<NominaDocenteViewModel>> ListMesAsync(int anio, int semestreId)
        {
            var horarios = (await _horariosService.ListAsync()) ?? Enumerable.Empty<HorarioViewModel>();
            var horariosPeriodo = horarios
                .Where(h => h.Hor_Año == anio && h.Sem_Id == semestreId)
                .ToList();

            if (!horariosPeriodo.Any())
                return Enumerable.Empty<NominaDocenteViewModel>();

            // Diccionario rápido de empleados (Emp_Id → datos)
            var empleados = (await _empleadosService.ListAsync()) ?? Enumerable.Empty<EmpleadoViewModel>();
            var mapaEmp = empleados.ToDictionary(e => e.Emp_Id, e => e);

            return horariosPeriodo
                .GroupBy(h => h.Emp_Id)
                .Select(g => CalcularResumen(g.Key, g.ToList(), mapaEmp))
                .OrderByDescending(n => n.NetoAPagar)
                .ToList();
        }

        /// <summary>
        /// Devuelve desglose detallado para 1 docente (para el modal).
        /// </summary>
        public async Task<NominaDetalleViewModel> DetalleAsync(int empId, int anio, int semestreId)
        {
            var horarios = (await _horariosService.ListAsync()) ?? Enumerable.Empty<HorarioViewModel>();
            var horariosDoc = horarios
                .Where(h => h.Hor_Año == anio && h.Sem_Id == semestreId && h.Emp_Id == empId)
                .ToList();

            var empleados = (await _empleadosService.ListAsync()) ?? Enumerable.Empty<EmpleadoViewModel>();
            var mapaEmp = empleados.ToDictionary(e => e.Emp_Id, e => e);
            var resumen = CalcularResumen(empId, horariosDoc, mapaEmp);

            // Lineas por (Materia, Modalidad)
            var lineas = horariosDoc
                .GroupBy(h => new { h.Mat_Id, Mda = h.Mda_Id ?? 1 })
                .Select(g =>
                {
                    var area = AreaDe(g.Key.Mat_Id);
                    var tar = TarifaDe(area, g.Key.Mda);
                    var pSem = g.Count();
                    var pMes = pSem * SEMANAS_POR_MES;
                    return new NominaLineaViewModel
                    {
                        Mat_Id = g.Key.Mat_Id,
                        Mat_Nombre = g.First().Mat_Nombre,
                        Area = area,
                        Mda_Id = g.Key.Mda,
                        Mda_Descripcion = g.First().Mda_Descripcion,
                        PeriodosSemanales = pSem,
                        PeriodosMensuales = pMes,
                        TarifaPeriodo = tar,
                        Subtotal = tar * pMes
                    };
                })
                .OrderByDescending(l => l.Subtotal)
                .ToList();

            // Carga por día de la semana
            var cargaDias = horariosDoc
                .GroupBy(h => new { h.Dia_Id, h.Dia_Descripcion })
                .Select(g =>
                {
                    var total = g.Count();
                    var extras = Math.Max(0, total - MAX_PERIODOS_DIA_REGULAR);
                    return new NominaCargaDiaViewModel
                    {
                        Dia_Id = g.Key.Dia_Id,
                        Dia_Descripcion = g.Key.Dia_Descripcion,
                        Periodos = total,
                        PeriodosExtra = extras
                    };
                })
                .OrderBy(x => x.Dia_Id)
                .ToList();

            return new NominaDetalleViewModel
            {
                Resumen = resumen,
                Lineas = lineas,
                CargaPorDia = cargaDias
            };
        }

        /* ------------------------------------------------------------------ */
        /* Cálculo                                                            */
        /* ------------------------------------------------------------------ */

        private NominaDocenteViewModel CalcularResumen(
            int empId,
            List<HorarioViewModel> horariosDoc,
            Dictionary<int, EmpleadoViewModel> mapaEmp)
        {
            var vm = new NominaDocenteViewModel { Emp_Id = empId };

            if (mapaEmp.TryGetValue(empId, out var emp))
            {
                vm.Emp_Codigo = emp.Emp_Codigo;
                vm.NombreCompleto = (emp.Emp_Nombre ?? "").Trim();
                if (string.IsNullOrWhiteSpace(vm.NombreCompleto))
                    vm.NombreCompleto = horariosDoc.FirstOrDefault()?.Emp_NombreCompleto ?? $"Empleado {empId}";
                vm.TituloDescripcion = emp.Tit_Descripcion;
                vm.CargoDescripcion = emp.Car_Descripcion;
            }
            else
            {
                vm.NombreCompleto = horariosDoc.FirstOrDefault()?.Emp_NombreCompleto ?? $"Empleado {empId}";
            }

            // Periodos totales
            vm.PeriodosSemanales = horariosDoc.Count;
            vm.PeriodosMensuales = vm.PeriodosSemanales * SEMANAS_POR_MES;
            vm.MateriasDistintas = horariosDoc.Select(h => h.Mat_Id).Distinct().Count();
            vm.SeccionesAtendidas = horariosDoc.Select(h => $"{h.Cur_Id}-{h.Sec_Id}-{h.Cun_Id}").Distinct().Count();

            // Modalidad/área principal (la que más periodos tenga)
            vm.ModalidadPrincipal = horariosDoc
                .GroupBy(h => h.Mda_Descripcion ?? "—")
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            vm.AreaPrincipal = horariosDoc
                .GroupBy(h => AreaDe(h.Mat_Id))
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "OTR";

            // Sueldo bruto: sumar (periodos × tarifa) por (Mat, Mda).
            decimal bruto = 0m;
            foreach (var grp in horariosDoc.GroupBy(h => new { h.Mat_Id, Mda = h.Mda_Id ?? 1 }))
            {
                var area = AreaDe(grp.Key.Mat_Id);
                var tar = TarifaDe(area, grp.Key.Mda);
                bruto += tar * grp.Count() * SEMANAS_POR_MES;
            }
            vm.SueldoBruto = Math.Round(bruto, 2);

            // Horas extra: periodos que excedan 9/día.
            int extrasSem = horariosDoc
                .GroupBy(h => h.Dia_Id)
                .Sum(g => Math.Max(0, g.Count() - MAX_PERIODOS_DIA_REGULAR));

            vm.PeriodosExtraSemanales = extrasSem;
            vm.PeriodosExtraMensuales = extrasSem * SEMANAS_POR_MES;

            // Cálculo monto extra: tomar las clases que exceden el cupo del día
            // y aplicar recargo según modalidad (diurno 1.25x, nocturno 1.5x).
            decimal montoExtra = 0m;
            foreach (var grupoDia in horariosDoc.GroupBy(h => h.Dia_Id))
            {
                var ordenadas = grupoDia.OrderBy(h => h.Hor_HoraInicio).ToList();
                for (int i = MAX_PERIODOS_DIA_REGULAR; i < ordenadas.Count; i++)
                {
                    var h = ordenadas[i];
                    var area = AreaDe(h.Mat_Id);
                    var mda = h.Mda_Id ?? 1;
                    var tar = TarifaDe(area, mda);
                    var recargo = (mda == 3) ? RECARGO_EXTRA_NOCTURNO : RECARGO_EXTRA_DIURNO;
                    // Adicional sobre lo ya contado en bruto (extra = bruto × (recargo - 1))
                    montoExtra += tar * (recargo - 1m) * SEMANAS_POR_MES;
                }
            }
            vm.HorasExtraMonto = Math.Round(montoExtra, 2);
            vm.TotalBruto = vm.SueldoBruto + vm.HorasExtraMonto;

            // Deducciones
            vm.DeduccionIHSS = Math.Round(Math.Min(vm.TotalBruto, IHSS_TECHO_MENSUAL) * IHSS_PORCENTAJE, 2);
            vm.DeduccionColegio = COLEGIO_PROFESIONAL_MENSUAL;
            vm.DeduccionISR = CalcularISRMensual(vm.TotalBruto, vm.DeduccionIHSS);
            vm.TotalDeducciones = vm.DeduccionIHSS + vm.DeduccionISR + vm.DeduccionColegio;

            vm.NetoAPagar = Math.Round(vm.TotalBruto - vm.TotalDeducciones, 2);

            return vm;
        }

        /// <summary>
        /// ISR mensual usando tabla anual escalonada SAR HN 2025.
        /// Base = (sueldo bruto - IHSS) × 12 - 40000 (exención gastos).
        /// </summary>
        private static decimal CalcularISRMensual(decimal brutoMensual, decimal ihssMensual)
        {
            decimal baseAnual = (brutoMensual - ihssMensual) * 12m - EXENCION_GASTOS_MEDICOS_ANUAL;
            if (baseAnual <= 217493.16m) return 0m;

            decimal impuestoAnual = 0m;
            if (baseAnual <= 331638.50m)
            {
                impuestoAnual = (baseAnual - 217493.16m) * 0.15m;
            }
            else if (baseAnual <= 771254.32m)
            {
                impuestoAnual = (331638.50m - 217493.16m) * 0.15m
                              + (baseAnual - 331638.50m) * 0.20m;
            }
            else
            {
                impuestoAnual = (331638.50m - 217493.16m) * 0.15m
                              + (771254.32m - 331638.50m) * 0.20m
                              + (baseAnual - 771254.32m) * 0.25m;
            }

            return Math.Round(impuestoAnual / 12m, 2);
        }
    }
}

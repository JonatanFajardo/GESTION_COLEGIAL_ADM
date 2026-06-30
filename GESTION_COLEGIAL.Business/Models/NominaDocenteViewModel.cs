using System.Collections.Generic;

namespace GESTION_COLEGIAL.Business.Models
{
    /// Resumen de nómina mensual de un docente para listar en cards.
    public class NominaDocenteViewModel
    {
        public int Emp_Id { get; set; }
        public string Emp_Codigo { get; set; }
        public string NombreCompleto { get; set; }
        public string TituloDescripcion { get; set; }
        public string CargoDescripcion { get; set; }

        public int PeriodosSemanales { get; set; }
        public int PeriodosMensuales { get; set; }
        public int PeriodosExtraSemanales { get; set; }
        public int PeriodosExtraMensuales { get; set; }
        public int MateriasDistintas { get; set; }
        public int SeccionesAtendidas { get; set; }

        public decimal SueldoBruto { get; set; }
        public decimal HorasExtraMonto { get; set; }
        public decimal TotalBruto { get; set; }

        public decimal DeduccionIHSS { get; set; }
        public decimal DeduccionISR { get; set; }
        public decimal DeduccionColegio { get; set; }
        public decimal TotalDeducciones { get; set; }

        public decimal NetoAPagar { get; set; }

        // Para distribuir el badge de "modalidad principal" en la card
        public string ModalidadPrincipal { get; set; }

        // Para colorear según el área predominante
        public string AreaPrincipal { get; set; }
    }

    /// Desglose detallado por materia / modalidad (para el modal).
    public class NominaDetalleViewModel
    {
        public NominaDocenteViewModel Resumen { get; set; }
        public IEnumerable<NominaLineaViewModel> Lineas { get; set; }
        public IEnumerable<NominaCargaDiaViewModel> CargaPorDia { get; set; }
    }

    public class NominaLineaViewModel
    {
        public int Mat_Id { get; set; }
        public string Mat_Nombre { get; set; }
        public string Area { get; set; }
        public int Mda_Id { get; set; }
        public string Mda_Descripcion { get; set; }
        public int PeriodosSemanales { get; set; }
        public int PeriodosMensuales { get; set; }
        public decimal TarifaPeriodo { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class NominaCargaDiaViewModel
    {
        public int Dia_Id { get; set; }
        public string Dia_Descripcion { get; set; }
        public int Periodos { get; set; }
        public int PeriodosExtra { get; set; }
    }
}

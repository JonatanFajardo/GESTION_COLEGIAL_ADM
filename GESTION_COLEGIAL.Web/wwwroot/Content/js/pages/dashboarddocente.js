var DashboardDocente = (function () {
    'use strict';

    var _baseUrl = '';

    function fmtNum(val) {
        if (val === null || val === undefined || val === '') return '—';
        return Number(val).toLocaleString('es-HN');
    }

    function setText(id, value) {
        var el = document.getElementById(id);
        if (el) el.textContent = (value !== null && value !== undefined) ? value : '—';
    }

    function cargar() {
        // El UI controller hace passthrough crudo del API (PascalCase).
        $.getJSON(_baseUrl, function (res) {
            var d = res || {};

            // KPIs (DashboardDocente_KpisDocenteResult): TotalAlumnos, TotalMaterias
            var k = d.KpisDocente || {};
            setText('kpi-alumnosTotal', fmtNum(k.TotalAlumnos));

            // "Clases Hoy" = cantidad de filas en ClasesHoy
            var clases = d.ClasesHoy || [];
            setText('kpi-clasesHoy', fmtNum(clases.length));

            // No hay datos para Tareas Pendientes ni Promedio Sección en el SP actual.
            // Se muestra el total de materias del docente como referencia adicional.
            setText('kpi-tareasPendientes', '—');
            setText('kpi-promedioSeccion', '—');
        }).fail(function (xhr, status, err) {
            console.warn('DashboardDocente: no se pudo cargar el resumen.', status, err);
        });
    }

    var obj = {};

    obj.init = function (url) {
        _baseUrl = url || '';
        cargar();
    };

    return obj;
}());

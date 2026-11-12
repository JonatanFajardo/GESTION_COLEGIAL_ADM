/* Reporte de asistencia de alumnos — filtros en cascada, validación de fechas,
   "Solo en riesgo" y exportación a Excel (CSV) hecha en el navegador. */
(function ($) {
    "use strict";

    var DIA_MS = 24 * 60 * 60 * 1000;

    /* ───────────── Filtros: Nivel → Grado → Sección ───────────── */
    var $form = $("#ras-form-filtros");
    if ($form.length) {
        var filtros = [];
        try { filtros = JSON.parse($form.attr("data-filtros") || "[]"); } catch (e) { filtros = []; }
        var maxDias = parseInt($form.attr("data-max-dias"), 10) || 366;

        var $nivel = $("#ras-nivel"), $grado = $("#ras-grado"), $seccion = $("#ras-seccion");
        var $desde = $("#ras-desde"), $hasta = $("#ras-hasta"), $error = $("#ras-error-fechas");

        /* Llena un select con opciones únicas; sin padre queda deshabilitado. */
        function llenar($select, items, valor, texto, vacioConPadre, vacioSinPadre, hayPadre) {
            $select.empty().append($("<option>", { value: "", text: hayPadre ? vacioConPadre : vacioSinPadre }));
            if (hayPadre) {
                var vistos = {};
                items.forEach(function (it) {
                    var v = it[valor];
                    if (vistos[v]) return;
                    vistos[v] = true;
                    $select.append($("<option>", { value: v, text: it[texto] }));
                });
            }
            $select.prop("disabled", !hayPadre);
        }

        $nivel.on("change", function () {
            var niv = parseInt($nivel.val(), 10);
            var grados = filtros.filter(function (f) { return f.n === niv; })
                                .sort(function (a, b) { return a.c - b.c; });
            llenar($grado, grados, "c", "cn", "Todos los grados", "Elija un nivel primero", !isNaN(niv));
            llenar($seccion, [], "s", "sn", "Todas las secciones", "Elija un grado primero", false);
        });

        $grado.on("change", function () {
            var niv = parseInt($nivel.val(), 10), cun = parseInt($grado.val(), 10);
            var secciones = filtros.filter(function (f) { return f.n === niv && f.c === cun; })
                                   .sort(function (a, b) { return String(a.sn).localeCompare(String(b.sn), "es"); });
            llenar($seccion, secciones, "s", "sn", "Todas las secciones", "Elija un grado primero", !isNaN(cun));
        });

        function leerFecha(valor) {
            var m = /^(\d{4})-(\d{2})-(\d{2})$/.exec(valor || "");
            return m ? Date.UTC(+m[1], +m[2] - 1, +m[3]) : NaN;
        }

        function errorFechas() {
            var d = leerFecha($desde.val()), h = leerFecha($hasta.val()), hoy = leerFecha($hasta.attr("max"));
            if (isNaN(d) || isNaN(h)) return "Escriba las dos fechas: «Desde» y «Hasta».";
            if (!isNaN(hoy) && h > hoy) return "La fecha «Hasta» no puede ser posterior a hoy.";
            if (d > h) return "La fecha «Desde» tiene que ser anterior o igual a la fecha «Hasta».";
            if ((h - d) / DIA_MS + 1 > maxDias) return "Elija un rango de " + maxDias + " días como máximo (un año escolar).";
            return null;
        }

        $desde.add($hasta).on("change", function () {
            $desde.add($hasta).removeClass("is-invalid").removeAttr("aria-invalid");
            $error.text("");
        });

        $form.on("submit", function (ev) {
            var msg = errorFechas();
            if (msg) {
                ev.preventDefault();
                $error.text(msg);
                $desde.add($hasta).addClass("is-invalid").attr("aria-invalid", "true");
                $desde.trigger("focus");
                return;
            }
            // Sin campos vacíos en la dirección.
            $form.find("select").each(function () { if (!$(this).val()) $(this).prop("disabled", true); });
            $("#ras-cargando").removeAttr("hidden");
        });
    }

    /* ───────────── Solo en riesgo (tabla de alumnos y planilla) ───────────── */
    $("#ras-solo-riesgo").on("change", function () {
        var solo = this.checked;
        $("tr[data-riesgo]").each(function () {
            this.hidden = solo && this.getAttribute("data-riesgo") !== "1";
        });
        var hayVisibles = $($(this).attr("data-tabla")).find("tbody tr[data-riesgo]:not([hidden])").length > 0;
        $("#ras-sin-riesgo").prop("hidden", hayVisibles);
    });

    /* ───────────── Exportar a Excel (CSV con BOM y ";", lo abre Excel en español) ───────────── */
    function textoCelda(celda) {
        var v = celda.getAttribute("data-csv");
        if (v === null) v = celda.textContent;
        return v.replace(/\s+/g, " ").trim();
    }

    function campoCsv(v) {
        return /[";\r\n]/.test(v) ? '"' + v.replace(/"/g, '""') + '"' : v;
    }

    function filasCsv(tabla) {
        var filas = [];
        var omitir = [];
        $(tabla).find("thead tr").first().children().each(function (i) {
            if (this.hasAttribute("data-csv-omitir")) omitir.push(i);
        });
        $(tabla).find("tr").each(function () {
            if (this.hidden) return;
            var celdas = [];
            $(this).children("th,td").each(function (i) {
                if (omitir.indexOf(i) < 0) celdas.push(campoCsv(textoCelda(this)));
            });
            filas.push(celdas.join(";"));
        });
        return filas;
    }

    $(document).on("click", "[data-exportar]", function () {
        var tabla = document.querySelector(this.getAttribute("data-exportar"));
        if (!tabla) return;
        var csv = "﻿" + filasCsv(tabla).join("\r\n");
        var blob = new Blob([csv], { type: "text/csv;charset=utf-8" });
        var url = URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.href = url;
        a.download = this.getAttribute("data-archivo") || "asistencia.csv";
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
    });
})(jQuery);

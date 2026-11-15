// Cuadro de honor: ranking por año en dos niveles + impresión de lista y diplomas.
var CuadroHonor = (function () {
    "use strict";

    var cfg = { anio: 0, umbralSuperior: 95, urls: {} };
    var datos = [];       // último listado traído
    var porId = {};       // Alu_Id -> alumno

    function esc(s) { return $("<div>").text(s == null ? "" : s).html(); }
    function prom(n) { return parseFloat(n).toFixed(2); }
    function ubic(a) {
        return [a.Nivel, a.Curso, a.Seccion ? "Sección " + a.Seccion : null]
            .filter(function (x) { return x; }).map(esc).join(" · ");
    }

    function cargarDropdowns() {
        $.getJSON(cfg.urls.dropdowns, function (d) {
            (d.niveles || []).forEach(function (n) {
                $("#f-niv").append('<option value="' + n.id + '">' + esc(n.texto) + '</option>');
            });
            (d.secciones || []).forEach(function (s) {
                $("#f-sec").append('<option value="' + s.id + '">' + esc(s.texto) + '</option>');
            });
        });
    }

    function ver() {
        var anio = parseInt($("#f-anio").val(), 10) || cfg.anio;
        var niv = $("#f-niv").val();
        var sec = $("#f-sec").val();
        var url = cfg.urls.listado + "?anio=" + anio + (niv ? "&nivId=" + niv : "") + (sec ? "&secId=" + sec : "");

        $("#btn-ver").prop("disabled", true);
        $.getJSON(url, function (resp) {
            datos = (resp && resp.data) ? resp.data : [];
            porId = {};
            datos.forEach(function (a) { porId[a.Alu_Id] = a; });
            render(anio);
        }).fail(function () {
            toastr.error("No se pudo cargar el cuadro de honor.");
        }).always(function () {
            $("#btn-ver").prop("disabled", false);
        });
    }

    function filaHtml(a, pos) {
        var medalla = pos <= 3 ? ["🥇", "🥈", "🥉"][pos - 1] + " " : "";
        return '<tr>' +
            '<td class="no-print"><input type="checkbox" class="chk-alu" value="' + a.Alu_Id + '"></td>' +
            '<td class="ch-pos">' + medalla + pos + '</td>' +
            '<td><strong>' + esc(a.Alu_NombreCompleto) + '</strong></td>' +
            '<td class="text-center"><span class="badge badge-primary" style="font-size:.85rem">' + prom(a.Promedio) + '</span></td>' +
            '<td>' + esc(a.Concepto || "") + '</td>' +
            '<td class="text-muted">' + ubic(a) + '</td>' +
            '</tr>';
    }

    function render(anio) {
        var sup = datos.filter(function (a) { return a.NivelHonor === "S"; });
        var hon = datos.filter(function (a) { return a.NivelHonor !== "S"; });

        pintar("#tbody-superior", sup);
        pintar("#tbody-honor", hon);
        $("#sec-superior").prop("hidden", sup.length === 0);
        $("#sec-honor").prop("hidden", hon.length === 0);
        $("#estado-vacio").prop("hidden", datos.length !== 0);
    }

    function pintar(sel, lista) {
        var $b = $(sel).empty();
        lista.forEach(function (a, i) { $b.append(filaHtml(a, i + 1)); });
    }

    // ---- Impresión (ventana nueva, solo el contenido imprimible) ----
    function abrirImpresion(titulo, css, cuerpo) {
        var w = window.open("", "_blank");
        if (!w) { toastr.warning("Permita las ventanas emergentes para imprimir."); return; }
        w.document.write(
            '<!doctype html><html lang="es"><head><meta charset="utf-8"><title>' + titulo + '</title>' +
            '<style>' + css + '</style></head><body onload="window.focus();window.print();">' + cuerpo + '</body></html>');
        w.document.close();
    }

    var CSS_LISTA =
        'body{font-family:Arial,Helvetica,sans-serif;margin:24px;color:#1e293b;}' +
        'h2{text-align:center;margin:0 0 4px;}h3{margin:24px 0 8px;}.sub{text-align:center;color:#64748b;margin:0 0 16px;}' +
        'table{width:100%;border-collapse:collapse;margin-bottom:20px;}' +
        'th,td{border:1px solid #cbd5e1;padding:6px 9px;text-align:left;font-size:12.5px;}th{background:#f1f5f9;}' +
        '.r{text-align:right;}';

    var CSS_DIP =
        'body{font-family:Arial,Helvetica,sans-serif;margin:0;color:#1e293b;}' +
        '.diploma{border:3px double #c7a008;border-radius:10px;padding:48px 40px;margin:24px auto;max-width:900px;text-align:center;page-break-after:always;}' +
        '.diploma:last-child{page-break-after:auto;}' +
        '.diploma h2{letter-spacing:2px;color:#b7791f;margin:0 0 4px;}' +
        '.sub{color:#64748b;}' +
        '.nombre{font-size:2rem;font-weight:700;border-bottom:2px solid #e2e8f0;display:inline-block;padding:6px 32px;margin:18px 0;}' +
        '.firma{margin-top:64px;display:inline-block;border-top:1px solid #334155;padding-top:6px;min-width:260px;}';

    function tablaLista(titulo, lista) {
        if (!lista.length) return "";
        var filas = lista.map(function (a, i) {
            return '<tr><td>' + (i + 1) + '</td><td>' + esc(a.Alu_NombreCompleto) + '</td><td class="r">' + prom(a.Promedio) +
                '</td><td>' + esc(a.Concepto || "") + '</td><td>' + ubic(a) + '</td></tr>';
        }).join("");
        return '<h3>' + titulo + '</h3><table><thead><tr><th>#</th><th>Alumno</th><th class="r">Promedio</th><th>Concepto</th><th>Nivel · Curso · Sección</th></tr></thead><tbody>' +
            filas + '</tbody></table>';
    }

    function imprimirLista() {
        if (!datos.length) { toastr.warning("Primero cargue el cuadro de honor."); return; }
        var sup = datos.filter(function (a) { return a.NivelHonor === "S"; });
        var hon = datos.filter(function (a) { return a.NivelHonor !== "S"; });
        var cuerpo = '<h2>Cuadro de Honor</h2><p class="sub">Año ' + esc($("#f-anio").val()) + '</p>' +
            tablaLista("Cuadro de Honor Superior", sup) + tablaLista("Cuadro de Honor", hon);
        abrirImpresion("Cuadro de Honor", CSS_LISTA, cuerpo);
    }

    function seleccionados() {
        var ids = $(".chk-alu:checked").map(function () { return parseInt(this.value, 10); }).get();
        if (!ids.length) ids = datos.map(function (a) { return a.Alu_Id; }); // ninguno marcado = todos
        return ids;
    }

    function diplomaHtml(a) {
        var titulo = a.NivelHonor === "S" ? "Cuadro de Honor Superior" : "Cuadro de Honor";
        return '<div class="diploma">' +
            '<h2>' + titulo.toUpperCase() + '</h2>' +
            '<p class="sub">Año ' + esc($("#f-anio").val()) + '</p>' +
            '<p style="margin-top:28px">Se otorga el presente reconocimiento a</p>' +
            '<div class="nombre">' + esc(a.Alu_NombreCompleto) + '</div>' +
            '<p>por su destacado rendimiento académico, con un promedio de <strong>' + prom(a.Promedio) + '</strong> (' + esc(a.Concepto || "") + ').</p>' +
            '<p class="sub">' + ubic(a) + '</p>' +
            '<div class="firma">Dirección</div>' +
            '</div>';
    }

    function imprimirDiplomas() {
        if (!datos.length) { toastr.warning("Primero cargue el cuadro de honor."); return; }
        var cuerpo = seleccionados().map(function (id) { return porId[id]; })
                      .filter(function (a) { return a; })
                      .map(diplomaHtml).join("");
        abrirImpresion("Diplomas · Cuadro de Honor", CSS_DIP, cuerpo);
    }

    function init(options) {
        cfg = $.extend(true, cfg, options || {});
        cargarDropdowns();
        ver();

        $("#btn-ver").on("click", ver);
        $("#btn-imprimir-lista").on("click", imprimirLista);
        $("#btn-imprimir-diplomas").on("click", imprimirDiplomas);
        $(document).on("change", ".chk-all", function () {
            var tier = $(this).data("tier");
            var sel = tier === "superior" ? "#tbody-superior" : "#tbody-honor";
            $(sel + " .chk-alu").prop("checked", this.checked);
        });
    }

    return { init: init };
})();

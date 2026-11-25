/* Cupos de las secciones del año (modal _CuposAnio.cshtml).
   GcCupos.abrir(aniId, nombreAnio, alCambiar): lista las secciones con inscritos y cupo; quien tiene el
   permiso "Editar cupos del año" puede cambiar el cupo (el servidor revisa el permiso y que no sea menor
   que los inscritos). alCambiar() se llama después de guardar un cupo. */
var GcCupos = (function () {
    "use strict";

    var aniId = 0, alCambiar = null, puedeEditar = false;

    function esc(t) { return $("<div>").text(t == null ? "" : String(t)).html(); }
    function $m() { return $("#modalCuposAnio"); }

    function aviso(texto, tipo) {
        var $a = $("#cuposAnioAviso");
        if (!texto) { $a.addClass("d-none").text(""); return; }
        $a.removeClass("d-none alert-light-success alert-light-danger").addClass(tipo === "ok" ? "alert-light-success" : "alert-light-danger").text(texto);
    }

    function fila(x) {
        var nombre = x.Sea_Nombre || [x.Cun_Descripcion, x.Sec_Descripcion].join(" ");
        var libres = x.Libres == null ? "—" : x.Libres;
        var celdaCupo = puedeEditar
            ? '<input type="number" min="' + Math.max(1, x.Inscritos) + '" max="200" class="form-control form-control-sm text-center cupo-input" value="' + (x.Sea_Cupo == null ? "" : x.Sea_Cupo) + '" placeholder="Sin definir" aria-label="Cupo de ' + esc(nombre) + '" data-original="' + (x.Sea_Cupo == null ? "" : x.Sea_Cupo) + '">'
            : (x.Sea_Cupo == null ? '<span class="text-muted">Sin definir</span>' : esc(x.Sea_Cupo));
        var btn = puedeEditar ? '<button type="button" class="btn btn-sm btn-primary cupo-guardar d-none">Guardar</button>' : "";
        var cls = x.Libres != null && x.Libres <= 0 ? ' class="table-warning"' : "";
        return '<tr' + cls + ' data-sea="' + x.Sea_Id + '" data-inscritos="' + x.Inscritos + '">' +
            '<td>' + esc(nombre) + '</td><td class="text-center cupo-inscritos">' + x.Inscritos + '</td>' +
            '<td class="text-center">' + celdaCupo + '</td><td class="text-center cupo-libres">' + libres + '</td><td class="text-right">' + btn + '</td></tr>';
    }

    function cargar() {
        var $b = $("#cuposAnioBody");
        $b.html('<tr><td colspan="5" class="text-center text-muted">Cargando…</td></tr>');
        $.getJSON($m().data("url-lista"), { aniId: aniId }).done(function (r) {
            puedeEditar = !!(r && r.puedeEditar);
            var lista = (r && r.data) || [];
            if (!lista.length) { $b.html('<tr><td colspan="5" class="text-center text-muted">Este año todavía no tiene secciones.</td></tr>'); return; }
            $b.html(lista.map(fila).join(""));
            $("#cuposAnioNota").text(puedeEditar
                ? "El cupo es el máximo de alumnos de la sección. No puede ser menor que los alumnos ya inscritos."
                : "Solo Dirección y Secretaría pueden cambiar los cupos.");
        }).fail(function () {
            $b.html('<tr><td colspan="5" class="text-center text-danger">No se pudieron cargar las secciones.</td></tr>');
        });
    }

    function enlazar() {
    $(document).on("input", "#cuposAnioBody .cupo-input", function () {
        var $i = $(this);
        $i.closest("tr").find(".cupo-guardar").toggleClass("d-none", $i.val() === String($i.attr("data-original")) || $i.val() === "");
    });

    $(document).on("click", "#cuposAnioBody .cupo-guardar", function () {
        var $btn = $(this), $tr = $btn.closest("tr"), $i = $tr.find(".cupo-input");
        var cupo = parseInt($i.val(), 10), ins = parseInt($tr.data("inscritos"), 10);
        aviso("");
        if (isNaN(cupo) || cupo < 1 || cupo > 200) { aviso("El cupo debe ser un número entre 1 y 200.", "error"); return; }
        if (cupo < ins) { aviso("Esa sección ya tiene " + ins + " alumnos inscritos: el cupo no puede ser menor.", "error"); return; }
        $btn.prop("disabled", true);
        $.post($m().data("url-guardar"), { seaId: $tr.data("sea"), cupo: cupo }).done(function (r) {
            if (r && r.ok) {
                $i.attr("data-original", String(cupo));
                $btn.prop("disabled", false).addClass("d-none");
                $tr.find(".cupo-libres").text(r.libres);
                $tr.toggleClass("table-warning", r.libres <= 0);
                aviso("Cupo guardado.", "ok");
                if (typeof alCambiar === "function") alCambiar();
            } else {
                aviso((r && r.mensaje) || "No se pudo cambiar el cupo.", "error");
                $btn.prop("disabled", false);
            }
        }).fail(function () { aviso("No se pudo cambiar el cupo. Intente de nuevo.", "error"); $btn.prop("disabled", false); });
    });

    }
    // jQuery puede cargarse después de este archivo (al final de la plantilla).
    if (window.jQuery) enlazar(); else document.addEventListener("DOMContentLoaded", enlazar);

    return {
        abrir: function (id, nombreAnio, cb) {
            aniId = id; alCambiar = cb || null; aviso("");
            $("#modalCuposAnioTitulo").text("Cupos de las secciones" + (nombreAnio ? " · " + nombreAnio : ""));
            $m().modal("show");
            cargar();
        }
    };
})();

/**
 * BoletinProvisional — aviso "Resultados provisionales" en el boletín (Cierre de parciales).
 * Pide el estado de los parciales del año y, si alguno con notas todavía no está publicado por
 * Dirección, pinta un aviso en el contenedor y devuelve { Pac_Id: true } de los parciales provisionales
 * para que la pantalla marque sus columnas.
 *
 *   BoletinProvisional.marcar("/BoletinAdm/PublicacionAsync", 2026, "#contenedor", function (porPac) { … });
 */
var BoletinProvisional = (function () {
    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function marcar(url, anio, contenedor, alMarcar) {
        var $c = $(contenedor);
        var listo = typeof alMarcar === "function" ? alMarcar : function () { };
        $.getJSON(url, { anio: anio })
            .done(function (r) {
                var periodos = (r && r.data) ? r.data : (Array.isArray(r) ? r : []);
                var pendientes = periodos.filter(function (p) { return p.NotasPeriodo > 0 && !p.Publicado; });
                if (!pendientes.length) { $c.attr("hidden", true).empty(); listo({}); return; }

                var lista = pendientes.map(function (p) {
                    return esc(p.Parcial + " del " + String(p.Semestre || "").toLowerCase());
                }).join(", ");
                $c.html('<div class="alert alert-light-warning mb-4" role="status"><i class="bi bi-hourglass-split mr-1" aria-hidden="true"></i>' +
                    '<strong>Resultados provisionales.</strong> Dirección todavía no publica: ' + lista + ' de ' + esc(anio) +
                    '. Estas notas aún pueden cambiar.</div>').removeAttr("hidden");

                var porPac = {};
                pendientes.forEach(function (p) { porPac[p.Pac_Id] = true; });
                listo(porPac);
            })
            .fail(function () { $c.attr("hidden", true).empty(); listo({}); });
    }

    return { marcar: marcar };
}());

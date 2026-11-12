/* Tarjeta "Próximos 7 días" (Calendario escolar) para los dashboards.
   Sin jQuery: el layout carga jQuery al final del body y esta tarjeta vive dentro de la página. */
(function () {
    if (window.__proximosDias) return;
    window.__proximosDias = true;

    function esc(v) {
        if (v === null || v === undefined) return "";
        return String(v)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }

    function pintar(seccion, lista) {
        var cuerpo = seccion.querySelector(".cal-proximos-cuerpo");
        if (!lista.length) {
            cuerpo.innerHTML = '<p class="cal-proximos-vacio">No hay nada programado.</p>';
            return;
        }
        var base = seccion.getAttribute("data-dia") || "";
        var html = "<ul>";
        lista.forEach(function (e) {
            var detalle = '<span class="cal-tipo ' + esc(e.clase) + '"><i class="bi ' + esc(e.icono) + '" aria-hidden="true"></i> ' + esc(e.tipo) + "</span>";
            if (e.lugar) detalle += " · " + esc(e.lugar);
            if (e.cierra) detalle += ' · <span class="cal-cierra"><i class="bi bi-lock-fill" aria-hidden="true"></i> Sin clases</span>';
            html += "<li>" +
                '<span class="cal-proximos-cuando' + (e.esHoy ? " cal-proximos-cuando--hoy" : "") + '">' + esc(e.fecha) + "<br>" + esc(e.hora) + "</span>" +
                '<span class="cal-proximos-que"><a href="' + esc(base + "/" + e.id) + '">' + esc(e.titulo) + "</a><small>" + detalle + "</small></span>" +
                "</li>";
        });
        cuerpo.innerHTML = html + "</ul>";
    }

    function iniciar() {
        var secciones = document.querySelectorAll(".cal-proximos[data-url]");
        Array.prototype.forEach.call(secciones, function (seccion) {
            fetch(seccion.getAttribute("data-url"), { credentials: "same-origin", headers: { Accept: "application/json" } })
                .then(function (r) {
                    if (!r.ok) throw new Error("HTTP " + r.status);
                    return r.json();
                })
                .then(function (lista) { pintar(seccion, Array.isArray(lista) ? lista : []); })
                .catch(function () {
                    seccion.querySelector(".cal-proximos-cuerpo").innerHTML =
                        '<p class="cal-proximos-vacio">No se pudo cargar el calendario.</p>';
                });
        });
    }

    if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", iniciar);
    else iniciar();
})();

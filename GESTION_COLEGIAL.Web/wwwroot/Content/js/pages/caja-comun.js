/** Ayudas compartidas por las pantallas de caja (Cuentas por cobrar, Movimientos de caja, Cuenta del alumno). */
var Caja = (function () {
    function dinero(v) {
        return "L " + (parseFloat(v) || 0).toLocaleString("es-HN", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }
    function fecha(v) {
        if (!v) return "";
        var d = new Date(v);
        return isNaN(d.getTime()) ? "" : d.toLocaleDateString("es-HN");
    }
    function fechaHora(v) {
        var d = new Date(v);
        return isNaN(d.getTime()) ? "" : d.toLocaleDateString("es-HN") + " " + d.toLocaleTimeString("es-HN", { hour: "2-digit", minute: "2-digit" });
    }
    function iso(d) {
        var m = d.getMonth() + 1, day = d.getDate();
        return d.getFullYear() + "-" + (m < 10 ? "0" : "") + m + "-" + (day < 10 ? "0" : "") + day;
    }
    function esc(t) {
        return $("<div>").text(t == null ? "" : String(t)).html();
    }
    // Lista que viene como [...] o { data: [...] }.
    function filas(r) {
        return Array.isArray(r) ? r : ((r && Array.isArray(r.data)) ? r.data : []);
    }
    // Mensaje al usuario con el aviso del sistema (toastr); tipo: success, error, warning, info.
    function avisar(tipo, texto) {
        if (window.toastr && toastr[tipo]) toastr[tipo](texto);
        else if (window.Swal) Swal.fire({ icon: tipo, title: texto, confirmButtonText: "Entendido" });
        else alert(texto);
    }
    // Mensaje de error de una respuesta AJAX del ADM ({ success, message }).
    function errorDe(xhr) {
        var j = xhr && xhr.responseJSON;
        if (j && j.message) return j.message;
        if (xhr && xhr.status === 403) return "No tiene permiso para hacer esta acción.";
        return "No se pudo completar la operación. Intente de nuevo.";
    }
    return { dinero: dinero, fecha: fecha, fechaHora: fechaHora, iso: iso, esc: esc, filas: filas, avisar: avisar, errorDe: errorDe };
})();

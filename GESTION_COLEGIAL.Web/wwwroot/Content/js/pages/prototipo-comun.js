/** Ayudas compartidas por los prototipos de caja. */
var Proto = (function () {
    function dinero(v) {
        return "L " + (parseFloat(v) || 0).toLocaleString("es-HN", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }
    function fecha(v) {
        var d = new Date(v);
        return isNaN(d.getTime()) ? "" : d.toLocaleDateString("es-HN");
    }
    function iso(d) {
        var m = d.getMonth() + 1, day = d.getDate();
        return d.getFullYear() + "-" + (m < 10 ? "0" : "") + m + "-" + (day < 10 ? "0" : "") + day;
    }
    function normal(t) {
        return (t || "").normalize("NFD").replace(/[̀-ͯ]/g, "").toLowerCase().trim();
    }

    // Alumnos para enlazar una cuenta con su alumno. La lista de cuentas trae solo el nombre
    // completo; aquí se busca el alumno cuyo nombre corto está contenido en ese nombre.
    var alumnos = null, cache = {};
    function cargarAlumnos() {
        if (alumnos) return $.Deferred().resolve(alumnos).promise();
        return $.getJSON("/Prototipos/AlumnosAsync").then(function (r) {
            alumnos = (r || []).map(function (a) {
                a._tokens = normal(a.Alu_Nombre).split(/\s+/).filter(Boolean);
                return a;
            });
            return alumnos;
        });
    }
    function buscarAlumno(nombreCompleto) {
        var clave = normal(nombreCompleto);
        if (clave in cache) return cache[clave];
        var tokens = clave.split(/\s+/), hallado = null;
        for (var i = 0; alumnos && i < alumnos.length; i++) {
            var a = alumnos[i];
            if (a._tokens.length && a._tokens[0] === tokens[0] &&
                a._tokens.every(function (t) { return tokens.indexOf(t) >= 0; })) { hallado = a; break; }
        }
        cache[clave] = hallado;
        return hallado;
    }
    function avisoNoGuarda(accion) {
        var msg = "Prototipo: \"" + accion + "\" todavía no guarda. Si se aprueba, se construye en la API y la base de datos.";
        if (window.Swal) Swal.fire({ icon: "info", title: "Prototipo", text: msg, confirmButtonText: "Entendido" });
        else alert(msg);
    }
    return { dinero: dinero, fecha: fecha, iso: iso, cargarAlumnos: cargarAlumnos, buscarAlumno: buscarAlumno, avisoNoGuarda: avisoNoGuarda };
})();

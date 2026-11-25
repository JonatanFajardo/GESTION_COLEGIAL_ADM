/**
 * BoletinNotas — boletín de calificaciones por alumno y año (lo usan "Notas y boletín › Boletín" y /BoletinAdm).
 * Una columna por semestre × parcial (los dos semestres no se juntan); el selector "Semestre" deja ver
 * todo el año o un solo semestre. Cada período se marca "Provisional" por separado mientras Dirección
 * no lo publique en Cierre de parciales.
 *
 *   BoletinNotas.init({
 *       urlGet, urlAlumnos, urlPublicacion, urlEscala, urlNotaMinima,
 *       avisoProvisional: "#nb-boletin-provisional"
 *   });
 *
 * Requiere en la vista: #f-alu, #f-anio, #f-sem-boletin, #btn-ver, #panel-boletin, #thead-boletin, #tbody-boletin.
 */
var BoletinNotas = (function () {
    var urls = {}, escala = [], notaMinima = 70, datos = [], columnas = [], provisionales = {};

    function esc(s) {
        return String(s === null || s === undefined ? "" : s)
            .replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;").replace(/'/g, "&#39;");
    }
    function esNumero(v) { return v !== null && v !== undefined && v !== "" && !isNaN(parseFloat(v)); }
    function fmtNota(v) { return esNumero(v) ? parseFloat(v).toFixed(2) : "-"; }
    function claseNota(v) {
        return (esNumero(v) && parseFloat(v) < notaMinima) ? ' class="text-center px-2 text-danger font-weight-bold"' : ' class="text-center px-2"';
    }

    // Traduce un promedio al concepto de la escala (el rango con la mayor nota mínima que no supera el promedio).
    function conceptoDe(prom) {
        if (!esNumero(prom)) return "";
        var v = parseFloat(prom), mejor = null;
        $.each(escala, function (_, e) {
            var d = parseFloat(e.Esc_NotaDesde);
            if (v >= d && (mejor === null || d > parseFloat(mejor.Esc_NotaDesde))) mejor = e;
        });
        return mejor ? mejor.Esc_Concepto + (mejor.Esc_Abreviatura ? " (" + mejor.Esc_Abreviatura + ")" : "") : "";
    }

    function clave(r) { return (r.Sem_Id || 0) + "-" + r.Par_Id; }

    // Períodos (semestre × parcial) que tienen notas, en orden.
    function periodos() {
        var vistos = {}, lista = [];
        $.each(datos, function (_, r) {
            var k = clave(r);
            if (vistos[k]) return;
            vistos[k] = true;
            lista.push({
                clave: k, sem: r.Sem_Id || 0, par: r.Par_Id,
                semTxt: r.Sem_Descripcion || "", parTxt: r.Par_Nombre || r.Par_Descripcion || ("Parcial " + r.Par_Id)
            });
        });
        lista.sort(function (a, b) { return a.sem - b.sem || a.par - b.par; });
        return lista;
    }

    function llenarSemestres() {
        var $s = $("#f-sem-boletin"), vistos = {};
        $s.empty().append('<option value="">Todo el año</option>');
        $.each(periodos(), function (_, p) {
            if (p.sem && !vistos[p.sem]) { vistos[p.sem] = true; $s.append('<option value="' + p.sem + '">' + esc(p.semTxt) + '</option>'); }
        });
        $s.prop("disabled", Object.keys(vistos).length < 2);
    }

    function pintar() {
        var sem = $("#f-sem-boletin").val();
        columnas = periodos().filter(function (p) { return !sem || String(p.sem) === String(sem); });

        // Encabezado en dos filas: el semestre arriba (agrupa sus parciales) y cada parcial debajo.
        var fila1 = '<th rowspan="2" class="align-middle px-2">Materia</th>', fila2 = "", grupos = [];
        $.each(columnas, function (_, p) {
            var g = grupos[grupos.length - 1];
            if (g && g.sem === p.sem) g.n++; else grupos.push({ sem: p.sem, txt: p.semTxt, n: 1 });
            var marca = provisionales[p.clave] ? '<div><span class="badge outline-badge-warning">Provisional</span></div>' : "";
            fila2 += '<th class="text-center px-2" data-per="' + esc(p.clave) + '">' + esc(p.parTxt) + marca + "</th>";
        });
        $.each(grupos, function (_, g) { fila1 += '<th colspan="' + g.n + '" class="text-center border-left px-2">' + esc(g.txt) + "</th>"; });
        fila1 += '<th rowspan="2" class="text-center align-middle border-left px-2">Promedio</th><th rowspan="2" class="text-center align-middle px-2">Concepto</th>';
        $("#thead-boletin").closest("thead").html('<tr id="thead-boletin">' + fila1 + "</tr><tr>" + fila2 + "</tr>");

        var tbody = $("#tbody-boletin").empty();
        if (!datos.length || !columnas.length) {
            tbody.append('<tr><td colspan="' + (columnas.length + 3) + '" class="text-center text-muted">Sin notas registradas para este alumno.</td></tr>');
            return;
        }

        var materias = {}, orden = [];
        $.each(datos, function (_, r) {
            var mat = r.Mat_Nombre || "Sin materia";
            if (!materias[mat]) { materias[mat] = {}; orden.push(mat); }
            materias[mat][clave(r)] = r.Not_Nota;
        });

        $.each(orden, function (_, mat) {
            var celdas = '<td class="px-2">' + esc(mat) + "</td>", vals = [];
            $.each(columnas, function (_, p) {
                var nota = materias[mat].hasOwnProperty(p.clave) ? materias[mat][p.clave] : null;
                if (esNumero(nota)) vals.push(parseFloat(nota));
                celdas += "<td" + claseNota(nota) + ">" + fmtNota(nota) + "</td>";
            });
            // Promedio aritmético simple sobre los parciales con nota (del semestre elegido o de todo el año).
            var prom = vals.length ? vals.reduce(function (a, b) { return a + b; }, 0) / vals.length : null;
            celdas += "<td" + claseNota(prom) + "><strong>" + fmtNota(prom) + "</strong></td>";
            celdas += '<td class="text-center text-muted px-2">' + esc(conceptoDe(prom)) + "</td>";
            tbody.append("<tr>" + celdas + "</tr>");
        });
    }

    function ver() {
        var alu = $("#f-alu").val(), anio = $("#f-anio").val();
        if (!alu || !anio) { alert("Seleccione el alumno y el año."); return; }

        $.getJSON(urls.get, { Alu_Id: alu, Anio: anio }, function (res) {
            datos = (res && res.data) ? res.data : (res || []);
            if (!$.isArray(datos)) datos = [];
            provisionales = {};
            llenarSemestres();
            pintar();
            $("#panel-boletin").show();

            // Períodos que Dirección aún no publica: aviso y marca "Provisional" en su columna.
            if (window.BoletinProvisional && urls.publicacion) {
                BoletinProvisional.marcar(urls.publicacion, anio, urls.aviso, function (porPac, porPeriodo) {
                    provisionales = porPeriodo || {};
                    pintar();
                });
            }
        });
    }

    function init(cfg) {
        urls = { get: cfg.urlGet, publicacion: cfg.urlPublicacion, aviso: cfg.avisoProvisional || "#boletin-provisional" };

        $.getJSON(cfg.urlAlumnos, function (data) {
            var sa = $("#f-alu");
            $.each(data.alumnos, function (_, a) { sa.append('<option value="' + esc(a.id) + '">' + esc(a.texto) + "</option>"); });
        });
        if (cfg.urlEscala) {
            $.getJSON(cfg.urlEscala, function (res) { escala = (res && res.data) ? res.data : (res || []); });
        }
        if (cfg.urlNotaMinima) {
            $.getJSON(cfg.urlNotaMinima).done(function (res) {
                if (res && esNumero(res.valor)) notaMinima = parseFloat(res.valor);
            });
        }

        $("#btn-ver").on("click", ver);
        $("#f-sem-boletin").on("change", pintar);
    }

    return { init: init };
}());

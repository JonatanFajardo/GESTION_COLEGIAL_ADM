/**
 * GcTabla — tabla personalizable por usuario sobre Tabulator.
 *
 * Cada usuario puede cambiar el ancho y el orden de las columnas, fijar columnas (pin),
 * elegir la densidad y seleccionar filas para exportarlas. Todo se guarda en el navegador
 * con una clave por tabla y por usuario: "gc-tabla-<nombre>-<usuario>".
 *
 * Uso:
 *   var t = GcTabla.crear({
 *       elemento: "#tabla-empleados",        // <div class="gc-tabla" data-usuario="…">
 *       nombre: "empleados",                 // parte de la clave guardada
 *       url: "/Empleados/ListAsync",         // devuelve [...] o { data: [...] }
 *       columnas: [ { field, title, ... } ], // definición de Tabulator; el orden es el inicial
 *       buscarEn: ["Emp_Nombre", ...],       // campos que revisa el buscador
 *       archivo: "empleados",                // nombre de los archivos exportados
 *       tituloPdf: "Listado de empleados",
 *       buscador: "#mi-input",               // opcional: usar un buscador que ya existe
 *       filtroExtra: function (fila) {},     // opcional: filtros propios de la pantalla
 *       alCargar: function (filas) {}        // opcional: después de cada carga de datos
 *   });
 *   t.filtrar();   // vuelve a aplicar buscador + filtroExtra
 *   t.recargar();  // pide los datos otra vez
 *
 * Requiere: tabulator.min.js (y para exportar: xlsx.full.min.js, jspdf, jspdf-autotable).
 * Los estilos (densidad, pin, selección) están en custom.css bajo .gc-tabla.
 */
var GcTabla = (function () {
    var DENSIDADES = ["comoda", "normal", "compacta"];
    var instancias = [];

    var IDIOMA = {
        es: {
            data: { loading: "Cargando…", error: "No se pudo cargar la información" },
            pagination: {
                page_size: "Filas", page_title: "Mostrar página", first: "Primero", first_title: "Primera página",
                last: "Último", last_title: "Última página", prev: "Anterior", prev_title: "Página anterior",
                next: "Siguiente", next_title: "Página siguiente", all: "Todos",
                counter: { showing: "Mostrando", of: "de", rows: "registros", pages: "páginas" }
            }
        }
    };

    function barraHtml(id, conBuscador) {
        return '' +
            '<div class="d-flex flex-wrap align-items-center justify-content-between mb-2 gc-tabla-barra">' +
            (conBuscador ? '<input type="search" id="' + id + '-buscar" class="form-control form-control-sm w-auto mb-2 mr-2" placeholder="Buscar…" aria-label="Buscar en la tabla" />' : '<span></span>') +
            '  <div class="d-flex flex-wrap align-items-center mb-2">' +
            '    <label for="' + id + '-densidad" class="mb-0 mr-2 text-muted small">Densidad</label>' +
            '    <select id="' + id + '-densidad" class="custom-select custom-select-sm w-auto mr-3">' +
            '      <option value="comoda">Cómoda</option><option value="normal">Normal</option><option value="compacta">Compacta</option>' +
            '    </select>' +
            '    <button type="button" id="' + id + '-recargar" class="btn btn-outline-primary btn-sm mr-1" title="Recargar tabla" aria-label="Recargar tabla"><i class="bi bi-arrow-clockwise"></i></button>' +
            '    <button type="button" id="' + id + '-pdf" class="btn btn-outline-primary btn-sm mr-1"><i class="bi bi-file-earmark-pdf mr-1"></i>PDF</button>' +
            '    <button type="button" id="' + id + '-excel" class="btn btn-outline-primary btn-sm mr-1"><i class="bi bi-file-earmark-spreadsheet mr-1"></i>Excel</button>' +
            '    <button type="button" id="' + id + '-restablecer" class="btn btn-outline-secondary btn-sm" title="Volver al orden, ancho, columnas fijas y densidad originales">Restablecer</button>' +
            '  </div>' +
            '</div>' +
            '<div id="' + id + '-seleccion" class="alert alert-light-primary d-none align-items-center flex-wrap py-2 mb-2 gc-tabla-seleccion" role="status">' +
            '  <strong class="mr-3"><span id="' + id + '-nsel">0</span> seleccionados</strong>' +
            '  <button type="button" id="' + id + '-sel-excel" class="btn btn-primary btn-sm mr-1"><i class="bi bi-file-earmark-spreadsheet mr-1"></i>Exportar a Excel</button>' +
            '  <button type="button" id="' + id + '-sel-pdf" class="btn btn-outline-primary btn-sm mr-1"><i class="bi bi-file-earmark-pdf mr-1"></i>Exportar a PDF</button>' +
            '  <button type="button" id="' + id + '-sel-quitar" class="btn btn-outline-secondary btn-sm">Quitar selección</button>' +
            '</div>';
    }

    function crear(op) {
        var $tabla = $(op.elemento);
        var id = $tabla.attr("id");
        var clave = "gc-tabla-" + op.nombre + "-" + ($tabla.data("usuario") || "anonimo");
        var COLUMNAS = {}, ORDEN_DEFECTO = [];
        op.columnas.forEach(function (c) { COLUMNAS[c.field] = c; ORDEN_DEFECTO.push(c.field); });
        var camposBusqueda = op.buscarEn || ORDEN_DEFECTO;

        $tabla.addClass("gc-tabla").before(barraHtml(id, !op.buscador));
        var $buscar = $(op.buscador || ("#" + id + "-buscar"));
        var $densidad = $("#" + id + "-densidad");

        // ── Estado guardado ──
        function porDefecto() { return { orden: ORDEN_DEFECTO.slice(), anchos: {}, fijas: [], densidad: "normal" }; }
        function leer() {
            try {
                var e = JSON.parse(localStorage.getItem(clave) || "null");
                if (!e || !Array.isArray(e.orden)) return porDefecto();
                e.orden = e.orden.filter(function (f) { return COLUMNAS[f]; });
                ORDEN_DEFECTO.forEach(function (f) { if (e.orden.indexOf(f) < 0) e.orden.push(f); });
                e.fijas = (e.fijas || []).filter(function (f) { return COLUMNAS[f]; });
                e.anchos = e.anchos || {};
                if (DENSIDADES.indexOf(e.densidad) < 0) e.densidad = "normal";
                return e;
            } catch (x) { return porDefecto(); }
        }
        function guardar() { try { localStorage.setItem(clave, JSON.stringify(estado)); } catch (x) { } }
        var estado = leer();

        // ── Encabezado con botón de fijar ──
        function tituloConPin(cell) {
            var col = cell.getColumn(), campo = col.getField();
            var fija = estado.fijas.indexOf(campo) >= 0;
            var cont = document.createElement("span");
            cont.className = "d-inline-flex align-items-center";
            var texto = document.createElement("span");
            texto.textContent = col.getDefinition().title;
            var pin = document.createElement("button");
            pin.type = "button";
            pin.className = "btn btn-link btn-sm p-0 ml-2 gc-tabla-pin" + (fija ? " text-primary" : " text-muted");
            pin.title = fija ? "Soltar columna" : "Fijar columna";
            pin.setAttribute("aria-label", pin.title + " " + texto.textContent);
            pin.innerHTML = '<i class="bi ' + (fija ? "bi-pin-angle-fill" : "bi-pin-angle") + '"></i>';
            pin.addEventListener("click", function (e) { e.stopPropagation(); alternarFija(campo); });
            pin.addEventListener("mousedown", function (e) { e.stopPropagation(); });
            cont.appendChild(texto);
            cont.appendChild(pin);
            return cont;
        }

        // Las columnas fijas van primero: Tabulator las ancla a la izquierda.
        // La columna "Acciones" va siempre al final y fija a la derecha (siempre a mano).
        function columnas() {
            var orden = estado.fijas.concat(estado.orden.filter(function (f) { return estado.fijas.indexOf(f) < 0 && f !== "Acciones"; }));
            if (COLUMNAS.Acciones) orden.push("Acciones");
            return orden.map(function (campo) {
                var def = $.extend({}, COLUMNAS[campo]);
                // Que el título, el pin y la flecha de orden quepan sin cortarse.
                var minimo = Math.round((def.title || "").length * 9 + (def.headerSort === false ? 50 : 86));
                def.minWidth = Math.max(def.minWidth || 90, minimo);
                if (def.width && def.width < def.minWidth) def.width = def.minWidth;
                if (campo === "Acciones") {
                    def.frozen = true;
                    def.headerSort = false;
                    def.movable = false;
                } else {
                    def.titleFormatter = tituloConPin;
                    def.frozen = estado.fijas.indexOf(campo) >= 0;
                }
                if (estado.anchos[campo]) def.width = estado.anchos[campo];
                return def;
            });
        }

        function tomarOrden() {
            estado.orden = tabla.getColumns().map(function (c) { return c.getField(); })
                .filter(function (f) { return COLUMNAS[f]; });
        }
        function alternarFija(campo) {
            var i = estado.fijas.indexOf(campo);
            if (i >= 0) estado.fijas.splice(i, 1); else estado.fijas.push(campo);
            tomarOrden();
            guardar();
            tabla.setColumns(columnas());
        }
        function aplicarDensidad(d) {
            DENSIDADES.forEach(function (x) { $tabla.removeClass("gc-densidad-" + x); });
            $tabla.addClass("gc-densidad-" + d);
            $densidad.val(d);
        }
        aplicarDensidad(estado.densidad);

        // ── Tabla ──
        var tabla = new Tabulator(op.elemento, {
            ajaxURL: op.url,
            ajaxResponse: function (url, params, r) {
                return Array.isArray(r) ? r : ((r && r.data) || []);
            },
            layout: "fitColumns",
            movableColumns: true,
            columnDefaults: { resizable: true, vertAlign: "middle", minWidth: 90 },
            // Casilla para seleccionar filas: un clic en la fila no la selecciona, solo la casilla.
            rowHeader: {
                formatter: "rowSelection", titleFormatter: "rowSelection", titleFormatterParams: { rowRange: "active" },
                headerSort: false, resizable: false, frozen: true, hozAlign: "center", headerHozAlign: "center",
                width: 44, minWidth: 44, download: false
            },
            columns: columnas(),
            pagination: true,
            paginationSize: op.filasPorPagina || 20,
            paginationSizeSelector: [10, 20, 50, 100, true],
            paginationCounter: "rows",
            placeholder: op.vacio || "Ningún dato disponible en esta tabla",
            locale: "es",
            langs: IDIOMA
        });

        tabla.on("columnMoved", function () {
            tomarOrden();
            var fijasAlInicio = estado.orden.slice(0, estado.fijas.length)
                .every(function (f) { return estado.fijas.indexOf(f) >= 0; });
            guardar();
            if (!fijasAlInicio) tabla.setColumns(columnas());
        });
        tabla.on("columnResized", function (col) {
            estado.anchos[col.getField()] = Math.round(col.getWidth());
            guardar();
        });
        tabla.on("rowSelectionChanged", function (data) {
            $("#" + id + "-nsel").text(data.length);
            $("#" + id + "-seleccion").toggleClass("d-none", data.length === 0).toggleClass("d-flex", data.length > 0);
        });
        tabla.on("dataLoaded", function (filas) { if (op.alCargar) op.alCargar(filas); });

        // ── Buscador + filtros de la pantalla ──
        function filtrar() {
            var q = ($buscar.val() || "").toString().trim().toLowerCase();
            if (!q && !op.filtroExtra) { tabla.clearFilter(); return; }
            tabla.setFilter(function (d) {
                if (op.filtroExtra && !op.filtroExtra(d)) return false;
                if (!q) return true;
                return camposBusqueda.some(function (f) {
                    var v = d[f];
                    return v !== null && v !== undefined && v.toString().toLowerCase().indexOf(q) >= 0;
                });
            });
        }
        $buscar.on("input", filtrar);

        // ── Barra ──
        function exportar(tipo, rango) {
            var archivo = (op.archivo || op.nombre) + (rango === "selected" ? "-seleccion" : "");
            if (tipo === "xlsx") tabla.download("xlsx", archivo + ".xlsx", { sheetName: op.tituloPdf || op.nombre }, rango);
            else tabla.download("pdf", archivo + ".pdf", { orientation: "landscape", title: op.tituloPdf || "" }, rango);
        }
        $densidad.on("change", function () {
            estado.densidad = $(this).val();
            aplicarDensidad(estado.densidad);
            guardar();
            tabla.redraw(true);
        });
        $("#" + id + "-recargar").on("click", function () { tabla.replaceData(); });
        $("#" + id + "-excel").on("click", function () { exportar("xlsx", "active"); });
        $("#" + id + "-pdf").on("click", function () { exportar("pdf", "active"); });
        $("#" + id + "-sel-excel").on("click", function () { exportar("xlsx", "selected"); });
        $("#" + id + "-sel-pdf").on("click", function () { exportar("pdf", "selected"); });
        $("#" + id + "-sel-quitar").on("click", function () { tabla.deselectRow(); });
        $("#" + id + "-restablecer").on("click", function () {
            try { localStorage.removeItem(clave); } catch (x) { }
            estado = porDefecto();
            aplicarDensidad(estado.densidad);
            tabla.setColumns(columnas());
            tabla.redraw(true);
        });

        var api = { tabla: tabla, filtrar: filtrar, recargar: function () { tabla.replaceData(); }, clave: clave };
        instancias.push(api);
        return api;
    }

    // Recarga todas las tablas de la página (lo usan las ventanas de eliminar al terminar).
    function recargar() {
        instancias.forEach(function (t) { t.recargar(); });
        return instancias.length > 0;
    }

    // Ícono de acciones reutilizable (mismo estilo que el resto del sistema).
    var ICONOS = {
        ver: '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-eye p-1 br-6 mb-1"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>',
        editar: '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 p-1 br-6 mb-1"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg>',
        eliminar: '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-trash p-1 br-6 mb-1"><polyline points="3 6 5 6 21 6"></polyline><path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path></svg>'
    };

    /** Botones ver / editar / eliminar de una fila. extraEliminar: atributos extra del botón eliminar. */
    function acciones(id, permisos, extraEliminar) {
        var b = '<div class="d-flex justify-content-center align-items-center text-nowrap">';
        b += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip detail-btn text-muted pr-2" title="Ver detalles">' + ICONOS.ver + '</a>';
        if (permisos.editar) b += '<a href="javascript:void(0);" data-id="' + id + '" class="bs-tooltip edit-btn text-muted pr-2" title="Editar">' + ICONOS.editar + '</a>';
        if (permisos.eliminar) {
            b += '<span class="border-left mx-1"></span>';
            b += '<a href="javascript:void(0);" ' + (extraEliminar || '') + ' data-id="' + id + '" class="bs-tooltip delete-btn text-muted pl-2" title="Eliminar">' + ICONOS.eliminar + '</a>';
        }
        return b + '</div>';
    }

    /** Avatar de CORK: foto redonda o, si falta, las iniciales. */
    function avatar(nombre, foto) {
        var ini = (nombre || "").split(" ").filter(Boolean).slice(0, 2)
            .map(function (p) { return p.charAt(0); }).join("").toUpperCase();
        var dentro = foto
            ? '<img class="rounded-circle" src="' + foto.replace("~/", "/") + '" alt="" data-ini="' + ini + '" onerror="GcTabla.sinFoto(this)">'
            : '<span class="avatar-title rounded-circle">' + ini + '</span>';
        return '<div class="d-flex align-items-center"><div class="avatar avatar-sm mr-2">' + dentro + '</div><span>' + (nombre || "") + '</span></div>';
    }
    function sinFoto(img) {
        img.outerHTML = '<span class="avatar-title rounded-circle">' + (img.getAttribute("data-ini") || "") + '</span>';
    }

    return { crear: crear, recargar: recargar, acciones: acciones, avatar: avatar, sinFoto: sinFoto };
}());

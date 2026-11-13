var DirectorioContacto = (function () {
    var cfg = {};
    var state = {
        tab: "alumno",          // "alumno" | "encargado"
        view: "cards",          // "cards" | "table"
        alumnos: [],
        encargados: [],
        curFilter: "",
        secFilter: "",
        search: "",
        allAlumnosLoaded: false // cache: si ya cargamos el set completo
    };
    var dtAlumno = null, dtEncargado = null;
    var searchDebounce = null;

    function init(config) {
        cfg = config;
        wireTabs();
        wireViewToggle();
        wireSearch();
        wireReload();
        loadEncargados();
        loadCursosFilter();
        showEmptyAlumnoHint();
    }

    function showEmptyAlumnoHint() {
        $("#dc-cards-grid-alumno").empty();
        $("#dc-cards-alumno").hide();
        $("#dc-table-alumno").hide();
        $("#dc-empty-alumno")
            .html('<i class="bi bi-search-heart h2 d-block"></i>' +
                  '<h5>Busca o filtra para empezar</h5>' +
                  '<p><strong>Escribe</strong> el nombre del alumno o encargado en el buscador, o elige un <strong>curso</strong> arriba. Evitamos cargar todo el directorio a la vez para no afectar el rendimiento.</p>')
            .show();
        if (!state.allAlumnosLoaded) {
            $("#tab-count-alumno").text("—");
            $("#kpi-total-alumnos").text("—");
        }
    }

    function showLoadingAlumno(title, subtitle) {
        title = title || 'Cargando…';
        subtitle = subtitle || 'Buscando alumnos que coincidan con los filtros.';
        $("#dc-cards-alumno").hide();
        $("#dc-table-alumno").hide();
        $("#dc-empty-alumno")
            .html('<i class="bi bi-hourglass-split h2 d-block"></i><h5>' + esc(title) + '</h5><p>' + esc(subtitle) + '</p>')
            .show();
    }

    function showNoResultsAlumno() {
        $("#dc-cards-alumno").hide();
        $("#dc-table-alumno").hide();
        $("#dc-empty-alumno")
            .html('<i class="bi bi-inbox h2 d-block"></i><h5>Sin resultados</h5><p>No se encontraron alumnos con los filtros actuales.</p>')
            .show();
    }

    // ---------------- Tabs ----------------
    function wireTabs() {
        $("#dc-tabs .ci-tab-item").on("click", function (e) {
            e.preventDefault();
            var target = $(this).data("target");
            if (state.tab === target) return;
            $("#dc-tabs .ci-tab-item").removeClass("active");
            $(this).addClass("active");
            $("#dc-tabs .nav-link").removeClass("active");
            $(this).find(".nav-link").addClass("active");
            state.tab = target;
            $("#dc-tab-alumno").toggle(target === "alumno");
            $("#dc-tab-encargado").toggle(target === "encargado");
            $("#dc-filters").toggle(target === "alumno");
            $("#dc-new-encargado").toggle(target === "encargado");
            $("#dc-search").val("");
            state.search = "";
            renderActive();
        });
    }

    // ---------------- View toggle ----------------
    function wireViewToggle() {
        $(".ci-view-toggle .ci-view-btn").on("click", function () {
            var view = $(this).data("view");
            if (state.view === view) return;
            $(".ci-view-toggle .ci-view-btn").removeClass("active");
            $(this).addClass("active");
            state.view = view;
            applyViewVisibility();
            renderActive();
        });
    }

    // La visibilidad de cards/tabla vs empty state la maneja renderActive()
    function applyViewVisibility() { /* no-op, kept for compatibility */ }

    function wireSearch() {
        $("#dc-search").on("input", function () {
            var val = ($(this).val() || "").toLowerCase().trim();
            state.search = val;
            clearTimeout(searchDebounce);
            searchDebounce = setTimeout(function () {
                // Si estamos en tab alumno y el usuario empezó a buscar sin filtro,
                // cargamos el set completo una vez y filtramos client-side.
                if (state.tab === "alumno" && state.search.length >= 2 && !state.allAlumnosLoaded && !state.curFilter && !state.secFilter) {
                    loadAllAlumnos();
                } else {
                    renderActive();
                }
            }, 250);
        });
    }

    function loadAllAlumnos() {
        showLoadingAlumno('Cargando directorio completo…', 'Esto solo pasa la primera vez que buscás.');
        $.getJSON(cfg.urlAlumnos, function (resp) {
            state.alumnos = (resp && resp.data) ? resp.data : [];
            state.allAlumnosLoaded = true;
            $("#tab-count-alumno").text(state.alumnos.length);
            $("#kpi-total-alumnos").text(state.alumnos.length);
            renderActive();
        }).fail(function () {
            showNoResultsAlumno();
        });
    }

    function wireReload() {
        $("#dc-reload").on("click", function () {
            if (state.tab === "alumno") loadAlumnos();
            else loadEncargados();
        });
    }

    // ---------------- Data loading ----------------
    function loadAlumnos() {
        if (!state.curFilter && !state.secFilter) {
            state.alumnos = [];
            showEmptyAlumnoHint();
            return;
        }
        var params = {};
        if (state.curFilter) params.Cur_Id = state.curFilter;
        if (state.secFilter) params.Sec_Id = state.secFilter;
        showLoadingAlumno();
        $.getJSON(cfg.urlAlumnos, params, function (resp) {
            state.alumnos = (resp && resp.data) ? resp.data : [];
            $("#tab-count-alumno").text(state.alumnos.length);
            $("#kpi-total-alumnos").text(state.alumnos.length);
            if (state.tab === "alumno") renderActive();
        }).fail(function () {
            showNoResultsAlumno();
        });
    }

    function loadEncargados() {
        $.getJSON(cfg.urlEncargados, function (resp) {
            state.encargados = (resp && resp.data) ? resp.data : [];
            $("#tab-count-encargado").text(state.encargados.length);
            $("#kpi-total-encargados").text(state.encargados.length);
            var sinCorreo = state.encargados.filter(function (e) { return !e.Per_CorreoElectronico; }).length;
            var sinTel = state.encargados.filter(function (e) { return !e.Per_Telefono; }).length;
            $("#kpi-sin-correo").text(sinCorreo);
            $("#kpi-sin-telefono").text(sinTel);
            if (state.tab === "encargado") renderActive();
        });
    }

    function loadCursosFilter() {
        $.getJSON(cfg.urlCursos, function (resp) {
            var cursos = (resp && resp.data) ? resp.data : [];
            var $chips = $("#dc-chips-curso");
            $chips.empty();
            $chips.append('<span class="btn btn-sm btn-rounded btn-outline-primary mr-2 mb-2 grad-chip active" data-cur="">Todos los cursos</span>');
            cursos.forEach(function (c) {
                var id = c.Cun_Id || c.Cur_Id || c.id || "";
                var txt = c.Cun_Descripcion || c.Cur_Descripcion || c.Cur_Nombre || c.descripcion || "";
                if (!id || !txt) return;
                $chips.append('<span class="btn btn-sm btn-rounded btn-outline-primary mr-2 mb-2 grad-chip" data-cur="' + id + '">' + esc(txt) + '</span>');
            });
            $chips.find(".grad-chip").on("click", function () {
                $chips.find(".grad-chip").removeClass("active");
                $(this).addClass("active");
                state.curFilter = $(this).attr("data-cur") || "";
                state.secFilter = "";
                loadSeccionesFilter(state.curFilter);
                loadAlumnos();
            });
        });
    }

    function loadSeccionesFilter(curId) {
        var $chips = $("#dc-chips-seccion");
        $chips.empty();
        $chips.append('<span class="btn btn-sm btn-rounded btn-outline-primary mr-2 mb-2 grad-chip active" data-sec="">Todas las secciones</span>');
        if (!curId) {
            wireSeccionChips();
            return;
        }
        $.getJSON(cfg.urlSecciones, { Cur_Id: curId }, function (resp) {
            var secs = (resp && resp.data) ? resp.data : [];
            secs.forEach(function (s) {
                var id = s.Sec_Id || s.id || "";
                var txt = s.Sec_Descripcion || s.descripcion || "";
                if (!id || !txt) return;
                $chips.append('<span class="btn btn-sm btn-rounded btn-outline-primary mr-2 mb-2 grad-chip" data-sec="' + id + '">' + esc(txt) + '</span>');
            });
            wireSeccionChips();
        });
    }

    function wireSeccionChips() {
        var $chips = $("#dc-chips-seccion");
        $chips.find(".grad-chip").off("click").on("click", function () {
            $chips.find(".grad-chip").removeClass("active");
            $(this).addClass("active");
            state.secFilter = $(this).attr("data-sec") || "";
            loadAlumnos();
        });
    }

    // ---------------- Rendering ----------------
    function renderActive() {
        if (state.tab === "alumno") {
            var noFilter = !state.curFilter && !state.secFilter;
            var noSearch = !state.search;
            if (noFilter && noSearch) {
                showEmptyAlumnoHint();
                return;
            }
            var data = filterAlumnos(state.alumnos);
            if (data.length === 0) {
                showNoResultsAlumno();
                return;
            }
            $("#dc-empty-alumno").hide();
            $("#dc-cards-alumno").toggle(state.view === "cards");
            $("#dc-table-alumno").toggle(state.view === "table");
            if (state.view === "cards") renderAlumnoCards(data);
            else renderAlumnoTable(data);
        } else {
            var data2 = filterEncargados(state.encargados);
            if (data2.length === 0) {
                $("#dc-cards-encargado").hide();
                $("#dc-table-encargado").hide();
                $("#dc-empty-encargado").show();
                return;
            }
            $("#dc-empty-encargado").hide();
            $("#dc-cards-encargado").toggle(state.view === "cards");
            $("#dc-table-encargado").toggle(state.view === "table");
            if (state.view === "cards") renderEncargadoCards(data2);
            else renderEncargadoTable(data2);
        }
    }

    function filterAlumnos(list) {
        if (!state.search) return list;
        return list.filter(function (a) {
            var hay = [a.Alu_NombreCompleto, a.Alu_Identidad, a.Enc_NombreCompleto, a.Enc_Telefono, a.Enc_Correo, a.Cur_Nombre, a.Sec_Descripcion]
                .join(" ").toLowerCase();
            return hay.indexOf(state.search) >= 0;
        });
    }

    function filterEncargados(list) {
        if (!state.search) return list;
        return list.filter(function (e) {
            var hay = [e.Enc_Nombre, e.Per_Identidad, e.Per_Telefono, e.Per_CorreoElectronico, e.Enc_Ocupacion]
                .join(" ").toLowerCase();
            return hay.indexOf(state.search) >= 0;
        });
    }

    function initials(name) {
        if (!name) return "?";
        var parts = String(name).trim().split(/\s+/);
        if (parts.length >= 2) return (parts[0][0] + parts[1][0]).toUpperCase();
        return name.substring(0, 2).toUpperCase();
    }

    function esc(v) {
        return $("<div/>").text(v == null ? "" : String(v)).html();
    }

    function telLink(tel) {
        if (!tel) return '<span class="badge outline-badge-warning mr-1 mb-1"><i class="bi bi-exclamation-triangle mr-1"></i>Sin tel.</span>';
        return '<a class="btn btn-sm btn-outline-primary mr-1 mb-1" href="tel:' + esc(tel) + '"><i class="bi bi-telephone-fill mr-1"></i>' + esc(tel) + '</a>';
    }

    function mailLink(mail) {
        if (!mail) return '<span class="badge outline-badge-warning mr-1 mb-1"><i class="bi bi-envelope-slash mr-1"></i>Sin correo</span>';
        return '<a class="btn btn-sm btn-outline-primary mr-1 mb-1 text-truncate mw-100" href="mailto:' + esc(mail) + '"><i class="bi bi-envelope-fill mr-1"></i>' + esc(mail) + '</a>';
    }

    // -------- Alumno cards --------
    function renderAlumnoCards(list) {
        var $grid = $("#dc-cards-grid-alumno").empty();
        list.forEach(function (a) {
            var card = '' +
                '<div class="col-xl-4 col-md-6 mb-4">' +
                '<div class="border rounded p-3 h-100">' +
                '  <div class="d-flex align-items-center mb-2">' +
                '    <div class="dc-avatar rounded-circle bg-primary text-white font-weight-bold d-flex align-items-center justify-content-center mr-2">' + esc(initials(a.Alu_NombreCompleto)) + '</div>' +
                '    <div class="text-truncate">' +
                '      <div class="font-weight-bold text-truncate">' + esc(a.Alu_NombreCompleto) + '</div>' +
                '      <div class="text-muted small"><i class="bi bi-mortarboard mr-1"></i>' + esc(a.Cur_Nombre) + ' &middot; Sec. ' + esc(a.Sec_Descripcion) + '</div>' +
                '    </div>' +
                '  </div>' +
                '  <div class="small mb-2">' +
                '    <div class="mb-1"><i class="bi bi-card-text text-muted mr-1"></i>' + esc(a.Alu_Identidad || "—") + '</div>' +
                '    <div><i class="bi bi-person-badge text-muted mr-1"></i>Encargado: <strong>' + esc(a.Enc_NombreCompleto || "—") + '</strong></div>' +
                '  </div>' +
                '  <div class="d-flex flex-wrap">' +
                       telLink(a.Enc_Telefono) +
                       mailLink(a.Enc_Correo) +
                '  </div>' +
                '</div>' +
                '</div>';
            $grid.append(card);
        });
    }

    // -------- Alumno table --------
    function renderAlumnoTable(list) {
        if (dtAlumno) {
            dtAlumno.clear().rows.add(list).draw();
            return;
        }
        dtAlumno = $("#dc-datatable-alumno").DataTable({
            data: list,
            deferRender: true,
            language: { url: "https://cdn.datatables.net/plug-ins/1.10.20/i18n/Spanish.json" },
            pageLength: 25,
            order: [],
            columns: [
                { data: "Alu_Identidad" },
                { data: "Alu_NombreCompleto" },
                { data: "Cur_Nombre" },
                { data: "Sec_Descripcion" },
                { data: "Enc_NombreCompleto" },
                {
                    data: "Enc_Telefono",
                    render: function (d) { return d ? '<a href="tel:' + esc(d) + '">' + esc(d) + '</a>' : '<span class="text-muted">—</span>'; }
                },
                {
                    data: "Enc_Correo",
                    render: function (d) { return d ? '<a href="mailto:' + esc(d) + '">' + esc(d) + '</a>' : '<span class="text-muted">—</span>'; }
                },
                {
                    data: null, orderable: false,
                    render: function (row) {
                        var btns = "";
                        if (row.Enc_Telefono) btns += '<a class="btn btn-sm btn-outline-primary mr-1" href="tel:' + esc(row.Enc_Telefono) + '" title="Llamar"><i class="bi bi-telephone"></i></a>';
                        if (row.Enc_Correo) btns += '<a class="btn btn-sm btn-outline-primary mr-1" href="mailto:' + esc(row.Enc_Correo) + '" title="Enviar correo"><i class="bi bi-envelope"></i></a>';
                        return btns || '<span class="text-muted">—</span>';
                    }
                }
            ]
        });
    }

    // -------- Encargado cards --------
    function renderEncargadoCards(list) {
        var $grid = $("#dc-cards-grid-encargado").empty();
        list.forEach(function (e) {
            var estado = e.EsActivo ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-secondary">Inactivo</span>';
            var card = '' +
                '<div class="col-xl-4 col-md-6 mb-4">' +
                '<div class="border rounded p-3 h-100">' +
                '  <div class="d-flex align-items-center mb-2">' +
                '    <div class="dc-avatar rounded-circle bg-success text-white font-weight-bold d-flex align-items-center justify-content-center mr-2">' + esc(initials(e.Enc_Nombre)) + '</div>' +
                '    <div class="text-truncate">' +
                '      <div class="font-weight-bold text-truncate">' + esc(e.Enc_Nombre) + '</div>' +
                '      <div class="text-muted small"><i class="bi bi-card-text mr-1"></i>' + esc(e.Per_Identidad || "—") + ' &middot; ' + estado + '</div>' +
                '    </div>' +
                '  </div>' +
                '  <div class="small mb-2">' +
                '    <div><i class="bi bi-briefcase text-muted mr-1"></i>' + esc(e.Enc_Ocupacion || "Sin ocupación") + '</div>' +
                '  </div>' +
                '  <div class="d-flex flex-wrap mb-2">' +
                       telLink(e.Per_Telefono) +
                       mailLink(e.Per_CorreoElectronico) +
                '  </div>' +
                '  <div class="border-top pt-2 text-right">' +
                '    <a class="btn btn-sm btn-outline-primary mr-1" href="' + cfg.urlEncDetalle + '/' + e.Enc_Id + '" title="Ver detalle"><i class="bi bi-eye"></i></a>' +
                '    <a class="btn btn-sm btn-outline-primary mr-1" href="' + cfg.urlEncEditar + '/' + e.Enc_Id + '" title="Editar"><i class="bi bi-pencil"></i></a>' +
                '  </div>' +
                '</div>' +
                '</div>';
            $grid.append(card);
        });
    }

    // -------- Encargado table --------
    function renderEncargadoTable(list) {
        if (dtEncargado) {
            dtEncargado.clear().rows.add(list).draw();
            return;
        }
        dtEncargado = $("#dc-datatable-encargado").DataTable({
            data: list,
            deferRender: true,
            language: { url: "https://cdn.datatables.net/plug-ins/1.10.20/i18n/Spanish.json" },
            pageLength: 25,
            order: [[2, "asc"]],
            columnDefs: [{ targets: 0, visible: false }],
            columns: [
                { data: "Enc_Id" },
                { data: "Per_Identidad" },
                { data: "Enc_Nombre" },
                {
                    data: "Per_Telefono",
                    render: function (d) { return d ? '<a href="tel:' + esc(d) + '">' + esc(d) + '</a>' : '<span class="text-muted">—</span>'; }
                },
                {
                    data: "Per_CorreoElectronico",
                    render: function (d) { return d ? '<a href="mailto:' + esc(d) + '">' + esc(d) + '</a>' : '<span class="text-muted">—</span>'; }
                },
                { data: "Enc_Ocupacion", defaultContent: "—" },
                {
                    data: "EsActivo",
                    render: function (d) { return d ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-secondary">Inactivo</span>'; }
                },
                {
                    data: null, orderable: false,
                    render: function (row) {
                        return '' +
                            '<a class="btn btn-sm btn-outline-primary mr-1" href="' + cfg.urlEncDetalle + '/' + row.Enc_Id + '" title="Ver"><i class="bi bi-eye"></i></a>' +
                            '<a class="btn btn-sm btn-outline-primary mr-1" href="' + cfg.urlEncEditar + '/' + row.Enc_Id + '" title="Editar"><i class="bi bi-pencil"></i></a>';
                    }
                }
            ]
        });
    }

    return { init: init };
}());

var CentroAnuncios = (function () {
    var obj = {};
    var urlList, urlCreate, urlDelete, urlOpciones;
    var todosLosAnuncios = [];
    var filtroActivo = "todos";

    obj.init = function (ul, uc, ud, uo) {
        urlOpciones = uo;
        urlList   = ul;
        urlCreate = uc;
        urlDelete = ud;

        cargarAnuncios();
        iniciarPublico();

        $(document).on("click", "#tabs-anuncios .nav-link", function () {
            $("#tabs-anuncios .nav-link").removeClass("active");
            $(this).addClass("active");
            filtroActivo = $(this).data("filtro");
            renderCards();
        });

        $("#form-nuevo").on("submit", function (e) {
            e.preventDefault();
            var titulo    = $("input[name='Anu_Titulo']").val().trim();
            var contenido = $("textarea[name='Anu_Contenido']").val().trim();
            if (!titulo || !contenido) {
                alert("El título y el contenido son obligatorios.");
                return;
            }
            $.post(urlCreate, $(this).serialize(), function (res) {
                if (res && res.success) {
                    $("#modal-nuevo").modal("hide");
                    $("#form-nuevo")[0].reset();
                    reiniciarPublico();
                    cargarAnuncios();
                }
            });
        });

        $(document).on("click", ".btn-eliminar-anuncio", function () {
            var id = $(this).data("id");
            if (!confirm("¿Está seguro de que desea eliminar este anuncio?")) return;
            $.post(urlDelete, { Anu_Id: id }, function (res) {
                if (res && res.success) cargarAnuncios();
            });
        });
    };

    // Público del anuncio: Nivel > Grado > Sección en cascada estricta
    function vaciar($sel, texto, deshabilitar) {
        $sel.empty().append($("<option>").val("").text(texto)).prop("disabled", deshabilitar);
    }

    function llenar($sel, tipo, padre, texto, cb) {
        vaciar($sel, texto, true);
        $.getJSON(urlOpciones, { tipo: tipo, padre: padre }, function (res) {
            (res.data || []).forEach(function (o) {
                $sel.append($("<option>").val(o.Id).text(o.Texto));
            });
            if (cb) cb();
        });
    }

    function reiniciarPublico() {
        $("#anu-familias").hide();
        vaciar($("#anu-grado"), "Todos los grados", true);
        vaciar($("#anu-seccion"), "Todas las secciones", true);
        llenar($("#anu-nivel"), "nivel", null, "Todas las familias", function () { $("#anu-nivel").prop("disabled", false); });
    }

    function iniciarPublico() {
        reiniciarPublico();
        $("#anu-publico").on("change", function () {
            $("#anu-familias").toggle($(this).val() === "familias");
        });
        $("#anu-nivel").on("change", function () {
            var v = $(this).val();
            vaciar($("#anu-seccion"), "Todas las secciones", true);
            vaciar($("#anu-grado"), "Todos los grados", true);
            if (!v) return;
            llenar($("#anu-grado"), "grado", v, "Todos los grados", function () { $("#anu-grado").prop("disabled", false); });
        });
        $("#anu-grado").on("change", function () {
            var v = $(this).val();
            vaciar($("#anu-seccion"), "Todas las secciones", true);
            if (!v) return;
            llenar($("#anu-seccion"), "seccion", v, "Todas las secciones", function () { $("#anu-seccion").prop("disabled", false); });
        });
    }

    function insigniaPublico(a) {
        var cls = a.Anu_Publico === "personal" ? "badge-secondary" : (a.Anu_Publico === "familias" ? "badge-info" : "badge-primary");
        var ico = a.Anu_Publico === "personal" ? "bi-person-badge" : (a.Anu_Publico === "familias" ? "bi-people" : "bi-building");
        return '<span class="badge ' + cls + '"><i class="bi ' + ico + ' mr-1"></i>' + escapeHtml(a.PublicoTexto || "") + '</span>';
    }

    function cargarAnuncios() {
        $.ajax({
            url: urlList,
            dataType: "json",
            success: function (res) {
                todosLosAnuncios = res.data || [];
                renderCards();
            }
        });
    }

    function renderCards() {
        var vigentes  = todosLosAnuncios.filter(function (a) { return a.EsVigente == 1; });
        var expirados = todosLosAnuncios.filter(function (a) { return a.EsVigente != 1; });

        var lista;
        if      (filtroActivo === "vigentes")  lista = vigentes;
        else if (filtroActivo === "expirados") lista = expirados;
        else                                   lista = todosLosAnuncios;

        // Actualizar contadores en tabs
        $("[data-filtro='todos'] .tab-count").text(todosLosAnuncios.length);
        $("[data-filtro='vigentes'] .tab-count").text(vigentes.length);
        $("[data-filtro='expirados'] .tab-count").text(expirados.length);

        var container = $("#cards-anuncios");
        var vacio     = $("#anuncios-vacio");

        if (lista.length === 0) {
            container.html("");
            vacio.show();
            return;
        }
        vacio.hide();

        var html = "";
        lista.forEach(function (a) {
            var esVigente = a.EsVigente == 1;
            var badge     = esVigente
                ? '<span class="badge badge-success">Vigente</span>'
                : '<span class="badge badge-secondary">Expirado</span>';

            var diasHtml = "";
            if (esVigente && a.Anu_FechaExpiracion) {
                var dias = Math.ceil((new Date(a.Anu_FechaExpiracion) - new Date()) / 86400000);
                var clsDias = dias <= 7 ? "outline-badge-danger" : "outline-badge-info";
                diasHtml = '<div class="mt-1"><span class="badge ' + clsDias + '"><i class="fa-solid fa-clock fa-xs mr-1"></i>' + dias + ' días restantes</span></div>';
            }

            var fechaPub = a.Anu_FechaPublicacion
                ? (typeof a.Anu_FechaPublicacion === "string" ? a.Anu_FechaPublicacion.substring(0, 10) : new Date(a.Anu_FechaPublicacion).toISOString().substring(0, 10))
                : "";

            html += '<div class="col-lg-4 col-md-6 mb-4">'
                +   '<div class="card component-card_1 h-100' + (esVigente ? ' border-primary' : '') + '">'
                +     '<div class="card-body d-flex flex-column">'
                +       '<div class="d-flex justify-content-between align-items-center mb-3">'
                +         '<i class="fa-solid fa-bullhorn ' + (esVigente ? 'text-primary' : 'text-muted') + ' h4 mb-0"></i>'
                +         badge
                +       '</div>'
                +       '<div class="mb-2">' + insigniaPublico(a) + '</div>'
                +       '<h5 class="card-title">' + escapeHtml(a.Anu_Titulo) + '</h5>'
                +       '<p class="card-text text-muted flex-grow-1">' + escapeHtml(a.Anu_Contenido) + '</p>'
                +       '<hr class="my-2">'
                +       '<div class="small text-muted">'
                +         '<div><i class="fa-solid fa-calendar-days mr-1"></i>' + fechaPub + '</div>'
                +         '<div><i class="fa-solid fa-user mr-1"></i>' + escapeHtml(a.PublicadoPor || '') + '</div>'
                +         diasHtml
                +       '</div>'
                +       '<div class="text-right mt-3">'
                +         '<button type="button" class="btn btn-sm btn-outline-danger btn-eliminar-anuncio" data-id="' + a.Anu_Id + '">'
                +           '<i class="fa-solid fa-trash fa-xs mr-1"></i> Eliminar'
                +         '</button>'
                +       '</div>'
                +     '</div>'
                +   '</div>'
                + '</div>';
        });

        container.html(html);
    }

    function escapeHtml(str) {
        if (!str) return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    return obj;
}());

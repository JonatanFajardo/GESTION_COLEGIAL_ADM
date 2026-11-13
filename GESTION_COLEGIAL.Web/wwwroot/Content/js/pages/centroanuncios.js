var CentroAnuncios = (function () {
    var obj = {};
    var urlList, urlCreate, urlDelete;
    var todosLosAnuncios = [];
    var filtroActivo = "todos";

    obj.init = function (ul, uc, ud) {
        urlList   = ul;
        urlCreate = uc;
        urlDelete = ud;

        cargarAnuncios();

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

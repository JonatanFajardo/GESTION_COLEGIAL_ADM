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
            var cls       = esVigente ? "vigente" : "expirado";
            var badge     = esVigente
                ? '<span class="badge-vigente">Vigente</span>'
                : '<span class="badge-expirado">Expirado</span>';

            var diasHtml = "";
            if (esVigente && a.Anu_FechaExpiracion) {
                var dias = Math.ceil((new Date(a.Anu_FechaExpiracion) - new Date()) / 86400000);
                var clsDias = dias <= 7 ? "dias-urgente" : "dias-ok";
                diasHtml = '<span class="dias-restantes ' + clsDias + '"><i class="fa-solid fa-clock fa-xs"></i> ' + dias + ' días restantes</span>';
            }

            var fechaPub = a.Anu_FechaPublicacion
                ? (typeof a.Anu_FechaPublicacion === "string" ? a.Anu_FechaPublicacion.substring(0, 10) : new Date(a.Anu_FechaPublicacion).toISOString().substring(0, 10))
                : "";

            html += '<div class="col-lg-4 col-md-6 mb-4">'
                +   '<div class="anuncio-card ' + cls + '">'
                +     '<div class="anuncio-accent"></div>'
                +     '<div class="anuncio-body">'
                +       '<div class="anuncio-top-row">'
                +         '<div class="anuncio-icon-wrap"><i class="fa-solid fa-bullhorn"></i></div>'
                +         badge
                +       '</div>'
                +       '<div class="anuncio-titulo">' + escapeHtml(a.Anu_Titulo) + '</div>'
                +       '<div class="anuncio-contenido">' + escapeHtml(a.Anu_Contenido) + '</div>'
                +       '<hr class="anuncio-divider">'
                +       '<div class="anuncio-meta">'
                +         '<div><i class="fa-solid fa-calendar-days"></i>' + fechaPub + '</div>'
                +         '<div><i class="fa-solid fa-user"></i>' + escapeHtml(a.PublicadoPor || '') + '</div>'
                +         diasHtml
                +       '</div>'
                +     '</div>'
                +     '<div class="anuncio-footer">'
                +       '<button class="btn-eliminar-anuncio" data-id="' + a.Anu_Id + '">'
                +         '<i class="fa-solid fa-trash fa-xs"></i> Eliminar'
                +       '</button>'
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

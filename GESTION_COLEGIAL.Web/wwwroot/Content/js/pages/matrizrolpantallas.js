var MatrizRolPantallas = (function () {
    var obj = {};
    var _datos = [];      // array plano original de la API
    var _roles = [];      // roles únicos ordenados
    var _grupos = {};     // { grupo: [ { Pan_Descripcion, accesos: { Rol: 0|1 } } ] }

    obj.init = function (url) {
        $("#spinner-matriz").show();

        $.ajax({
            url: url,
            type: "GET",
            dataType: "json",
            success: function (response) {
                // El controlador envuelve en { data: [...] }
                var lista = (response && response.data) ? response.data : response;
                _datos = lista || [];
                _procesar(_datos);
                _renderizar();
                $("#spinner-matriz").hide();
            },
            error: function () {
                $("#spinner-matriz").hide();
                $("#tbody-matriz").html('<tr><td class="text-danger text-center">Error al cargar los datos.</td></tr>');
            }
        });

        // Filtro por nombre de pantalla
        $("#filter-pantalla").on("keyup", function () {
            _renderizar($(this).val().toLowerCase().trim());
        });
    };

    // Transforma array plano en estructura { roles, grupos }
    function _procesar(lista) {
        var rolesSet = {};
        var gruposMap = {};

        lista.forEach(function (item) {
            var rol = item.Rol_Descripcion;
            var pan = item.Pan_Descripcion;
            var grp = item.Pan_Grupo || "General";
            var tiene = item.TieneAcceso === 1 || item.TieneAcceso === true;

            // Roles únicos
            rolesSet[rol] = true;

            // Agrupar pantallas
            if (!gruposMap[grp]) gruposMap[grp] = {};
            if (!gruposMap[grp][pan]) gruposMap[grp][pan] = {};
            gruposMap[grp][pan][rol] = tiene;
        });

        _roles = Object.keys(rolesSet).sort();
        _grupos = gruposMap;
    }

    // Construye y vuelca el HTML de la tabla
    function _renderizar(filtro) {
        filtro = filtro || "";
        var $thead = $("#thead-matriz");
        var $tbody = $("#tbody-matriz");

        // Cabecera: columna Pantalla + una columna por rol
        var thRoles = _roles.map(function (r) {
            return '<th class="col-rol">' + _esc(r) + '</th>';
        }).join("");
        $thead.html('<tr><th style="min-width:200px;">Pantalla</th>' + thRoles + '</tr>');

        // Cuerpo
        var filas = "";
        var hayDatos = false;

        var gruposOrdenados = Object.keys(_grupos).sort();
        gruposOrdenados.forEach(function (grp) {
            var pantallas = Object.keys(_grupos[grp]).sort();

            // Filtrar pantallas por texto
            var pantallasFiltradas = filtro
                ? pantallas.filter(function (p) { return p.toLowerCase().indexOf(filtro) >= 0; })
                : pantallas;

            if (pantallasFiltradas.length === 0) return;

            hayDatos = true;

            // Fila de grupo (cabecera de sección)
            var colspanTotal = _roles.length + 1;
            filas += '<tr class="grupo-header"><td colspan="' + colspanTotal + '">' +
                '<i class="fa-solid fa-folder-open me-1"></i> ' + _esc(grp) + '</td></tr>';

            pantallasFiltradas.forEach(function (pan) {
                var celdas = _roles.map(function (rol) {
                    var tiene = _grupos[grp][pan][rol];
                    if (tiene) {
                        return '<td class="col-check text-success"><i class="fa-solid fa-check"></i></td>';
                    } else {
                        return '<td class="col-check text-muted">&ndash;</td>';
                    }
                }).join("");
                filas += '<tr><td>' + _esc(pan) + '</td>' + celdas + '</tr>';
            });
        });

        $tbody.html(filas);

        if (!hayDatos) {
            $("#msg-vacio").show();
        } else {
            $("#msg-vacio").hide();
        }
    }

    // Escapa HTML básico
    function _esc(str) {
        if (!str) return "";
        return String(str)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    return obj;
}());

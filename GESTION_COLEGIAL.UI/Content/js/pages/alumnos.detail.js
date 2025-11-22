var AlumnosDetail = (function () {
    var obj = {};

    obj.init = function (config) {
        $(function () {
            // Inicializacion de la pagina de detalle
            initializeTooltips();
        });
    };

    function initializeTooltips() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    return obj;
}());

var EncargadosDetail = (function () {
    var obj = {};

    obj.init = function (config) {
        $(function () {
            initializeTooltips();
        });
    };

    function initializeTooltips() {
        $('[data-toggle="tooltip"]').tooltip();
    }

    return obj;
}());

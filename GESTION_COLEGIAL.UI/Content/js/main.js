var appConfig = (function () {
    var obj = {};
    obj.alert = function (type, message) {
        $(function () {
            toastr.options = {
                "closeButton": false,
                "debug": false,
                "newestOnTop": false,
                "progressBar": false,
                "positionClass": "toast-bottom-right",
                "preventDuplicates": true,
                "onclick": null,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }

            toastr.remove();

            switch (type) {
                default:
                case "info":
                case 0:
                    toastr.info('<i class="mdi mdi-message-text-clock-outline"></i><span>' + message + '</span>')
                    break;
                case "success":
                case 1:
                    toastr.success('<i class="mdi mdi-check-underline"></i><span>' + message + '</span>')
                    break;
                case "error":
                case 2:
                    toastr.error('<i class="mdi mdi-upload-off-outline"></i><span>' + message + '</span>')
                    break;
                case "warning":
                case 3:
                    toastr.warning('<i class="mdi mdi-exclamation-thick"></i><span>' + message + '</span>')
                    break;
            }
        })
    }

    function setutPlugins() {
        $.extend(true, $.fn.dataTable.defaults, {
            dom:
                "<'row'<'col-md-8 text-left'B><'col-md-4'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            order: [],
            scrollCollapse: true,
            paging: true,
            stateSave: true,
            processing: true,
            lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, "Todos"]],
            pageLenght: 25,
            displayLength: 25,
            language: {
                processing: "Procesando...",
                lengthMenu: "Mostrar _MENU_ registros",
                zeroRecords: "No se encontraron resultados",
                emptyTable: "Ningún dato disponible en esta tabla",
                info: "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                infoEmpty: "Mostrando registros del 0 al 0 de un total de 0 registros",
                infoFiltered: "(filtrado de un total de _MAX_ registros)",
                infoPostFix: "",
                search: "",
                url: "",
                infoThousands: ",",
                loadingRecords: " ",
                searchPlaceholder: "Buscar en la tabla...",
                paginate: {
                    first: "Primero",
                    last: "Último",
                    next: "Siguiente",
                    previous: "Anterior"
                },
                aria: {
                    sortAscending: ": Activar para ordenar la columna de manera ascendente",
                    sortDescending: ": Activar para ordenar la columna de manera descendente"
                }
            }
        });
    };
    obj.init = function () {
        setutPlugins();
    }

    return obj;
}());

$(function () {
    appConfig.init();
});
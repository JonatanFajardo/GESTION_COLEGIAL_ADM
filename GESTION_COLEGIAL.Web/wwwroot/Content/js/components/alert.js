var alertConfig = (function () {
    var obj = {};
    obj.alert = function (message, type) {
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
                case "Info":
                case 0:
                    toastr.info('<i class="bi bi-info-circle mr-1"> </i><span>' + message + '</span>')
                    break;
                case "Warning":
                case 1:
                    toastr.warning('<i class="bi bi-exclamation-triangle mr-1"> </i><span>' + message + '</span>')
                    break;
                case "Success":
                case 2:
                    toastr.success('<i class="bi bi-check-circle mr-1"> </i><span>' + message + '</span>')
                    break;
                case "Error":
                case 3:
                    toastr.error('<i class="bi bi-x-circle mr-1"> </i><span>' + message + '</span>')
                    break;
            }
        });
    }

    return obj;
}());
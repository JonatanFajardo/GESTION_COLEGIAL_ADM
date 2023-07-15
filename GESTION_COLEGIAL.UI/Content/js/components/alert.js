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
                    toastr.info('<i class="mdi mdi-information-outline"> </i><span>' + message + '</span>')
                    break;
                case "Warning":
                case 1:
                    toastr.warning('<i class="mdi mdi-alert-outline"> </i><span>' + message + '</span>')
                    break;
                case "Success":
                case 2:
                    toastr.success('<i class="mdi mdi-checkbox-marked-circle-outline"> </i><span>' + message + '</span>')
                    break;
                case "Error":
                case 3:
                    toastr.error('<i class="mdi mdi-close-circle-outline"> </i><span>' + message + '</span>')
                    break;
            }
        });
    }

    return obj;
}());
var estado = $("#Mat_EsActivo");



estado.click(function () {
    if (estado.checked) {
        estado.checked = false;
        estado.val(false);
    } else if (estado.checked = 'undefined') {
        //es undefined cuando la pagina esta recien cargada y el ckekbox es true
        estado.checked = true;
        estado.val(true);
        estado.prop("checked");
    }
    else {
        estado.val(true);
        estado.prop("checked");
    }
});
var estado = $("#EsActivoMateria");



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

var estadoNivel = $("#EsActivoNivel");



estadoNivel.click(function () {
    if (estadoNivel.checked) {
        estadoNivel.checked = false;
        estadoNivel.val(false);
    } else if (estadoNivel.checked = 'undefined') {
        //es undefined cuando la pagina esta recien cargada y el ckekbox es true
        estadoNivel.checked = true;
        estadoNivel.val(true);
        estadoNivel.prop("checked");
    }
    else {
        estadoNivel.val(true);
        estadoNivel.prop("checked");
    }
});



var estadoSemestre = $("#EsActivoSemestre");



estadoSemestre.click(function () {
    if (estadoSemestre.checked) {
        estadoSemestre.checked = false;
        estadoSemestre.val(false);
    } else if (estadoSemestre.checked = 'undefined') {
        //es undefined cuando la pagina esta recien cargada y el ckekbox es true
        estadoSemestre.checked = true;
        estadoSemestre.val(true);
        estadoSemestre.prop("checked");
    }
    else {
        estadoSemestre.val(true);
        estadoSemestre.prop("checked");
    }
});

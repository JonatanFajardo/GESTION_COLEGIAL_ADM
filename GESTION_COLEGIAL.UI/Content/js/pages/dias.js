﻿var Dias = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Dia_Id",
                "Dia_Descripcion"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;
}());
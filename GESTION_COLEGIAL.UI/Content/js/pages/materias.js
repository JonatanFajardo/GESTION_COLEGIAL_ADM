﻿var Materias = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Mat_Id",
                "Mat_Nombre"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;
}());
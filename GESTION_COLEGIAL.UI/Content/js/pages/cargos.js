﻿var Cargos = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Car_Id",
                "Car_Descripcion"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;
}());
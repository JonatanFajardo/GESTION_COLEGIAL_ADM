﻿var NivelesEducativos = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                "Niv_Id",
                "Niv_Descripcion"
            ];
            datatable.init(Direction.listUrl, header);
        })
    }
    return obj;
}());
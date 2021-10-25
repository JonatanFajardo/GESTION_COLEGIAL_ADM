var Alumnos = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Alu_Id",
                    Size: 200
                },
                {
                    FieldName: "Per_Identidad"
                },
                {
                    FieldName: "Alu_Nombre"
                },
                {
                    FieldName: "Cno_Descripcion"
                },
                {
                    FieldName: "Est_Descripcion"
                },
                {
                    FieldName: "EsActivo"
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());
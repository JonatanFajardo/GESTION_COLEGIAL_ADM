var Cursos = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Cur_Id",
                    Size: 200
                },
                {
                    FieldName: "Cno_Descripcion"
                },
                {
                    FieldName: "Aul_Descripcion"
                },
                {
                    FieldName: "Niv_Descripcion"
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
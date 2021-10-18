var Encargados = (function () {

    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Enc_Id",
                    Size: 200
                },
                {
                    FieldName: "Per_Identidad"
                },
                {
                    FieldName: "Enc_Nombre"
                },
                {
                    FieldName: "Per_Telefono"
                },
                {
                    FieldName: "Enc_Ocupacion"
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
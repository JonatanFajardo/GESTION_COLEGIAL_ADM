var Eventos = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "even_Id",
                    Size: 200
                },
                {
                    FieldName: "even_Nombre"
                },
                {
                    FieldName: "even_Informacion"
                },
                {
                    FieldName: "even_Fecha"
                },
                {
                    FieldName: "even_Hora"
                },
                {
                    FieldName: "even_Concluido"
                }
            ]
            console.log(Direction)
            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
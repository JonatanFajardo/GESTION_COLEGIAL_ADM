var Horas = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "Hor_Id",
                    Size: 200
                },
                {
                    FieldName: "Hor_Hora"
                }
            ]
            console.log('hola' + Direction)

            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());
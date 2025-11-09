var Horas = (function () {
    var obj = {};

    obj.datatableCatalogs = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "HorarioId",
                    Size: 200
                },
                {
                    FieldName: "Hora"
                }
            ]
            console.log('hola' + Direction)

            datatableCatalogs.init(Direction, header);
        })
    }
    return obj;
}());

var Cursos = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        $(function () {
            var header = new Array();
            //Nombre | Tamaño/AutoWidth | Visibilidad
            header = [
                {
                    FieldName: "CursoId",
                    Size: 200
                },
                {
                    FieldName: "NombreCurso"
                },
                {
                    FieldName: "DescripcionNivel"
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

$(document).ready(function () {
    $('#NivelId').change(function () {
        var NivelId = $(this).val();
        debugger
        $.ajax({
            type: "post",
            url: "/Cas/GetStatelist?Cid=" + countryId,
            contentType: "html",
            success: function (response) {
                debugger
                $("#Cno_Id").empty();
                $("#Cno_Id").append(response);
            }
        })
    })
})

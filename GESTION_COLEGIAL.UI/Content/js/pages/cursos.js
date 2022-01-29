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
            console.log("di:"+Direction);
        })
    }
    return obj;
}());




document.ready(function () {
    $('#Niv_Id').change(function () {
        var Niv_Id = $(this).val();
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
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
                }
            ]
            datatable.init(Direction, header);
        })
    }
    return obj;
}());

$("#Niv_Id").on("change", function (e) {
    var valueSelected = $(this).val();

    //var json = {
    //    data:
    //        [
    //            {
    //                Value: 1,
    //                Text: "jona"
    //            },
    //            {
    //                Value: 1,
    //                Text: "jona"
    //            }
    //        ]
    //}
    //console.log(json.data)
    //FillDropDown(valueSelected, json.data);
    var data = {
        id: valueSelected
    }
    $.ajax({
        type: "GET",
        url: 'GetCursosNiveles/',
        data:data,
        dataType: "json",
        success: function (response) {
                
            if (response != null) {
                FillDropDown(valueSelected, response);
            }
            else {
            }
        }, 
        error: function (request, status, error) {
            console.log('Error: ' + request.responseText);
        }
    });
   


    
});
$(function () {
});

/**
 * Permite ingresar data a un dropdawn.
 * @param {any} dropDownId
 * @param {any} list El primer valor debe de ser el valor y el segundo el texto a mostrar.
 */
function FillDropDown(dropDownId, list) {
    $(dropDownId).empty();
    $.each(list, function (index, row) {
        if (index == 0) {
            console.log(row)
            console.log(row['data'][0]//revisar
            $(dropDownId).append("<option value='" + row.Value + "' selected='selected'>" + row.Text + "</option>");
        } else {
            $(dropDownId).append("<option value='" + row.Value + "'>" + row.Text + "</option>")
        }
    });
}
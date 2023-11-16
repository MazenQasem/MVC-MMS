$(document).ready(function () {
    $('#txtpin').keypress(function (event) {
        var len = 0;
        len = $("#txtpin").val().length;
        if (event.which == 13) {
            if (len >= 3) {
                $("#txtpin").attr("readonly", "readonly");
                return false;
            }
        }
    });

   

});
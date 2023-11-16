 

var SelectedDrugtbl;
$(document).ready(function () {
    $('.ViewTxtCSS').attr("readonly", "readonly");
    $("#gIACode").val(gIACode());
    var EDate = new Date();
    $('#DateTime').val(MyDate(EDate, "yy-mm-dd"));
    $('#OperatorName').val(OperatorName);

    clearcontrolls('');
    $('#PinNo').focus();
});
function clearcontrolls(type) {
    if (type == '') {
        SelectedDrugtbl = null;
        $('#SelectedDrugtbl').dataTable().fnClearTable();
        $('#SelectedDrugtbl').dataTable().fnDestroy();
        $('#txtMsg').MazClear();
        $('#PIN').MazClear();
        $('#PTName').MazClear();
        $('#Age').MazClear();
        $('#Sex').MazClear();
        $('#PinNo').MazClear();
    } else if (type == 'new') {
        $('#SelectedDrugtbl').dataTable().fnClearTable();
        $('#SelectedDrugtbl').dataTable().fnDestroy();
        $('#txtMsg').MazClear();
        $('#PIN').MazClear();
        $('#PTName').MazClear();
        $('#Age').MazClear();
        $('#Sex').MazClear();
        $('#PinNo').MazClear();
        $('#PinNo').MazEnabled();
        $('#PinNo').focus();
    }
    return false;
}
$(document).on('keydown', '#PinNo', function (e) {
    if (event.which == 13) {          
        if (parseInt($("#PinNo").val().length) > 1) {
            GetData($(this).val());
            $("#PinNo").MazDisabled();
        }
    }
          
});
function GetData(PIN) {
    var URL = "/PharmacyMSG/View";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify({ PIN: PIN }),
        success: function (msg) {
            if (msg.ErrMsg != "") {
            } else {
                $('#PIN').val(msg.PinNo);
                $('#PTName').val(msg.PTName);
                $('#Age').val(msg.Age);
                $('#Sex').val(msg.Sex);
                InsertItemProc(msg.PHmsgList);
            }
        }, Failure: function (msg)
        { ShowMessage("Check Your Entry"); }
    });
    return false;
}
$(document).on('click', '#btnClear', function () {
    clearcontrolls('new');
    return false;
});
function InsertItemProc(itm) {
    var ItemsList = [];
    ItemsList = itm;
    render_InsertItemProc('#SelectedDrugtbl', ItemsList);
    SelectedDrugtbl = null;      $('#btnSave').focus();      return false;
}
function render_InsertItemProc(tblid, xItem) {
    var SelectProcTable = $(tblid).dataTable({
        destroy: true,
        data: xItem,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
            { data: 'ID', targets: [0], className: 'hidden', width: '0%' },
            { data: 'msg', targets: [1], className: '', width: '55%' },
            { data: 'Station', targets: [2], className: '', width: '20%' },
            { data: 'Operator', targets: [3], className: '', width: '20%' },
            { data: 'deleted', targets: [4], className: 'hidden', },
             {
                 data: 'Action', targets: [5], className: 'TAC', width: '5%',
                 "mRender": function (data, type, full) {

                     return '<a href="#"><span class="icon-trash cancel-user2 edit-mode"></span></a>'
                     + '<a href="#"><span class="icon-ok save-user edit-mode hidden"></span></a> ';
                 }
             },
        ]
    }).fnDraw();
    SelectProcTable = $(tblid).DataTable();
    $('.disable').prop('disabled', true);

};
function ProcedureSelect() {
    var ProcID = $('#txtMsg').val();
    var rows = $('#SelectedDrugtbl').dataTable().fnGetNodes();
    var ItemsList = [];
    for (var i = 0; i < rows.length; i++) {
        var InsertItems = {};
        InsertItems.ID = $(rows[i]).find("td:eq(0)").html();
        InsertItems.msg = $(rows[i]).find("td:eq(1)").html();
        InsertItems.Station = $(rows[i]).find("td:eq(2)").html();
        InsertItems.Operator = $(rows[i]).find("td:eq(3)").html();
        InsertItems.deleted = $(rows[i]).find("td:eq(4)").html();
        ItemsList.push(InsertItems);
    }
    var InsertItems = {};

    InsertItems.ID = 0;
    InsertItems.msg = ProcID;
    InsertItems.Station = gStationName();
    InsertItems.Operator = OperatorName();
    InsertItems.deleted = 'False';
    ItemsList.push(InsertItems);
    InsertItemProc(ItemsList);
    $('#txtMsg').MazClear();
    $('#txtMsg').focus();
    return false;
}
$(document).on('click', '#btnAddItem', function (e) {
    if (parseInt($('#txtMsg').val().length) > 1) {
        ProcedureSelect();
    }
    return false;
});
$(document).on('click', '.cancel-user2', function (e) {
    var tr = $(this).closest("tr");
    tr.find('.cancel-user2').addClass('hidden');
    tr.find('.save-user').removeClass('hidden');
    tr.find("td:eq(4)").html('True');
    tr.children('td,th').css('background-color', 'lightslategrey');     e.preventDefault();
    return false;
});
$(document).on('click', '.save-user', function (e) {
    var tr = $(this).closest("tr");
    tr.find('.save-user').addClass('hidden');
    tr.find('.cancel-user2').removeClass('hidden');
    tr.find("td:eq(4)").html('False');
    tr.children('td,th').css('background-color', 'white');     e.preventDefault();
    return false;
});
$(document).on('click', '#btnSave', function () {
    var rows = $('#SelectedDrugtbl').dataTable().fnGetNodes();
    var DrugAllergyMdl = {};
    var ItemsList = [];
    if (rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            var InsertItems = {};
            if (parseInt($(rows[i]).find("td:eq(1)").html().length) > 0) {
                InsertItems.ID = $(rows[i]).find("td:eq(0)").html();
                InsertItems.msg = $(rows[i]).find("td:eq(1)").html();
                InsertItems.Station = $(rows[i]).find("td:eq(2)").html();
                InsertItems.Operator = $(rows[i]).find("td:eq(3)").html();
                InsertItems.deleted = $(rows[i]).find("td:eq(4)").html();
                ItemsList.push(InsertItems);
            } else {
                ShowMessage("No Message to Save");
                return false;
            }

        }
        DrugAllergyMdl.PHmsgList = ItemsList;
        DrugAllergyMdl.PIN = $('#PIN').val();
    } else {

        ShowMessage("No Message to Save");
        return false;
    }


    if (parseInt($('#PIN').val().length) > 1) {
        var URL = "/PharmacyMSG/Save";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ DrugAllergyT: DrugAllergyMdl }),
            success: function (msg) {
                if (msg.isSuccess == true) {
                    ShowMessage(msg.Message);
                    clearcontrolls('new');
                    return false;

                } else {
                    ShowMessage(msg.Message);
                    return false;
                }
            }
        });


    } else { MazAlert("Please Enter Pin or this Pin not Correct"); return false; }

    return false;
});


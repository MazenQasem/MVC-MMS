 

var SelectedDrugtbl;
$(document).ready(function () {
    $('.ViewTxtCSS').attr("readonly", "readonly");
    $("#gIACode").val(gIACode());
    var EDate = new Date();
    $('#DateTime').val(MyDate(EDate, "yy-mm-dd"));
    $('#OperatorName').val(OperatorName);

    clearcontrolls('');
    $('#PinNo').focus();
    try {
        var URL = GetAppName() + "/MazenMain/LoadListByItem";
        $('#allItemsList').select2({
            ajax: {
                url: URL,
                dataType: 'json',
                type: "POST",
                delay: 250,
                data: function (params) {
                    return {
                        query: LoadItems(gStationID(), params),
                        page: 100
                    };
                },
                results: function (data) {
                    return {
                        results: $.map(data, function (item) {
                            return {
                                text: item.Name,
                                                                 id: item.ID
                            }
                        })
                    };
                },
                cache: true
            },
            minimumInputLength: 2,
            formatResult: FormatResult,
            formatSelection: FormatSelection,
        });
    } catch (err) { ShowMessage('Cant found Items'); }


    $("#ReportDialog").dialog({
        autoOpen: false,
        modal: true,
        width: 1093,
        position: { my: "center", at: "top+100", of: window },
        resizable: false,
        title: 'Print out',
                 open: function () {
                                                   fix();
        },
        buttons: {
            "Print": function () {
                printContent("DivRpt");

            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });



});

function clearcontrolls(type) {
    if (type == '') {
        SelectedDrugtbl = null;
        $('#SelectedDrugtbl').dataTable().fnClearTable();
        $('#SelectedDrugtbl').dataTable().fnDestroy();
        $('#allItemsList').MazClearList();
        $('.OtherAllergyDIV').hide();
        $('.DrugAllergyDIV').hide();
        $('#PIN').MazClear();
        $('#PTName').MazClear();
        $('#Age').MazClear();
        $('#Sex').MazClear();
        $('#PinNo').MazClear();
        $('#othertxt').MazClear();
        $("#AllergyTypeList").select2("val", "0");
        $("#AllergyTypeList").MazSelectTwoEnabled('false');
    } else if (type == 'type') {
        SelectedDrugtbl = null;
                                            $('.OtherAllergyDIV').hide();
        $('.DrugAllergyDIV').hide();
    } else if (type == 'new') {
        $('#SelectedDrugtbl').dataTable().fnClearTable();
        $('#SelectedDrugtbl').dataTable().fnDestroy();
        $('#allItemsList').MazClearList();
        $('.OtherAllergyDIV').hide();
        $('.DrugAllergyDIV').hide();
        $('#PIN').MazClear();
        $('#PTName').MazClear();
        $('#Age').MazClear();
        $('#Sex').MazClear();
        $('#PinNo').MazClear();
        $('#PinNo').MazEnabled();
        $("#AllergyTypeList").select2("val", "0");
        $("#AllergyTypeList").MazSelectTwoEnabled('false');
        $('#othertxt').MazClear();
        $('#PinNo').focus();

    }
    return false;
}

function LoadItems(st, txt) {
    var StrSql;
    StrSql = "select ID,Name from M_Generic " + txt + " Order by Name ";
    return StrSql;
}

$(document).on('change', '#AllergyTypeList', function () {
    clearcontrolls('type');
    if (parseInt($('#AllergyTypeList').val()) == 1) {
        $('.OtherAllergyDIV').hide();
        $('.DrugAllergyDIV').show();
    } else if (parseInt($('#AllergyTypeList').val()) == 2) {

        $('.OtherAllergyDIV').show();
        $('.DrugAllergyDIV').hide();

    } else {
        $('.OtherAllergyDIV').hide();
        $('.DrugAllergyDIV').hide();
    }
    return false;
});
$(document).on('keydown', '#PinNo', function (e) {
    if (event.which == 13) {  
        
        if (parseInt($("#PinNo").val().length) > 1) {
            GetData($(this).val());
            $('#AllergyTypeList').MazSelectTwoEnabled('true');
            $('#AllergyTypeList').select2("val", "1");
            $('.OtherAllergyDIV').hide();
            $('.DrugAllergyDIV').show();
            $("#PinNo").MazDisabled();
        }
    }
          
});
function GetData(PIN) {
    var URL = "/DrugAllergy/View";
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
                $('#othertxt').val(msg.othertxt);
                InsertItemProc(msg.DrugList);
            }
        }
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
            { data: 'ID', targets: [0], className: 'hidden' },
            { data: 'Name', targets: [1], className: '', },
             {
                 data: 'Action', targets: [2], className: 'TAC', width: '5%',
                 "mRender": function (data, type, full) {
                     return '<a href="#"><span class="icon-trash cancel-user2 edit-mode"></span></a>';
                 }
             },
        ]
    }).fnDraw();
    SelectProcTable = $(tblid).DataTable();
    $('.disable').prop('disabled', true);

};
function ProcedureSelect(ProcIDD) {
    var ProcID = $('#allItemsList').select2('data');
    var rows = $('#SelectedDrugtbl').dataTable().fnGetNodes();
    var ItemsList = [];
    for (var i = 0; i < rows.length; i++) {
        var InsertItems = {};
        InsertItems.ID = $(rows[i]).find("td:eq(0)").html();
        InsertItems.Name = $(rows[i]).find("td:eq(1)").html();
        if (parseInt(InsertItems.ID) == parseInt(ProcIDD)) {
            ShowMessage('Item Already Selected !');
            return false;
        }
        else {
            ItemsList.push(InsertItems);
        }
    }
    var InsertItems = {};
    InsertItems.ID = ProcIDD;
    InsertItems.Name = ProcID.text;
    ItemsList.push(InsertItems);
    InsertItemProc(ItemsList);
    $('#allItemsList').select2("open");
    return false;
}
$(document).on('change', '#allItemsList', function () {
    var itemindex = $('#allItemsList').select2('val');
    if (itemindex > 0) {
        ProcedureSelect(itemindex);
    } else {
        ShowMessage('Please Select an Item First!');
    }
    return false;
});
$(document).on('click', '#btnAddItem', function (e) {
    var itemindex = $('#allItemsList').select2('val');
    if (itemindex > 0) {
        ProcedureSelect(itemindex);
    } else {
        ShowMessage('Please Select an Item First!');
    }
    return false;
});
$(document).on('click', '.cancel-user2', function (e) {
    var po = $(this).closest("tr").get(0);
    var iPos = $("#SelectedDrugtbl").dataTable().fnGetPosition(po);
    if (iPos !== null) {
        $("#SelectedDrugtbl").dataTable().fnDeleteRow(iPos);     }
    e.preventDefault();
    return false;
});

$(document).on('click', '#btnSave', function () {
    var rows = $('#SelectedDrugtbl').dataTable().fnGetNodes();
    var DrugAllergyMdl = {};
    var ItemsList = [];
    for (var i = 0; i < rows.length; i++) {
        var InsertItems = {};
        InsertItems.ID = $(rows[i]).find("td:eq(0)").html();
        InsertItems.Name = $(rows[i]).find("td:eq(1)").html();
        ItemsList.push(InsertItems);
    }
    DrugAllergyMdl.DrugList = ItemsList;
    DrugAllergyMdl.PIN = $('#PIN').val();
    DrugAllergyMdl.othertxt = $('#othertxt').val();

    console.log(parseInt($('#PIN').val().length));

    if (parseInt($('#PIN').val().length) > 1) {
        var URL = "/DrugAllergy/Save";
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

$(document).on('click', '#btnPrint', function () {
    PrintOrder(parseInt($('#PIN').val()), 'ReportDialog', "/DrugAllergy/Print", "DivRpt");
    return false;
});

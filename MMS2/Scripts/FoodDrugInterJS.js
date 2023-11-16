 var ViewtempTbl = null;
$(document).ready(function () {
    $('.ViewWidget').show();
    $('.DetailWidget').hide();
    $(".divGeneric").show();
    $(".divNewGeneric").hide();
    $("#flgNewGeneric").val(false);
    $("#flgNewGenericF").val(false);
    $('.ViewTxtCSS').attr("readonly", "readonly");
    try {
        var URL = GetAppName() + "/MazenMain/LoadListByItem";
        $('#GenericList').select2({
            ajax: {
                url: URL,
                dataType: 'json',
                type: "POST",
                delay: 250,
                data: function (params) {
                    return {
                        query: LoadItems(params),
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

    try {
        var URL = GetAppName() + "/MazenMain/LoadListByItem";
        $('#DrugList').select2({
            ajax: {
                url: URL,
                dataType: 'json',
                type: "POST",
                delay: 250,
                data: function (params) {
                    return {
                        query: LoadFoods(params),
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

    try {
        var URL = GetAppName() + "/MazenMain/LoadListByItem";
        $('#GenericListF').select2({
            ajax: {
                url: URL,
                dataType: 'json',
                type: "POST",
                delay: 250,
                data: function (params) {
                    return {
                        query: LoadItemsF(params),
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
});
function LoadItems(txt) {
    var StrSql;
    StrSql = "select ID,Name from M_Generic where deleted=0 "
        + " and name like '%" + txt + "%' "
        + "  order by Name ";
    return StrSql;
}

function LoadFoods(txt) {
    var StrSql;
    StrSql = "select id as ID,name as Name from FOODINTERACTION_VW where deleted=0  "
        + " and name like '%" + txt + "%' "
        + "  order by Name ";
    return StrSql;
}

function LoadItemsF(txt) {
    var StrSql;
    StrSql = "Select distinct ID,Generic as Name from DFDetail where "
        + " Generic like '%" + txt + "%' "
        + "  order by Generic ";
    return StrSql;
}
$(document).on('change', '#GenericList', function () {
    ClearControls('');
    var Obj = $('#GenericList').select2('data');
    var GID = parseInt(Obj.id);
    var Gname = Obj.text;
    if (GID > 0) {
        $("#GenericList").MazSelectTwoEnabled('false');
        GetCurrentGenericDetail(GID, Gname);

        return false;
    }
    return false;
});
function GetCurrentGenericDetail(GID, Gname) {
    var URL = "/FoodDrugInter/GetGenDtl";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify({ GID: GID, Gname: Gname }),
        success: function (msg) {
            render_IndentView("#tblGenDrug", msg);
            $("#DrugList").select2("open");
            return false;
        }
    });
    return false;


}
$(document).on('change', '#DrugList', function () {
    if (parseInt($('#DrugList').val()) > 0) {
        var DrugID = parseInt($('#DrugList').val());


        var DrugData = $('#DrugList').select2('data');

        var rows = $('#tblGenDrug').dataTable().fnGetNodes();
        var Seq = 1;
        var ItemsList = [];
        for (var i = 0; i < rows.length; i++) {
            var InsertItems = {};
            InsertItems.Seq = Seq;
            InsertItems.GenericID = $(rows[i]).find("td:eq(1)").html();
            InsertItems.Generic = $(rows[i]).find("td:eq(2)").html();
            InsertItems.DrugID = $(rows[i]).find("td:eq(3)").html();
            InsertItems.Drug = $(rows[i]).find("td:eq(4)").html();
            InsertItems.Reaction = $(rows[i]).find(".Reactiontxt").val();
            if (parseInt(InsertItems.DrugID) == parseInt(DrugID)) {
                ShowMessage(' Already Selected !');
                return false;
            }
            else {
                ItemsList.push(InsertItems);
                Seq += 1;
            }
        }
        var GenericData = $('#GenericList').select2('data');
        var InsertItems = {};
        InsertItems.Seq = Seq;
        console.log($('#flgNewGeneric').val());
        if ($('#flgNewGeneric').val() == 'false') {
            InsertItems.GenericID = GenericData.id;
            InsertItems.Generic = GenericData.text;
        } else {
            InsertItems.GenericID = 0;             InsertItems.Generic = $('#NewGeneric').val();
        }
        InsertItems.DrugID = DrugData.id;
        InsertItems.Drug = DrugData.text;
        InsertItems.Reaction = "";
        ItemsList.push(InsertItems);
        render_IndentView("#tblGenDrug", ItemsList);
        $('.Reactiontxt').focus;
        return false;
    }
    return false;
});
function render_IndentView(tableID, dataxc) {
    ViewtempTbl = $(tableID).dataTable({
        destroy: true,
        data: dataxc,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        processing: false,
        scrollCollapse: false,
        scrollY: '50vh',           scrollCollapse: true,         columnDefs: [
            { data: 'Seq', targets: [0], className: 'Align_Left', width: '5%' },
            { data: 'GenericID', targets: [1], className: 'hidden', width: '20%' },
            { data: 'Generic', targets: [2], width: '15%' },
            { data: 'DrugID', targets: [3], className: 'hidden', width: '20%' },
            { data: 'Drug', targets: [4], width: '15%' },
            {
                data: 'Reaction', targets: [5], className: '', width: '60%',
                'mRender': function (data, type, full) {
                    return '<span class="display-mode">' +
                    '<input type="text" id="Reactiontxt" value="' + data + '" class="Reactiontxt" />';
                }
            },
             {
                 data: 'Action', targets: [6], className: 'TAC', width: '5%',
                 "mRender": function (data, type, full) {
                     return '<a href="#"><span class="icon-trash cancel-user edit-mode"></span></a>';
                 }
             },
        ],
        rowCallback: function (row, data, index) {           },
        drawCallback: function (oSettings) {  
        }
    }).fnDraw();


    ViewtempTbl = $(tableID).DataTable();
    $('.Reactiontxt').focus();
    return false;
};
function ClearControls(type) {
    if (type == 'Formulary') {
        $('#txtBrandName').val('');
        $('#txtCategory').val('');
        $('#txtUse').val('');
        $('#txtIndication').val('');
        $('#txtWarning').val('');
        $('#txtReactions').val('');
        $('#txtMechanism').val('');
        $('#txtDosage').val('');
        $('#txtDosageForms').val('');
        $('#txtRemarks').val('');

        $('#btnDeleteF').MazDisabled();
        $('.RepWidget').hide();
                                                                                          
        return false;
    }
    $('#tblGenDrug').dataTable().fnClearTable();
    $('#tblGenDrug').dataTable().fnDestroy();
    ViewtempTbl = null;
    $('.DetailWidget').hide();
    $('.ViewWidget').show();
    $('.divNewGeneric').hide();
    if (type == 'clear') {
        $(".divGeneric").show();
        $('.divNewGeneric').hide();
        $("#GenericList").MazSelectTwoEnabled('true');
        $("#DrugList").MazClearList();
        $("#NewGeneric").val('');
        $("#flgNewGeneric").val(false);
    }
    else if (type == 'new') {
        $("#DrugList").MazClearList();
        $(".divGeneric").hide();
        $(".divNewGeneric").show();
        $("#NewGeneric").val('');
        $("#NewGeneric").focus();
        $("#flgNewGeneric").val(true);
    }
    return false;
};
$(document).on('keydown', '.Reactiontxt', function (event) {
    if (event.which == 13 || event.which == 39) {          $('#DrugList').select2("open");
        return false;
    }
});
$(document).on('click', '.cancel-user', function (e) {
    var po = $(this).closest("tr").get(0);
    var iPos = $("#tblGenDrug").dataTable().fnGetPosition(po);
    if (iPos !== null) {
        $("#tblGenDrug").dataTable().fnDeleteRow(iPos);     }
              e.preventDefault();
    return false;

});
$(document).on('click', '#btnNew', function () {
    ClearControls('new');
    return false;
});
$(document).on('click', '#btnClear', function () {
    ClearControls('clear');
         return false;
});

$(document).on('click', '#btnSave', function () {
    if (Validate() == true) {

        var DrugTable = [];
        var rows = $('#tblGenDrug').dataTable().fnGetNodes();
        for (var i = 0; i < rows.length; i++) {
            var InsertItems = {};
            InsertItems.Seq = $(rows[i]).find("td:eq(0)").html();
            InsertItems.GenericID = $(rows[i]).find("td:eq(1)").html();
            InsertItems.Generic = $(rows[i]).find("td:eq(2)").html();
            InsertItems.DrugID = $(rows[i]).find("td:eq(3)").html();
            InsertItems.Drug = $(rows[i]).find("td:eq(4)").html();
            InsertItems.Reaction = $(rows[i]).find(".Reactiontxt").val();
            DrugTable.push(InsertItems);
        }

                 var URL = "/FoodDrugInter/Save";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ DrugTable: DrugTable }),
            success: function (msg) {
                if (msg.isSuccess == true) {
                    ShowMessage(msg.Message);
                    console.log(msg.Message);
                    ClearControls('clear');
                    return false;

                } else {
                    console.log('err:' + msg.Message);
                    ShowMessage(msg.Message);
                    return false;
                }

            }
        });



    }

    return false;
});
function Validate() {
    if ($('#flgNewGeneric').val() == 'false') {
        if ($('#GenericList').val() > 0) { } else { ShowMessage('Please select Generic'); return false; }
    } else {
        if ($('#NewGeneric').val().length > 0) {
            var GetName = 0;
            GetName = GetNameOrValVar("select count(*) as Cname from m_generic where rtrim(name)=rtrim('" + $('#NewGeneric').val() + "')", "Cname", GetName);
            console.log("name Count= " + GetName);
            if (parseInt(GetName) > 0) {
                ShowMessage('Please Generic with Same name are Exist, Enter New Generic Name'); return false;
            }
        } else { ShowMessage('Please Enter Generic Name'); return false; }
    }
    var rows = $('#tblGenDrug').dataTable().fnGetNodes();
    if (rows.length == 0) { ShowMessage("Please select Drug for Interacting"); return false; }
    for (var i = 0; i < rows.length; i++) {
        var Description = $(rows[i]).find(".Reactiontxt").val().length;
        if (Description > 0) { }
        else {
            ShowMessage("Please enter Description");
            $(this).focus;
            return false;
        }
    }
    return true;
}




$(document).on('click', '#btnDrugFormulary', function () {
    $('.DetailWidget').show();
    $('.ViewWidget').hide();
    $('.ReptDiv').hide();
    $('#GenericIDF').val(0);
    $('#flgNewGenericF').val(false);
    $('#NewGenericF').val('');
    $('.divFDrugNew').hide();
    $('.divFDrugselect').show();
    return false;
});
$(document).on('change', '#GenericListF', function () {
    var Obj = $('#GenericListF').select2('data');
    var GID = parseInt(Obj.id);
    var Gname = Obj.text;
    if (GID > 0) {
        $("#GenericListF").MazSelectTwoEnabled('false');
        ClearControls('Formulary');
        var URL = "/FoodDrugInter/GetGenDtlF";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ GID: GID }),
            success: function (msg) {
                $('#GenericIDF').val(msg.GenericID);
                $('#txtBrandName').val(msg.BrandName);
                $('#txtCategory').val(msg.Category);
                $('#txtUse').val(msg.Use);
                $('#txtIndication').val(msg.Indication);
                $('#txtWarning').val(msg.Warning);
                $('#txtReactions').val(msg.Reaction);
                $('#txtMechanism').val(msg.Mechanism);
                $('#txtDosage').val(msg.Dosage);
                $('#txtDosageForms').val(msg.DosageForms);
                $('#txtRemarks').val(msg.Remarks);
                $('#btnDeleteF').MazEnabled();
            }
        });
        return false;
    }
    return false;
});
$(document).on('click', '#btnNewF', function () {
    ClearControls('Formulary');
    $('#GenericIDF').val(0);
    $('#flgNewGenericF').val(true);
    $('#NewGenericF').val('');
    $('.divFDrugNew').show();
    $('.divFDrugselect').hide();
    return false;
});
$(document).on('click', '#btnClearF', function () {
    ClearControls('Formulary');
    $('#GenericIDF').val(0);
    $('#flgNewGenericF').val(false);
    $('#NewGenericF').val('');
    $('.divFDrugNew').hide();
    $('.divFDrugselect').show();
    $("#GenericListF").MazSelectTwoEnabled('true');
    $("#GenericListF").select2('open');
    return false;
});
$(document).on('click', '#btnDeleteF', function (e) {
    if (parseInt($('#GenericListF').val()) > 0) {
        bootbox.confirm({
            message: 'Are you sure you want to delete this Record?',
            title: 'Confirm',
            buttons: {
                confirm: {
                    label: "Delete",
                    className: 'btn-success'
                },
                cancel: {
                    label: "Cancel",
                    className: 'btn-danger'
                }
            },
            callback: function (result) {
                                 if (result == true) {
                    e.preventDefault();
                    DeleteRec(parseInt($('#GenericListF').val()));
                }
            }
        });
    } else {
        MazAlert("Please Select Item First!");
        return false;
    }
    return false;

});
function DeleteRec(FdrugID) {
    var Obj = $('#GenericListF').select2('data');
    var GID = parseInt(Obj.id);
    var DrugTable = {};
    DrugTable.GenericID = GID;
    DrugTable.BrandName = $('#txtBrandName').val();
    DrugTable.Category = $('#txtCategory').val();
    DrugTable.Use = $('#txtUse').val();
    DrugTable.Indication = $('#txtIndication').val();
    DrugTable.Warning = $('#txtWarning').val();
    DrugTable.Reaction = $('#txtReactions').val();
    DrugTable.Mechanism = $('#txtMechanism').val();
    DrugTable.Dosage = $('#txtDosage').val();
    DrugTable.DosageForms = $('#txtDosageForms').val();
    DrugTable.Remarks = $('#txtRemarks').val();

    if (GID > 0) {
        var URL = "/FoodDrugInter/DeleteF";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ DrugTable: DrugTable }),
            success: function (msg) {
                if (msg.isSuccess == true) {
                    ShowMessage(msg.Message);
                    console.log(msg.Message);
                    $('#btnClearF').trigger('click');
                    return false;

                } else {
                    console.log('err:' + msg.Message);
                    ShowMessage(msg.Message);
                    return false;
                }

            }
        });
        return false;
    }
    return false;


}
$(document).on('click', '#btnSaveF', function (e) {
    if (validateF() == true) {
        var DrugTable = {};
        var GID = 0;
        if ($("#flgNewGenericF").val() == 'false') {
            var Obj = $('#GenericListF').select2('data');
            GID = parseInt(Obj.id);
        }

        DrugTable.GenericID = GID;
        DrugTable.GenericName = $("#NewGenericFF").val();
        DrugTable.BrandName = $('#txtBrandName').val();
        DrugTable.Category = $('#txtCategory').val();
        DrugTable.Use = $('#txtUse').val();
        DrugTable.Indication = $('#txtIndication').val();
        DrugTable.Warning = $('#txtWarning').val();
        DrugTable.Reaction = $('#txtReactions').val();
        DrugTable.Mechanism = $('#txtMechanism').val();
        DrugTable.Dosage = $('#txtDosage').val();
        DrugTable.DosageForms = $('#txtDosageForms').val();
        DrugTable.Remarks = $('#txtRemarks').val();


        var URL = "/FoodDrugInter/SaveF";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ DrugTable: DrugTable }),
            success: function (msg) {
                if (msg.isSuccess == true) {
                    ShowMessage(msg.Message);
                    console.log(msg.Message);
                    $('#btnClearF').trigger('click');
                    return false;

                } else {
                    console.log('err:' + msg.Message);
                    ShowMessage(msg.Message);
                    return false;
                }

            }
        });
        return false;

    }
    return false;

});
function validateF() {
    if ($('#flgNewGenericF').val() == 'false') {
        if ($('#GenericListF').val() > 0) { } else { ShowMessage('Please select Generic'); return false; }
    } else {
        if ($('#NewGenericFF').val().length > 0) {
            var GetName = 0;
            GetName = GetNameOrValVar("select count(*) as Cname from DFDetail where rtrim(GENERIC)=rtrim('" + $('#NewGenericF').val() + "')", "Cname", GetName);
            if (parseInt(GetName) > 0) {
                ShowMessage('Please Generic with Same name are Exist, Enter New Generic Name'); return false;
            }
        } else { ShowMessage('Please Enter Generic Name'); return false; }
    }
    if ($('#txtBrandName').val().length > 0) { }
    else { ShowMessage('Please Enter Brand Name'); return false; }

    return true;
}

$(document).on('click', '#btnViewF', function () {
    $('.EditDiv').hide();
    $('.ReptDiv').show();

    $('#txtBrandName2').val($('#txtBrandName').val());
    $('#txtCategory2').val($('#txtCategory').val());
    $('#txtUse2').val($('#txtUse').val());
    $('#txtIndication2').val($('#txtIndication').val());
    $('#txtWarning2').val($('#txtWarning').val());
    $('#txtReactions2').val($('#txtReactions').val());
    $('#txtMechanism2').val($('#txtMechanism').val());
    $('#txtDosage2').val($('#txtDosage').val());
    $('#txtDosageForms2').val($('#txtDosageForms').val());
    $('#txtRemarks2').val($('#txtRemarks').val());

    return false;
});
$(document).on('click', '#btnEditF', function () {
    $('.EditDiv').show();
    $('.ReptDiv').hide();
    return false;
});


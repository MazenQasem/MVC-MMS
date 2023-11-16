 
var ViewtempTbl;
var SelectItemsTable;
var GetDialogAnswer;
$(document).ready(function () {
    var xDate = new Date().getDay() - 11;
    var EDate = new Date();

    $('#FromDate').datepicker("setDate", xDate).inputmask("yyyy-mm-dd");;
    $('#ToDate').datepicker("setDate", EDate).inputmask("yyyy-mm-dd");;
    $('.ViewWidget').show();
    $('.DetailWidget').hide();
    $('.ViewTxtCSS').attr("readonly", "readonly");
    $('#btnShowData').click();
    LoadCommonList();
    $('#issueflag').change(function () { });
    $('#IssueSlno').focusout(function () {
        if ($('#IssueSlno').val() > 0) {
            LoadRelatedIssIndent($('#IssueSlno').val());
        }
    });
     
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
                        query: LoadItems(parseInt($('select#lstItemCategory option:selected').val()),
                            parseInt($('#CurrentStation').val()),
                             parseInt($('select#lstQOH option:selected').val()), params, 0),
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
function LoadRelatedIssIndent(issSlno) {
    var URL = "/IndentReturn/GetIssueInfo";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify({ issSlno: issSlno }),
        success: function (msg) {
            if (msg.ErrMsg != null) {
                ShowMessage(msg.ErrMsg);
            }
            var refTemp = $('#txtRef').val();
            clearcontrolls('new');
            $('#ToStationList').val(msg.ToStationID);
            $('#IssueSlno').val(msg.IssSlNO);
            $('#lstItemCategory').val(msg.ItemCategory);
            $('#IssueID').val(msg.mIssueID);
            $('#txtRef').val(refTemp);
            LoadItems(msg.ItemCategory, msg.CurrentStation, 0, '', msg.IssSlNO);
            CheckBox('#issueflag', true);
        }
    });
    return false;
}

$(document).on('click', '#btnShowData', function () {
    GetData();
    return false;
});
$(document).on('click', '#btnNewOrder', function () {
    clearcontrolls('new');
    return false;
});
$(document).on('click', '#btnClose', function () {
    clearcontrolls('view');
    return false;
});
$(document).on('click', '#btnSave', function () {
    if (NullTOInt($('#OrderID').val()) > 0) {
        callDialog("Do you really wish to cancel this return?");
    }
    else {
        Save();
    }
    return false;
});
function Save() {

    if (Validate() == true) {
        var IndentOrderModel = {};
        var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
        var ReturnList = [];
        for (var i = 0; i < rows.length; i++) {
            var qt = 0;
            qt = $(rows[i]).find("#lblReqQty").val();

            if (qt > 0) {
                var IndentReturnedItemList = {};
                IndentReturnedItemList.SNO = $(rows[i]).find("td:eq(0)").html();
                IndentReturnedItemList.Name = $(rows[i]).find("td:eq(1)").html();
                IndentReturnedItemList.BatchNo = $(rows[i]).find("td:eq(2)").html();
                IndentReturnedItemList.BatchID = $(rows[i]).find("td:eq(3)").html();
                IndentReturnedItemList.ExpiryDate = $(rows[i]).find("#spandate").val();
                IndentReturnedItemList.Quantity = $(rows[i]).find("#lblReqQty").val();
                IndentReturnedItemList.QOH = $(rows[i]).find("td:eq(6)").html();
                IndentReturnedItemList.UnitID = $(rows[i]).find("td:eq(7)").html();
                IndentReturnedItemList.Remarks = $(rows[i]).find("#lblRemarks").val();
                IndentReturnedItemList.DrugType = $(rows[i]).find("td:eq(9)").html();
                IndentReturnedItemList.ID = $(rows[i]).find("td:eq(10)").html();

                IndentReturnedItemList.issQty = $(rows[i]).find("td:eq(12)").html();
                IndentReturnedItemList.retQty = $(rows[i]).find("td:eq(13)").val();
                IndentReturnedItemList.converQty = $(rows[i]).find("td:eq(14)").html();
                IndentReturnedItemList.unit = $(rows[i]).find("td:eq(15)").html();

                ReturnList.push(IndentReturnedItemList);
            }
        }
        IndentOrderModel.ReturnList = ReturnList;
        IndentOrderModel.ToStationID = $('select#ToStationList option:selected').val();
        IndentOrderModel.ItemCategory = $('select#lstItemCategory option:selected').val();
        IndentOrderModel.txtRef = $('#txtRef').val();
        IndentOrderModel.lbldate = $('#lbldate').val();
        IndentOrderModel.lblId = $('#lblId').val();
        IndentOrderModel.lbloperator = $('#lbloperator').val();
        IndentOrderModel.OrderID = $('#OrderID').val();
        IndentOrderModel.mIssueID = $('#IssueID').val();


        if (NullTOInt($('#OrderID').val()) > 0) {
            callDialog("Do you really wish to cancel this return?");
        }

        var URL = "/IndentReturn/Save";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ Order: IndentOrderModel }),
            success: function (msg) {
                if (msg.ErrMsg != null) {
                    ShowMessage(msg.ErrMsg);
                    clearcontrolls('view');
                }
            }
        });
        return false;
    }

}
function Cancel() {

    var IndentOrderModel = {};
    var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
    var ReturnList = [];
    for (var i = 0; i < rows.length; i++) {
        var IndentReturnedItemList = {};
        IndentReturnedItemList.SNO = $(rows[i]).find("td:eq(0)").html();
        IndentReturnedItemList.Name = $(rows[i]).find("td:eq(1)").html();
        IndentReturnedItemList.BatchNo = $(rows[i]).find("td:eq(2)").html();
        IndentReturnedItemList.BatchID = $(rows[i]).find("td:eq(3)").html();
        IndentReturnedItemList.ExpiryDate = $(rows[i]).find("#spandate").val();
        IndentReturnedItemList.Quantity = $(rows[i]).find("#lblReqQty").val();
        IndentReturnedItemList.QOH = $(rows[i]).find("td:eq(6)").html();
        IndentReturnedItemList.UnitID = $(rows[i]).find("td:eq(7)").html();
        IndentReturnedItemList.Remarks = $(rows[i]).find("#lblRemarks").val();
        IndentReturnedItemList.DrugType = $(rows[i]).find("td:eq(9)").html();
        IndentReturnedItemList.ID = $(rows[i]).find("td:eq(10)").html();

        IndentReturnedItemList.issQty = $(rows[i]).find("td:eq(12)").html();
        IndentReturnedItemList.retQty = $(rows[i]).find("td:eq(13)").val();
        IndentReturnedItemList.converQty = $(rows[i]).find("td:eq(14)").html();
        IndentReturnedItemList.unit = $(rows[i]).find("td:eq(15)").html();

        ReturnList.push(IndentReturnedItemList);
    }
    IndentOrderModel.ReturnList = ReturnList;
    IndentOrderModel.ToStationID = $('select#ToStationList option:selected').val();
    IndentOrderModel.ItemCategory = $('select#lstItemCategory option:selected').val();
    IndentOrderModel.txtRef = $('#txtRef').val();
    IndentOrderModel.lbldate = $('#lbldate').val();
    IndentOrderModel.lblId = $('#lblId').val();
    IndentOrderModel.lbloperator = $('#lbloperator').val();
    IndentOrderModel.OrderID = $('#OrderID').val();
    IndentOrderModel.mIssueID = $('#IssueID').val();




    var URL = "/IndentReturn/Save";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify({ Order: IndentOrderModel }),
        success: function (msg) {
            if (msg.ErrMsg != null) {
                ShowMessage(msg.ErrMsg);
                clearcontrolls('view');
            }
        }
    });
    return false;


}
function clearcontrolls(type) {
    SelectItemsTable = null;
    $('#allItemsList').empty();
    $('#s2id_allItemsList').find('a').find('span').empty();
    $('#SelectedItemtbl').dataTable().fnClearTable();
    $('#SelectedItemtbl').dataTable().fnDestroy();

    $('#lblId').val('');
    $('#lbldate').val('');
    $('#txtRef').val('');
    $('#lbloperator').val('');
    $('#lstItemCategory').val('');
    $('#lstSections').val('');
    $('#ToStationList').val('');
    $('#IssueSlno').val('');
    $('#OrderID').val(0);
    $('#IssueID').val(0);
    CheckBox('#issueflag', false);
    if (type == 'view') {
        $('.ViewWidget').show();
        $('#btnShowData').click();
        $('.DetailWidget').hide();
    } else if (type == 'new') {
        $('.ViewWidget').hide();
        $('.DetailWidget').show();
        $('#listitemDiv').show();

        $('#TableDiv').prop('disabled', false);
        $('#txtRef').prop('disabled', false);
        $('#lstItemCategory').prop('disabled', false);
        $('#ToStationList').prop('disabled', false);
        $('#IssueSlno').prop('disabled', false);
        $('#issueflag').prop('disabled', false);
        $('#btnSave').prop('disabled', false);
        $('#btnSave').text('Save');
    }
    else if (type == 'detail') {          $('.ViewWidget').hide();
        $('.DetailWidget').show();
        $('#listitemDiv').hide();
        $('#TableDiv').prop('disabled', true);
        $('#txtRef').prop('disabled', true);
        $('#lstItemCategory').prop('disabled', true);
        $('#ToStationList').prop('disabled', true);
        $('#IssueSlno').prop('disabled', true);
        $('#btnSave').prop('disabled', false);
        $('#btnSave').text('Cancel');

    }
    else if (type == 'disable') {          $('.ViewWidget').hide();
        $('.DetailWidget').show();
        $('#listitemDiv').hide();
        $('#TableDiv').prop('disabled', true);
        $('#txtRef').prop('disabled', true);
        $('#lstItemCategory').prop('disabled', true);
        $('#ToStationList').prop('disabled', true);
        $('#IssueSlno').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
    }

    return false;

}
function callDialog(txt1, txt2) {
    if (GetDialogAnswer != true) {
        $('#dialogMSG').text(txt1);
        $('#jDialog').dialog('open');
        $('.ui-dialog-buttonpane').hide();
        $('.dialogOtherAlert').hide();
        if (txt2 != null) {
            $('#dialogAlert').text(txt2);
            $('#jDialog').dialog('open');
            $('.ui-dialog-buttonpane').hide();
            $('.dialogOtherAlert').show();
        }
    }
    return false;
}
$(document).on('click', '#dialogYes', function () {
    GetDialogAnswer = true;
    $('#jDialog').dialog('close');
    Cancel();
    GetDialogAnswer = false;
    return false;
});
$(document).on('click', '#dialogNo', function () {
    GetDialogAnswer = false;
    $('#jDialog').dialog('close');
    return false;
});


function GetData() {
    var ParamTable = {};
    ParamTable.Sdate = $('#FromDate').val();;
    ParamTable.Edate = $('#ToDate').val();;
    var URL = "/IndentReturn/View";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify(ParamTable),
        success: function (msg) {
            render_IndentView('#tblIndentView', msg);
            ColorTable('#tblIndentView');
        }
    });
    return false;
}
function render_IndentView(tableID, dataxc) {


    ViewtempTbl = $(tableID).dataTable({
        destroy: true,
        data: dataxc,
        paging: true,
        ordering: false,
        searching: true,
        info: false,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
           {
               data: 'IndentID', className: '', targets: [0],
               "mRender": function (data, type, full) {
                                                           return '<a href="#" class="IndentDetail">' + data + '</a>';
               }
           },
            { data: 'IndentDateTime', targets: [1], className: 'Align_Left' },
            { data: 'IndentTo', className: 'cAR-align-center', targets: [2] },
            { data: 'IndentByName', targets: [3], className: 'Align_Left' },
            { data: 'referenceno', className: 'Align_Rigt Date hidden', targets: [4] },
            { data: 'Status', targets: [5], className: 'hidden' },
            { data: 'ToStationID', targets: [6], className: 'Align_Left hidden' },
            { data: 'RetrunIssueID', targets: [7], className: 'Align_Left hidden' },
            { data: 'ReturnType', targets: [8], className: 'Align_Left hidden' }

        ]
    }).fnDraw();


    PresTbl = $(tableID).DataTable();

};
function ColorTable(tableID) {
    var Presrows = $(tableID).dataTable().fnGetNodes();
    for (var i = 0; i < Presrows.length; i++) {
        if ($(Presrows[i]).find("td:eq(5)").html() == 2) {
            $(Presrows[i]).children('td,th').css('background-color', 'rgb(153, 255, 153)');         }
        else {
            $(Presrows[i]).children('td,th').css('background-color', 'rgba(255, 182, 193, 0.59)');         }
    }
    return false;
}
$(document).on('click', '.IndentDetail', function () {


    var tr = $(this).parents('tr:first');
    var orderid = $(this).closest("tr").find('td:eq(0)').text();
    var status = $(this).closest("tr").find('td:eq(5)').text();

    if (status > 0) {         clearcontrolls('disable');
    } else {
        clearcontrolls('detail');
    }


    $('#OrderID').val(orderid);
    $('#lblId').val($(this).closest("tr").find('td:eq(0)').text());
    $('#lbldate').val($(this).closest("tr").find('td:eq(1)').text());

    $('#txtRef').val($(this).closest("tr").find('td:eq(4)').text());
    $('#lbloperator').val($(this).closest("tr").find('td:eq(3)').text());
    $('#ToStationList').val($(this).closest("tr").find('td:eq(6)').text());
    $('#IssueSlno').val($(this).closest("tr").find('td:eq(7)').text());


    var str = $(this).closest("tr").find('td:eq(8)').text();
    if (str == 'true') {
        CheckBox('#issueflag', true)
    } else {
        CheckBox('#issueflag', false)

    }


    $.ajax({
        type: "POST",
        url: GetAppName() + "/IndentReturn/ViewDetails",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ OrderID: orderid }),
        success: function (msg) {
            if (msg[0].ErrMsg != null) {
                ShowMessage(msg[0].ErrMsg);
            }
            InsertItemtoBill(msg);
            form_original_data = $("#validate").serialize();
            ColHide('#SelectedItemtbl', 11);
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });
});



function LoadCommonList() {
    
    LoadList('#ToStationList', " select ID,Name from station where stores=1 and id <> ",
                ' ', ' order by name ', true);

    
    LoadList('#lstItemCategory', 'select ID,Name from ItemGroup', ' ', ' order by name ', false);


    $("#jDialog").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });

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

};


$(document).on('click', '#btnPrint', function () {
    var id = Number($('#OrderID').val());
    PrintOrder(id, 'ReportDialog', "/IndentReturn/Print", "DivRpt");
    return false;
});


function LoadItems(Cat, St, Qoh, txt, slno) {
    try {
        txt = " where x.itemcode like '%" + txt + "%' or x.name like '%" + txt + "%' ";
        var strCriteria = "";
        var StrSql;
        if (Qoh > 0)
        { strCriteria = " and x.qoh > 0 "; }
        if (Cat == 0) {
            StrSql = "select x.id ID,x.itemcode + ' - ' + x.name Name,x.drugtype from"
                   + " (select distinct i.id,i.name,i.drugtype,i.itemcode from item i,batchstore b where b.stationid = " + St
                   + " and b.itemid = i.id and b.quantity > 0 and i.deleted = 0) x "
                   + " " + txt + strCriteria + " order by x.drugtype,x.itemcode,x.name";

        } else {
            StrSql = "select x.id ID,x.itemcode + ' - ' + x.name Name,x.drugtype from"
                   + " (select distinct i.id,i.name,i.drugtype,i.itemcode from item i,batchstore b where b.stationid = " + St
                   + " and b.itemid = i.id and b.quantity > 0 and i.deleted = 0 and i.categoryid = " + Cat + " ) x "
                   + " " + txt + strCriteria + " order by x.drugtype,x.itemcode,x.name";

        }

        if (slno > 0) {
            StrSql = " select top 1 b.Substituteid as ID,i.itemcode + ' - ' + i.name as Name "
               + " from IndentIssue a,IndentIssueDetail b,Item i where "
               + " a.id = b.issueid and a.destinationid = " + St + " and a.status = 1 and "
               + " a.stationslno = " + slno + " and b.substituteid = i.id ";

            $.ajax({
                type: "POST",
                url: GetAppName() + "/IndentOrder/LoadList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({ Str: StrSql, GroupBy: " ", OrderBy: " order by name" }),
                success: function (msg) {
                    $.each(msg, function (i, Item) {
                                                 $("#allItemsList").select2("data", { id: Item.ID, text: Item.Name }, true);
                    });
                },
                Failure: function (msg) {
                    ShowMessage("Failure:" + msg.Message);
                }
            });


            return false;

        }


        return StrSql;
    } catch (err) { ShowMessage(err.toString()); }

}
$(document).on('change', '#lstItemCategory', function () {

    $('#allItemsList').empty();
    $('#s2id_allItemsList').find('a').find('span').empty();
    SelectItemsTable = null;
    $('#SelectedItemtbl').dataTable().fnClearTable();
    $('#SelectedItemtbl').dataTable().fnDestroy();
    var CatID = $('select#lstItemCategory option:selected').val();
    return false;
});
$(document).on('change', '#lstQOH', function () {
    $('#allItemsList').empty();
    $('#s2id_allItemsList').find('a').find('span').empty();
    return false;
});

$(document).on('change', '#allItemsList', function () {
    var itemindex = $('#allItemsList').select2('val');
    if (itemindex > 0) {
        ItemSelect(itemindex);
    } else {
        ShowMessage('Please Select an Item First!');
    }
    return false;
});

$(document).on('click', '#btnAddItem', function (e) {
    var itemindex = $('#allItemsList').select2('val');
    if (itemindex > 0) {
        ItemSelect(itemindex);
    } else {
        ShowMessage('Please Select an Item First!');
    }
    return false;
});



function ItemSelect(ItemID) {
              var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
    if (rows.length == 10) { ShowMessage("Can't Order more than 10 Items per Order!"); return false; }
    var SelectedItem = [];
    for (var i = 0; i < rows.length; i++) {
        var IndentReturnedItemList = {};
        IndentReturnedItemList.SNO = $(rows[i]).find("td:eq(0)").html();
        IndentReturnedItemList.Name = $(rows[i]).find("td:eq(1)").html();
        IndentReturnedItemList.BatchNo = $(rows[i]).find("td:eq(2)").html();
        IndentReturnedItemList.BatchID = $(rows[i]).find("td:eq(3)").html();
        IndentReturnedItemList.ExpiryDate = $(rows[i]).find("#spandate").val();

        IndentReturnedItemList.Quantity = $(rows[i]).find("#lblReqQty").val();
        IndentReturnedItemList.QOH = $(rows[i]).find("td:eq(6)").html();
        IndentReturnedItemList.UnitID = $(rows[i]).find("td:eq(7)").html();
        IndentReturnedItemList.Remarks = $(rows[i]).find("#lblRemarks").val();
        IndentReturnedItemList.DrugType = $(rows[i]).find("td:eq(9)").html();
        IndentReturnedItemList.ID = $(rows[i]).find("td:eq(10)").html();

        IndentReturnedItemList.issQty = $(rows[i]).find("td:eq(12)").html();
        IndentReturnedItemList.retQty = $(rows[i]).find("td:eq(13)").val();
        IndentReturnedItemList.converQty = $(rows[i]).find("td:eq(14)").html();
        IndentReturnedItemList.unit = $(rows[i]).find("td:eq(15)").html();


        if (IndentReturnedItemList.ID == ItemID) {
            ShowMessage('Item Already Selected !');
            return false;
        }
        else {
                         SelectedItem.push(IndentReturnedItemList);
        }
    }
    var ToStationID = $('select#ToStationList option:selected').val();
    var IssueID = NullTOInt($('#IssueID').val(), 0);

    var URL = "/IndentReturn/InsertItem";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify({ ItemID: ItemID, ToStationID: ToStationID, IssueID: IssueID, SelectedItem: SelectedItem, }),
        success: function (msg) {
                         if (msg[0].ErrMsg != null) {
                ShowMessage(msg[0].ErrMsg);
                return false;
            }
            InsertItemtoBill(msg);
        }
    });
    return false;
}
function InsertItemtoBill(itm) {
    var IndentReturnedItemList = [];
    IndentReturnedItemList = itm;
    render_InsertSelectedItem('#SelectedItemtbl', IndentReturnedItemList);
    TempDataTbl = null;      $('#btnSave').focus();      return false;
}
function render_InsertSelectedItem(tblid, xItem) {
    var SelectItemsTable = $(tblid).dataTable({
        destroy: true,
        data: xItem,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
            { data: 'SNO', targets: [0], className: '', width: '3%', },
            { data: 'Name', targets: [1], className: '', width: '30%', },

            { data: 'BatchNo', targets: [2], className: '', width: '7%' },
            { data: 'BatchID', targets: [3], className: 'hidden', width: '7%' },
            {
                data: 'ExpiryDate', targets: [4], className: '', width: '15%',
                'mRender': function (data, type, full) {
                    return '<span class="display-mode" id="spandate">' + MyDate(data, 'dd M yy') + '</span>';
                }
            },

            {
                data: 'Quantity', targets: [5], className: 'Align_Right_Col_Red', width: '10%',
                'mRender': function (data, type, full) {
                    return '<span class="display-mode">' +
                    '<input type="text" id="lblReqQty" value="' + data + '" class="display-mode disable" style="Align_Right_Col" />' +
                    '</span>' +
                    '<input type="text" id="ReqQty" value="' + data + '" class="edit-mode hidden AutoSelectTxt" />';
                }
            },
            { data: 'QOH', targets: [6], width: '10%', className: ' ' },

            { data: 'UnitID', targets: [7], className: 'hidden', width: '0%' },
            {
                data: 'Remarks', targets: [8], className: 'Align_Right_Col_Red', width: '20%',
                'mRender': function (data, type, full) {
                    return '<span class="display-mode">' +
                    '<input type="text" id="lblRemarks" value="' + data + '" class="display-mode disable" style="Align_Right_Col" />' +
                    '</span>' +
                    '<input type="text" id="NewRemarks" value="' + data + '" class="edit-mode hidden AutoSelectTxt" />';
                }
            },
            { data: 'DrugType', targets: [9], className: 'hidden', width: '0%' },
            { data: 'ID', targets: [10], className: ' hidden' },
            {
                data: 'Action', targets: [11], className: 'TAC',
                "mRender": function (data, type, full) {
                    return '<a href="#"><span class="icon-ok save-user edit-mode hidden"></span></a> ' +
                    '<a href="#"><span class="icon-pencil edit-user display-mode "></span></a> ' +
                    '<a href="#"><span class="icon-trash cancel-user edit-mode hidden"></span></a>';
                }
            },
            { data: 'issQty', targets: [12], className: 'hidden', width: '0%' },
            { data: 'retQty', targets: [13], className: 'hidden', width: '0%' },
            { data: 'converQty', targets: [14], className: 'hidden', width: '0%' },
            { data: 'unit', targets: [15], className: 'hidden', width: '0%' }

        ]
    }).fnDraw();

    SelectItemsTable = $(tblid).DataTable();

    $('.disable').prop('disabled', true);

};
$(document).on("click", ".edit-user", function (e) {
    $(".edit-mode").addClass("hidden");
    $(".display-mode").removeClass("hidden");
    var tr = $(this).parents('tr:first');
    tr.find('.edit-mode').removeClass("hidden");
    tr.find('.display-mode').addClass("hidden");

    var qty = tr.find('#lblReqQty').val();
    $(this).parents('tr:first').find('#ReqQty').val(qty);
    $(this).parents('tr:first').find('#ReqQty').focus();

    var rmk = tr.find('#lblRemarks').text();
    $(this).parents('tr:first').find('#NewRemarks').text(rmk);

    var ItemID = $(this).closest("tr").find('td:eq(10)').text();
    return false;
});
$(document).on('click', '.save-user', function (e) {
    var tr = $(this).parents('tr:first');
          
    $(".edit-mode").addClass("hidden");
    $(".display-mode").removeClass("hidden");
    e.preventDefault();
    return false;
});
$(document).on('click', '.cancel-user', function (e) {
    var po = $(this).closest("tr").get(0);
    var iPos = $("#SelectedItemtbl").dataTable().fnGetPosition(po);
    if (iPos !== null) {
        $("#SelectedItemtbl").dataTable().fnDeleteRow(iPos);     }
    $(".edit-mode").addClass("hidden");
    $(".display-mode").removeClass("hidden");
    e.preventDefault();
    return false;

});
 $(document).on("change", "#ReqQty", function (e) {
    ChangeInfo(this);
});
$(document).on("change", "#NewRemarks", function (e) {
    ChangeInfo(this);
});
function ChangeInfo(obj) {
    var tr = $(obj).parents('tr:first');
    var Rem = tr.find('#NewRemarks').val();
    tr.find('#lblRemarks').val(Rem);
    var QOH = tr.find("td:eq(6)").html();
    var Qt = tr.find('#ReqQty').val();
    if (parseInt(QOH) >= parseInt(Qt)) {
        tr.find('#lblReqQty').val(Qt);
    } else {
        tr.find('#ReqQty').val(0);
        tr.find('#lblReqQty').val(0);
        Qt = 0;
    }
    return false;
}


function Validate() {
    var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
    if (rows.length == 0) { ShowMessage("Please select Items for order"); return false; }
    for (var i = 0; i < rows.length; i++) {
        {
            var itm = $(rows[i]).find("td:eq(1)").html();
            var qty = $(rows[i]).find("#lblReqQty").val();
            if (qty > 0 && qty <= $(rows[i]).find("td:eq(6)").html()) { }
            else {
                $("#SelectedItemtbl").dataTable().fnDeleteRow(i);
                                                               }

        }
        
        var Drows = $('#SelectedItemtbl').dataTable().fnGetNodes();
        if (Drows.length == 0) { ShowMessage("Please select Items for order"); return false; }

        var ToStationID = $('select#ToStationList option:selected').val();
        if (ToStationID <= 0 || ToStationID == null) {
            ShowMessage("Please select destination.");
            $(this).focus;
            return false;
        }
        var CatID = $('select#lstItemCategory option:selected').val();
        if (CatID < 0 || CatID == null) {
            ShowMessage("Please select the Category.");
            $(this).focus;
            return false;
        }
        return true;
    }
}




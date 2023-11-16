 
 
var dataLoad = '';
var dataPinList = '';
$(document).ready(function (e) {

    get_data();
    $('.ViewWidget').show();
    $('.DetailWidget').hide();
    $('.ViewTxtCSS').attr("readonly", "readonly");
    $("#category").val(7);  
    var currentdate = new Date();
    var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var datetime = "" + currentdate.getDate() + "/" + monthNames[currentdate.getMonth()] + "/" + currentdate.getFullYear() + "  " + currentdate.getHours() + ":" + currentdate.getMinutes() + ":" + currentdate.getSeconds();
    $("#DateTime").val(datetime);

    $("#SelectedPinNo option[value='0']").remove();

    $(".edit-mode").addClass("hidden");
    $(".display-mode").removeClass("hidden");
    var tr = $(this).parents('tr:first');
    tr.find('.edit-mode').removeClass("hidden");
    tr.find('.display-mode').addClass("hidden");

    LoadCommonList();


    try {

        $('#allItemsList').select2({
            ajax: {
                url: GetAppName() + "/DirectIpIssues/LoadListItembyDirectIpIssue",
                dataType: 'json',
                type: "POST",
                delay: 150,
                data: function (params) {
                    return {
                        Cat: parseInt($('select#category option:selected').val()), ItemCode: params,
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


    $('select').select2();

});

function clearcontrolls(type) {
    SelectItemsTable = null;
    $('#allItemsList').empty();
    $('#s2id_allItemsList').find('a').find('span').empty();
    $('#SelectedItemtbl').dataTable().fnClearTable();
    $('#SelectedItemtbl').dataTable().fnDestroy();
    $('#lstSections').removeClass('UnViewTxtCSS');
    $('#lstSections').addClass('ViewTxtCSS');
    $('#lstSections').prop('disabled', true);

    $('#lblId').val('');
    $('#lbldate').val('');
    $('#dtpBydate').val('');
    $('#txtRef').val('');
    $('#lbloperator').val('');
    $('#lstItemCategory').val('');
    $('#lstSections').val('');
    $('#ToStationList').val('');
    $('#OrderID').val(0);




    if (type == 'view') {
        $('.ViewWidget').show();
        $('.DetailWidget').hide();
    } else if (type == 'new') {
        $('.ViewWidget').hide();
        $('.DetailWidget').show();
        $('#SelectedPinNo').MazSelectTwoEnabled('true');
        $('#Patient_IpId').MazSelectTwoEnabled('true');
        $('#doctor').MazSelectTwoEnabled('true');
        $('#category').MazSelectTwoEnabled('true');
        $('#listitemDiv').show();
    } else if (type == 'edit') {
        $('.ViewWidget').hide();
        $('.DetailWidget').show();


        $('#listitemDiv').show();
        $('#SelectedItemtbl').prop('disabled', false);
        $('#dtpBydate').prop('disabled', false);
        $('#txtRef').prop('disabled', false);
        $('#lstItemCategory').prop('disabled', false);
        $('#ToStationList').prop('disabled', false);
        $('#btnSave').prop('disabled', false);
    }
    else if (type == 'detail') {          $('.ViewWidget').hide();
        $('.DetailWidget').show();
        $('#listitemDiv').hide();
        $('#SelectedItemtbl').prop('disabled', true);
        $('#dtpBydate').prop('disabled', true);
        $('#txtRef').prop('disabled', true);
        $('#lstItemCategory').prop('disabled', true);
        $('#ToStationList').prop('disabled', true);
        $('#btnSave').prop('disabled', true);
    }



    return false;

}
function LoadCommonList() {
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
function get_data(Ipid) {

    if (Ipid) {

        var end = Ipid;
        var ParamTable = {};
        ParamTable.IpId = end;

        var URL = "/DirectIpIssues/LoadList";
        $.ajax({
            url: GetAppName() + URL,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: ParamTable,             success: function (msg) {



                $("#PinNo").select2("val", "0");
                $.each(dataLoad, function (index, value) {

                    if (value.IpNo == end) {
                        console.log(value.IpNo);
                        $("#PinNo").select2("val", value.IpNo);

                    }
                });

                render_IndentView('#tblIndentView', msg);
                ColorTable('#tblIndentView');
            }
        });

    } else {
        var end = $("#PinNo").val();
        var ParamTable = {};
        ParamTable.IpId = end;

        var URL = "/DirectIpIssues/LoadList";
        $.ajax({
            url: GetAppName() + URL,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: ParamTable,             success: function (msg) {

                console.log(msg.length);
                if (msg.length > 0) {
                    dataLoad = msg;
                    render_IndentView('#tblIndentView', msg);
                    ColorTable('#tblIndentView');

                    $("#Bed option").each(function () {
                        if ($(this).text() == msg[0].Bed) {
                            $("#s2id_Bed").select2("val", $(this).val());
                        }
                    });


                } else {
                    $("#s2id_Bed").select2("val", "0");
                    $('#tblIndentView').dataTable().fnClearTable();
                    $('#tblIndentView').dataTable().fnDestroy();

                }

            }
        });

    }

}

$("#PinNo").change(function () {

    get_data();
});
$("#Bed").change(function () {
    get_data(this.value);
});
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
            { data: 'Id', className: 'hidden', targets: [0] },
              {
                  data: 'StationSLno', className: '', targets: "StationSLno",
                  "mRender": function (data, type, full) {
                                                                    return '<a href="#" class="IndentDetail">' + data + '</a>';
                  }
              },


                              { data: 'PatientName', className: 'cAR-align-center', targets: "PatientName" },
                { data: 'IpNo', className: 'cAR-align-center', targets: 'IpNo' },
                 { data: 'Bed', className: 'cAR-align-center', targets: 'Bed' },
                  { data: 'Doctor', className: 'cAR-align-center', targets: 'Doctor' },
                    {
                        data: 'DateTime', className: 'cAR-align-center Date', targets: 'DateTime',
                        "render": function (data) {
                            var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec"];
                            var date = eval(data.replace(/\/Date\((\d+)\)\                             return monthNames[date.getMonth()] + " -" + date.getDate() + "-" + date.getFullYear();
                        }
                    },
                   { data: 'Operator', className: 'cAR-align-center', targets: 'Operator' },
        ]
    }).fnDraw();


    PresTbl = $(tableID).DataTable();

};
function ColorTable(tableID) {
    var Presrows = $(tableID).dataTable().fnGetNodes();
    for (var i = 0; i < Presrows.length; i++) {
                          if ($(Presrows[i]).find("td:eq(5)").html() == 2) {
            $(Presrows[i]).children('td,th').css('background-color', 'rgb(153, 255, 153)');         }
        else if ($(Presrows[i]).find("td:eq(5)").html() == 1) {
            $(Presrows[i]).children('td,th').css('background-color', 'rgb(224, 224, 66)');         }
        else {
            $(Presrows[i]).children('td,th').css('background-color', 'rgba(255, 182, 193, 0.59)'); 
        }
    }
    return false;
}

$(document).on('click', '.IndentDetail', function (e) {

    get_PatientList();
    var tr = $(this).parents('tr:first');
    var orderid = $(this).closest("tr").find('td:eq(0)').text();
         clearcontrolls('detail');
         ViewDetailsOrderId(orderid);
    return false;


});
function ViewDetailsOrderId(orderid) {

    $.ajax({
        type: "POST",
        url: GetAppName() + "/DirectIpIssues/ViewDetails",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ OrderID: orderid }),
        success: function (msg) {

            if (msg.ErrMsg != null) {
                ShowMessage(msg.ErrMsg);
            }
            console.log('msg' + orderid);
            console.log(msg.IssueList);
            $('.edit-user').hide();
                         InsertItemtoBillView(msg.IssueList);


            $("#ajaxIpID").val(msg.IpId);

                                                   $("#DateTime").val(msg.DateTime);
                                                   $("#orderno").val(orderid);

                     }, complete: function (data) {

            getInfo();
            $('#btnSave').MazDisabled();             $('#SelectedItemtbl').MazHideCol(10);             $(".edit-mode").addClass("hidden");             $(".display-mode").removeClass("hidden");             $('#btnClose').focus();             $('#SelectedPinNo').MazSelectTwoEnabled(false);
            $('#Patient_IpId').MazSelectTwoEnabled(false);
            $('#doctor').MazSelectTwoEnabled(false);
            $('#category').MazSelectTwoEnabled(false);

        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });

    return false;
}
function getInfo() {



    var ParamTable = {};
    ParamTable.IpId = $("#ajaxIpID").val();
         var URL = "/DirectIpIssues/InformationLoadList";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify(ParamTable),
        success: function (msg) {
            console.log(msg);

            var checkAlertPatientIsInOperationTheatre = 0;
            var counter = 1;
            $.each(msg, function (index, value) {


                if (value.AlertMsg !== '' && value.AlertMsg != null) {
                    checkAlertPatientIsInOperationTheatre = checkAlertPatientIsInOperationTheatre + 1;
                } else {

                    if (counter == 1) {

                        $("#s2id_Patient_IpId").select2("val", value.IpId);
                        $("#s2id_SelectedPinNo").select2("val", value.IpId);
                        $("#s2id_doctor").select2("val", value.DoctorId);
                        $("#age").val(value.Age);
                        $("#BedId").val(value.BedId);
                        $("#BedNO").val(value.BedName);
                        $("#sex").val(value.Sex == '1' ? "Female" : "Male");
                        var tr = $(this).parents('tr:first');
                                                 $('.TAC').find('.icon-pencil').addClass("hidden");


                    }
                }
                counter = counter + 1;
            });
        }
    });


     }


$(document).on('click', '#btnNewOrder', function () {
    clearcontrolls('new');

    $('#AddItemDiv').hide();
    $('#btnSave').prop('disabled', true);
    $('#btnPrint').prop('disabled', true);
    get_PatientList();

    $("#SelectedPinNo option[value='0']").remove();

    $("#Patient_IpId").append(new Option("Select Pin", 0));
    $("#SelectedPinNo").append(new Option("Select Pin", 0));
    $("#Patient_IpId").MazSelect2Val(0);
    $("#SelectedPinNo").MazSelect2Val(0);


                         
    return false;
});
function get_PatientList() {

    var ParamTable = {};
    var URL = "/DirectIpIssues/LoadPatientList";
    $.ajax({
        url: GetAppName() + URL,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: ParamTable,         success: function (msg) {


            $.each(msg, function (i, Item) {
                $("#Patient_IpId").append(new Option(Item.Name, Item.Id));
            });

        }
    });


}
$("#SelectedPinNo , #Patient_IpId").change(function () {
    $('#AddItemDiv').show();
    $('#totalAmt').val(0);
    get_InformationData(this.value);
    $('#SelectedItemtbl').dataTable().fnClearTable();
    $('#SelectedItemtbl').dataTable().fnDestroy();

    $('#btnSave').prop('disabled', false);
    $('#btnPrint').prop('disabled', false);

    if ($('#orderno').val() == "") {

        $('#btnSave').prop('disabled', false);
        $('#btnPrint').prop('disabled', false);
        $('#allItemsList').select2("open"); 

    } else {

        $('#btnSave').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);

    }

});
$(document).on('change', '#category', function () {
    $('#SelectedItemtbl').dataTable().fnClearTable();
    $('#SelectedItemtbl').dataTable().fnDestroy();
    $('#allItemsList').select2("open"); });
function get_InformationData(Ipid) {

              if (Ipid) {
        var ParamTable = {};
        ParamTable.IpId = Ipid;

        var URL = "/DirectIpIssues/InformationLoadList";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify(ParamTable),
            success: function (msg) {
                console.log(msg);

                var checkAlertPatientIsInOperationTheatre = 0;
                var counter = 1;
                $.each(msg, function (index, value) {


                    if (value.AlertMsg !== '' && value.AlertMsg != null) {
                        checkAlertPatientIsInOperationTheatre = checkAlertPatientIsInOperationTheatre + 1;
                    } else {

                        if (counter == 1) {
                            $("#s2id_Patient_IpId").select2("val", value.IpId);
                            $("#s2id_SelectedPinNo").select2("val", value.IpId);
                            $("#s2id_doctor").select2("val", value.DoctorId);
                            $("#age").val(value.Age);
                            $("#BedId").val(value.BedId);
                            $("#BedNO").val(value.BedName);
                            $("#sex").val(value.Sex == '1' ? "Female" : "Male");
                        }
                    }
                    counter = counter + 1;
                });


                if (checkAlertPatientIsInOperationTheatre > 0) {
                    ShowMessageYesNo("Patient is in Operation Theatre. Do you Still want to raise this Order?", false);
                                     }




            }
        });

    }

}

$(document).on('click', '#btnAddItem', function (e) {
    var itemindex = $('#allItemsList').select2('val');

    if (itemindex > 0) {
        ItemSelect(itemindex);
    } else {
        ShowMessage('Please Select an Item First!');
    }
    return false;
});
$(document).on('change', '#allItemsList', function (e) {
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

    var IssueList = [];
    for (var i = 0; i < rows.length; i++) {
        var IndentIssueInsertedItemList = {};
        IndentIssueInsertedItemList.SNO = $(rows[i]).find("td:eq(0)").html();
        IndentIssueInsertedItemList.Name = $(rows[i]).find("td:eq(1)").html();
        IndentIssueInsertedItemList.quantity = $(rows[i]).find("td:eq(2)").html();
        IndentIssueInsertedItemList.mrp = $(rows[i]).find("td:eq(3)").html();
        IndentIssueInsertedItemList.PrevQty = $(rows[i]).find("#lblPrevQty").val();
        IndentIssueInsertedItemList.Unit = $(rows[i]).find("#UomInputName").val(); 
        IndentIssueInsertedItemList.Amount = $(rows[i]).find("td:eq(6)").html();
        IndentIssueInsertedItemList.BillQty = $(rows[i]).find("#lblPrevQty").val();          IndentIssueInsertedItemList.BillUnit = $(rows[i]).find("#lblBillUnit").val();
        IndentIssueInsertedItemList.Remarks = $(rows[i]).find("#lblRemarks").val();
        IndentIssueInsertedItemList.ID = $(rows[i]).find("td:eq(11)").html();
        IndentIssueInsertedItemList.IssueUnitID = $(rows[i]).find("td:eq(12)").html();
        IndentIssueInsertedItemList.unitId = $(rows[i]).find("td:eq(12)").html();

        if (IndentIssueInsertedItemList.ID == ItemID) {
            ShowMessage('Item Already Selected !');
            return false;
        }
        else {
            IssueList.push(IndentIssueInsertedItemList);
        }
    }

    var URL = "/DirectIpIssues/InsertItemDirectIPIssue";

    console.log(IssueList);
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify({ ItemID: ItemID, IssueList: IssueList, }),
        success: function (msg) {

            console.log(JSON.stringify(msg));
            if (msg[0].ErrMsg != null) {
                ShowMessage(msg[0].ErrMsg);
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
    TempDataTbl = null;      $('#btnSave').focus();      $('.edit-user').trigger('click');  
    return false;
}
function InsertItemtoBillView(itm) {
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

            { data: 'SNO', targets: "SNO", className: '', width: '3%', },
            { data: 'Name', targets: "Name", className: '', width: '35%', },
            { data: 'quantity', targets: "quantity", className: '', width: '8%', },
            { data: 'mrp', targets: "mrp", width: '8%' },

            {
                data: 'PrevQty', targets: "PrevQty", className: 'Align_Right_Col_Red', width: '10%',
                'mRender': function (data, type, full) {
                    return '<span class="display-mode">' +
                    '<input type="text" id="lblPrevQty" value="' + data + '" class="display-mode disable" style="Align_Right_Col" />' +
                    '</span>' +
                    '<input type="text" id=""  value="' + data + '" class="edit-mode hidden AutoSelectTxt PrevQty" />';
                     
                }
            },
             {
                 data: 'Unit', targets: "Unit", className: 'Align_Right_Col_Red', width: '10%',
                 'mRender': function (data, type, full) {
                     return '<input type="text" id="UomInputName" value="' + data + '" class=" display-mode disable" />'
                         + '<select id="UomList" class="UomAdd  edit-mode hidden"   />';
                 }

             },
             { data: 'Amount', targets: "Amount", className: '', width: '10%', },
             { data: 'BillQty', targets: "BillQty", className: '', width: '10%', },

                                                                                                                                                       { data: 'Unit', targets: "BillUnit", className: '', width: '10%', },
                                                                                                                                                          {
                      data: 'Remarks', targets: "Remarks", className: 'Align_Right_Col_Red', width: '10%',
                      'mRender': function (data, type, full) {
                          return '<span class="display-mode">' +
                          '<input type="text" id="lblRemarks" value="' + (data == null ? " " : data) + '" class="display-mode disable" style="Align_Right_Col" />' +
                          '</span>' + '<input type="text" id="Remarks" value="' + (data == null ? " " : data) + '" class="edit-mode hidden AutoSelectTxt" />';
                      }
                  },

            {
                data: 'TAC', targets: "TAC", className: 'TAC',
                "mRender": function (data, type, full) {
                    return '<a href="#"><span class="icon-ok save-user edit-mode hidden"></span></a> ' +
                    '<a href="#"><span class="icon-pencil edit-user display-mode "></span></a> ' +
                    '<a href="#"><span class="icon-trash cancel-user edit-mode hidden"></span></a>';
                }
            },
        { data: 'ID', targets: "Id", className: 'hidden ' },
        { data: 'unitId', targets: "unitId", className: 'hidden ' },
        { data: 'tax', targets: "tax", className: 'hidden ' },


        ]
    }).fnDraw();

    SelectItemsTable = $(tblid).DataTable();

    $('.disable').prop('disabled', true);

};
$(document).on('click', '.save-user', function (e) {
    var tr = $(this).parents('tr:first');
              $('#btnSave').prop('disabled', false);
    $('#btnPrint').prop('disabled', false);

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

    var amount = 0;
    $('#SelectedItemtbl tr').each(function () {
        amt = parseFloat($(this).find("td:eq(6)").text().replace(/[^0-9,.]/g, '').replace(',', '.')) || 0;
        console.log(amt);
        amount += amt;
    });

    $('#totalAmt').val(amount.toFixed(10).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));


    e.preventDefault();
    return false;

});
$(document).on("click", ".edit-user", function (e) {
    $(".edit-mode").addClass("hidden");
    $(".display-mode").removeClass("hidden");
    var tr = $(this).parents('tr:first');
    tr.find('.edit-mode').removeClass("hidden");
    tr.find('.display-mode').addClass("hidden");

    var qty = tr.find('#lblPrevQty').val();
    $(this).parents('tr:first').find('.PrevQty').val(qty);     $(this).parents('tr:first').find('.PrevQty').focus().select(); 

         var ItemID = $(this).closest("tr").find('td:eq(11)').text();

    var CurrentUOM = $(this).closest("tr").find('td:eq(4)').text();
    
    







    return false;
});
function addHidden(theForm, key, value) {
         var input = document.createElement('input');
    input.type = 'hidden';
    input.name = key;
    input.id = key;
    input.value = value;
    theForm.appendChild(input);
}
$(document).on("change", ".PrevQty", function (e) {
    ChangeInfo(this);
    var tr = $(this).parents('tr:first');


    var UListID = tr.find(".UomAdd").find('option:selected').val();
    var ConversionPrice = $('#conversionName_' + UListID).val();

    var tax = tr.find("td:eq(13)").text();
    if (ConversionPrice) {
                 var val = tax / 100 + tr.find("td:eq(3)").text() * $(this).val() * ConversionPrice;
    } else {

        var val = tax / 100 + tr.find("td:eq(3)").text() * $(this).val();
    }

    tr.find('td:eq(7)').text($(this).val());

    var amtperItem = parseFloat(val) || 0;

         tr.find("td:eq(6)").text(amtperItem.toFixed(2));

          

    var itm = tr.find("td:eq(1)").html();
    var amount = 0;
    $('#SelectedItemtbl tr').each(function () {
        amt = parseFloat($(this).find("td:eq(6)").text().replace(/[^0-9,.]/g, '').replace(',', '.')) || 0;
        console.log(amt);
        amount += amt;
    });

         $('#totalAmt').val(amount.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'));


});
$(document).on("change", ".UomAdd", function (e) {
    ChangeInfo(this);
    $(this).parents('tr:first').find('.PrevQty').trigger("change");
});
$(document).on("change", "#Remarks", function (e) {
    ChangeInfo(this);
});
function ChangeInfo(obj) {

    var tr = $(obj).parents('tr:first');
          

    var UList = tr.find(".UomAdd").find('option:selected').text();
         tr.find("#UomInputName").val(UList);


    var UListID = tr.find(".UomAdd").find('option:selected').val();
    var ConversionPrice = $('#conversionName_' + UListID).val();

    tr.find("td:eq(12)").text(UListID);
    console.log('unitID ' + tr.find("td:eq(12)").html());


    tr.find("td:eq(8)").text(UList);

    var Rem = tr.find('.PrevQty').val();
    tr.find('#lblPrevQty').val(Rem);
    tr.find('#BillQty').val(Rem);

    var Rem = tr.find('#BillUnit').val();
    tr.find('#lblBillUnit').val(Rem);

    var Rem = tr.find('#Remarks').val();

    tr.find('#lblRemarks').val(Rem == "" ? " " : Rem);

          
    var Qt = tr.find('#BillUnit').val();
    tr.find('#lblBillUnit').val(Qt);




    return false;

}
   $(document).on('keydown', '.PrevQty', function (event) {
    if (event.which == 39 || event.which == 13) {                            $(this).parents('tr:first').find('.UomAdd').focus().select();
    }
}); $(document).on('keydown', '.UomAdd', function (event) {
    if (event.which == 39 || event.which == 13) {          $('#allItemsList').select2("open");
    }
});  
$(document).on('click', '#btnSave', function () {



    if (Validate() == true) {
        var IndentOrderModel = {};
        var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
        if (rows.length == 10) { ShowMessage("Can't Order more than 10 Items per Order!"); return false; }
        var SelectedItem = [];
        for (var i = 0; i < rows.length; i++) {

            var IndentIssueInsertedItemList = {};
            IndentIssueInsertedItemList.SNO = $(rows[i]).find("td:eq(0)").html();
            IndentIssueInsertedItemList.Name = $(rows[i]).find("td:eq(1)").html();
            IndentIssueInsertedItemList.quantity = $(rows[i]).find("td:eq(2)").html();
            IndentIssueInsertedItemList.mrp = $(rows[i]).find("td:eq(3)").html();
            IndentIssueInsertedItemList.PrevQty = $(rows[i]).find("#lblPrevQty").val();
            IndentIssueInsertedItemList.Unit = $(rows[i]).find("#UomInputName").val();             IndentIssueInsertedItemList.Amount = $(rows[i]).find("td:eq(6)").html();
            IndentIssueInsertedItemList.BillQty = $(rows[i]).find("#lblPrevQty").val();              IndentIssueInsertedItemList.BillUnit = $(rows[i]).find("#UomInputName").val();             IndentIssueInsertedItemList.Remarks = $(rows[i]).find("#lblRemarks").val();
            IndentIssueInsertedItemList.ID = $(rows[i]).find("td:eq(11)").html();
            IndentIssueInsertedItemList.IssueUnitID = $(rows[i]).find("td:eq(12)").html();

            SelectedItem.push(IndentIssueInsertedItemList);
        }
        IndentOrderModel.IssueList = SelectedItem;

                 IndentOrderModel.CatId = $('select#category option:selected').val();
        IndentOrderModel.DocID = $('select#doctor option:selected').val();
                          IndentOrderModel.DateTime = $('#DateTime').val();
                 IndentOrderModel.lbloperator = $('#operator').val();
        IndentOrderModel.OrderID = $('#orderno').val();

        IndentOrderModel.IpId = $('#SelectedPinNo').val();
        IndentOrderModel.BedId = $('#BedId').val();

         

        var URL = "/DirectIpIssues/checkDispatch3days";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ Order: IndentOrderModel }),
            success: function (msg) {

                $('#IndentOrderObj').val(JSON.stringify(IndentOrderModel));
                if (msg.ErrMsg != null) {
                    ShowMessageYesNo(msg.ErrMsg, true);
                } else {
                    console.log('IndentOrderModel');
                    console.log(IndentOrderModel);
                    finalValidation();
                }

            }
        });



        return false;
    }

    return false;
});
function Validate() {

    var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
    if (rows.length == 0) {
        ShowMessage("Please select Items for order"); $(this).focus;
        return false;
    }
    var isQty = 0;
    for (var i = 0; i < rows.length; i++) {
        var itm = $(rows[i]).find("td:eq(1)").html();
        var qty = $(rows[i]).find("#lblPrevQty").val();

        var Unit = $(rows[i]).find("#UomInputName").val();
        var UomList = $(rows[i]).find("#UomList").val();


        if (Unit == "") {
            ShowMessage("Please enter Unit for item " + itm);
            $(this).focus;
            return false;

        }
        if (UomList == "") {
            ShowMessage("Please enter Unit for item " + itm);
            $(this).focus;
            return false;

        }


        if (qty > 0) { }
        else {
            ShowMessage("Please enter quantity for item " + itm);
            $(this).focus;
            return false;
        }

                 var qty = $(rows[i]).find("#lblPrevQty").val();

        if (parseInt($(rows[i]).find("td:eq(2)").text()) < parseInt(qty)) {
            isQty = isQty + 1;
            ShowMessage("Can't Order more than QOH for Item " + itm);
            $(this).focus;
            return false;
        }




         
                                                      
    }

    if (isQty > 0) {
        $('#btnSave').prop('disabled', true);
        $('#btnPrint').prop('disabled', true);
        return false;
    } else {
        $('#btnSave').prop('disabled', false);
        $('#btnPrint').prop('disabled', false);
    }


    var ToStationID = $('select#SelectedPinNo option:selected').val();
    if (ToStationID <= 0 || ToStationID == null) {
        ShowMessage("Please select Patient No. / Name! ");
        $(this).focus;
        return false;
    }
                              
    return true;

}
function ShowMessageYesNo(Msg, save) {

    if (save == true) {
        BtnYes = 'Yes3dayscheckStation';
    } else {
        BtnYes = 'YesPatientOperation';
    }
    $('#btnShowMsg').hide();
    $('#bModal').hide();
    $('#DivMsg').html(
         '<a href="#bModal" role="button" class="btn" data-toggle="modal" id="btnShowMsg">Modal dialog</a>  ' +
         '<div id="bModal" class="modal hide fade in" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="false" style="display: block;">' +
        ' <div class="modal-header">' +
            '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>' +
        '    <h3 id="myModalLabel">Message</h3>' +
        '</div>' +
        '<div class="modal-body" style="font-size:14px;">' +
            '<p>' + Msg + ' </p>' +
        '</div>' +
        '<div class="modal-footer">' +
            '<button class="btn btn-info" id="' + BtnYes + '" data-dismiss="modal"  aria-hidden="true">Yes</button>  <button class="btn btn-danger" data-dismiss="modal" aria-hidden="true" id="patientOperationTheate">No</button>            ' +
        '</div>' +
    '</div>' +
    '</div>'
        );
         $('#bModal').hide();
    $('#btnShowMsg').click();
     
    return false;
}
$(document).on('click', '#patientOperationTheate', function (e) {
    clearcontrolls('view');

});
$(document).on('click', '#Yes3dayscheckStation', function (e) {

    finalValidation();
});
function finalValidation() {

    IndentOrderModel = JSON.parse($('#IndentOrderObj').val());

    var URL = "/DirectIpIssues/Save";
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


    return true;


}

$(document).on('click', '#btnPrint', function () {

    var id = Number($('#orderno').val());
     
    PrintOrder(id, 'ReportDialog', "/DirectIpIssues/Print", "DivRpt");
    return false;
});
function PrintOrder(id, dialogid, WebPage, DivID) {

    if (id > 0) {
        $('#' + dialogid).dialog('open');
        var URL = GetAppName() + WebPage;
                 $.ajax({
            url: URL,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            dataType: 'json',
            data: JSON.stringify({ OrderID: id }),
            success: function (data) {
                $('#' + DivID).html(data.htmlData);
            }
        });


    }
    return false;
}

$(document).on('click', '#btnClose', function () {
    $('.ViewWidget').show();
    $('.DetailWidget').hide();
    $("#age").val('');
    $("#sex").val('');
    $("#BedNO").val('');
    $("#orderno").val('');
    $("#allergies").val('');
    $("#totalAmt").val('');
    return false;
});


function format(x) {
    if (isNaN(x)) return "";

    n = x.toString().split('.');
    return n[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (n.length > 1 ? "." + n[1] : "");
}
function GetAppName() {
    var xx = $('.AppName').html();
    return xx;
}


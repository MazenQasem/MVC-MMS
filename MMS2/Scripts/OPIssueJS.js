  

 var MsgTbl;
var PresTbl;
var PresDtlTbl;
var DiagnosisTbl;
var SearchTbl;
var GetDialogAnswer;
var DialCalledBy; var TempDataTbl;  var SelectItemsTable;
var MprescriptionID;
var ScannerBool; 
$(document).ready(function () {

    ScannerBool = $('#ScannerBool').val().toLowerCase();     $('#ScannerList').val(ScannerBool);

    
    

    var myLength = $("#ErrMsg").val().length;
    if (myLength > 0 && $('#txtpin').val() == 0) {
        ShowMessage(">>:" + $("#ErrMsg").val());
        $("#ErrMsg").val("");
    }
    

    $('.ViewTxtCSS').attr("readonly", "readonly");
    $('.VIPCLASS').hide();
    $('#DocIdHolder').val(0);
    GetDialogAnswer = false;
    LoadCommonList();
    $('#txtpin').addClass("AutoSelectTxt");
    $("#txtpin").focus();



    
    $('#txtpin').keypress(function (event) {
        var len = 0;
        len = $("#txtpin").val().length;
        if (event.which == 13) {
            if (len >= 3) {
                $("#txtpin").attr("readonly", "readonly");
                GetPatient(0);



                return false;
            }
        }
    });


    $('#DoctorList').change(function () {
        $('#mDeptId').val('0');         if ($('#DocIdHolder').val() != $('#DoctorList').val()) {
            if (parseInt($('#DocIdHolder').val()) > 0) {
                var docid = $('#DoctorList').val();
                GetCOMPANY(docid);
                LoadPrescription(docid);
                $('#DocIdHolder').val(parseInt($('#DoctorList').val()));
            } else if (parseInt($('#DocIdHolder').val()) == 0) {
                $('#DocIdHolder').val(parseInt($('#DoctorList').val()));
            }
        }

        if (parseInt($('#mDeptId').val()) == 0 && (parseInt($('#DocIdHolder').val()) > 0 || parseInt($('#DoctorList').val()) > 0)) {
            var DeptID = 0;
            DeptID = GetNameOrValVar("select departmentid  from employee where id = " + parseInt($('#DoctorList').val()), "departmentid", DeptID);
            $('#mDeptId').val(parseInt(DeptID));
            console.log($('#mDeptId').val());

        }

        console.log("doctor change : " + $('#DoctorList').val() + "    holder:" + $('#DocIdHolder').val());
    });



    $('#cashbtn').change(function () {
        var CbtnStatus = $('#cashbtn').attr('checked');
        if (CbtnStatus == 'checked') {
            var N = false;
            $('#BillisCredit').val(N);

        }
    });
    $('#DepositChkNo').focusout(function (event) {
        var len = 0;
        len = $("#DepositChkNo").val().length;

        if (len >= 3 && $("#txtpin").val().length > 3) {
            $("#cashbtn").trigger('click');
            getDeposit();
        }

    });



          });
function LoadCommonList() {
    GetDoctorList();
    GetDisDoctorList();
    GetItemList();
    GetDiscountList();
    GetBillType();
                                                 $("#jDialog").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });
    $("#ViewDialog").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });
    $("#SearchBillDialog").dialog({
        autoOpen: false,
        modal: true,
        width: 1093,
        position: ['center', 50],
        open: function () {
                         fix();
        }
    });
    $("#SearchBillDialogDetail").dialog({
        autoOpen: false,
        modal: true,
        width: 1093,
        position: ['center', 50],
        open: function () {
                         fix();
        }
    });
    $("#ReportDialog").dialog({
        autoOpen: false,
        modal: true,
        width: 1093,
                 resizable: false,
        title: 'Print out',
        position: ['center', 50],
        open: function () {
                         var URL = GetAppName() + '/OPIssue/test';
            ViewPartial(URL, '#DivRpt');
                                               },
        buttons: {
            "Print": function () {
                printContent();
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });


         $('#ModelPresOrderHeader').dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        heigh: 600,
        position: ['center', 255],
        open: function () {
                         fix();
        }
    });
         $('#ModelDiagnosis').dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        heigh: 600,
        position: ['center', 255],
        open: function () {
                         fix();
        }
    });

         $('#ModelAlert').dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        heigh: 600,
        position: ['center', 255],
        open: function () {
                         fix();
        }
    });

         $('#ModelCompanyDtl').dialog({
        autoOpen: false,
        modal: true,
        width: 1200,
        heigh: 600,
        position: ['center', 255],
        open: function () {
                         fix();
        }
    });


    $('#ModelViewOrder').dialog({
        autoOpen: false,
        modal: true,
        width: 1200,
        heigh: 600,
        position: ['center', 255],
        open: function () {
                         fix();
        }
    });

    $('#ModelBatchDtl').dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        heigh: 600,
        position: ['right', 255],
        open: function () {
                         fix();
        }
    });
};
 function printContent() {
    var contents = $("#rptdiv").html();
    var frame1 = $('<iframe />');
    frame1[0].name = "frame1";
    frame1.css({ "position": "absolute", "top": "-1000000px" });
    $("body").append(frame1);
    var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
    frameDoc.document.open();
         frameDoc.document.write('<html><head><title>DIV Contents</title>');
    frameDoc.document.write('</head><body>');
         frameDoc.document.write('<link href="style.css" rel="stylesheet" type="text/css" />');
         frameDoc.document.write(contents);
    frameDoc.document.write('</body></html>');
    frameDoc.document.close();
    setTimeout(function () {
        window.frames["frame1"].focus();
        window.frames["frame1"].print();
        frame1.remove();
    }, 500);
}
$(document).on("click", "#btnSerchBill", function (e) {
    $('#SearchBillDialog').dialog('open');
    $('#srchBillNo').val(0);
         SearchTbl = $('#tblSearchBill').dataTable();
    $(SearchTbl).dataTable().fnClearTable();
    $(SearchTbl).dataTable().fnDestroy();
});
$(document).on("click", "#GetSearchBill", function (e) {
    var billno = $('#srchBillNo').val();
    var billtype = $('select#srchBillTypelist option:selected').val();

    if (billtype == null || billtype < 0) {
        ShowMessage("Please select Bill Type !");
        $("#srchBillTypelist").focus();
        return false;
    }
    URL = "/OpIssue/SearchBills";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { billno: billno, billtype: billtype },
        success: function (data) {

            var myLength = data[0].ErrMsg;
            if (data[0].ErrMsg != null && myLength.length > 0) {
                ShowMessage(data[0].ErrMsg);
            } else {
                render_InsertSearchedBIll("#tblSearchBill", data);
            }

        }
    });
    return false;


});
function render_InsertSearchedBIll(tblid, Datalist) {

         var SearchTbl = $(tblid).dataTable({
        destroy: true,
        data: Datalist,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
            {
                data: 'ID', targets: [0], className: 'hidden', width: '0%',

            },
            { data: 'slno', targets: [1], className: '', width: '3%', },
            { data: 'PatientName', targets: [2], className: '', width: '27%', },
            { data: 'DoctorName', targets: [3], className: '', width: '20%', },
            { data: 'Date', targets: [4], className: '', width: '10%', },
            { data: 'OperatorName', targets: [5], className: '', width: '20%', },
            {
                data: 'BillNo', targets: [6], className: '', width: '20%',
                "mRender": function (data, type, full) {
                                                              return '<a href="#" class="SearchDtlClass">' + data + '</a>';
                }
            },
            { data: 'Creditbillid', targets: [7], className: 'hidden', width: '0%', },
            { data: 'amount', targets: [8], className: 'hidden', width: '0%', },
            { data: 'Canceled', targets: [9], className: 'hidden', width: '0%', },
            { data: 'canceledby', targets: [10], className: 'hidden', width: '0%', },
            { data: 'compcredit', targets: [11], className: 'hidden', width: '0%', },
            { data: 'billtype', targets: [12], className: 'hidden', width: '0%', }

        ]
    }).fnDraw();

    SearchTbl = $(tblid).DataTable();

};
$(document).on("click", ".SearchDtlClass", function () {
    var tr = $(this).parents('tr:first');

    var Patient = {};
    Patient.PatientName = $(this).closest("tr").find('td:eq(2)').text();
    Patient.OperatorName = $(this).closest("tr").find('td:eq(5)').text();
    Patient.lbldate = $(this).closest("tr").find('td:eq(4)').text();
    Patient.COBILLNO = $(this).closest("tr").find('td:eq(0)').text();
    Patient.lblBillNo = $(this).closest("tr").find('td:eq(6)').text();

    var billtype = $('select#srchBillTypelist option:selected').val();
    if (billtype == 1)
    { Patient.BillisCredit = true; } else { Patient.BillisCredit = false; }
    $.ajax({
        url: GetAppName() + '/OpIssue/ViewDetail',
        type: "POST",
        data: JSON.stringify(Patient),
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            $("#SearchBillDialogDetail").dialog('open');
            $('#target').html(msg);
        }
    });


    $.ajax({
        url: GetAppName() + '/OpIssue/ViewDetailItems',
        type: "POST",
        data: JSON.stringify(Patient),
        contentType: "application/json; charset=utf-8",
        success: function (msg) {
            var InsertItemValue = [];
            InsertItemValue = msg.InsertItemValue;
            render_InsertSelectedItem2('#SelectedItemtbl2', InsertItemValue);
        }
    });




    return false;
});
function render_InsertSelectedItem2(tblid, xItem) {

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
            {
                data: 'DrugName', targets: [1], width: '40%',
            },
            { data: 'BatchNo', targets: [2], className: ' hidden' },
            { data: 'qoh', targets: [3], width: '5%' },

            { data: 'Highunit', targets: [4], className: '', width: '12%', },
            { data: 'price', targets: [5], className: 'Align_Right_Col', width: '7%', },
            { data: 'tax', targets: [6], className: 'Align_Right_Col', width: '2%', },
            {
                data: 'qty', targets: [7], className: 'Align_Right_Col_Red', width: '7%',
            },

            {
                data: 'NewUomName', targets: [8], className: '', width: '15%',
            },
            {
                data: 'amount', targets: [9], className: 'Align_Right_Col_Red .NumRoundTwo', width: '10%',
            },
            { data: 'NewUomName', targets: [10], className: ' hidden' },
            { data: 'DispatchQty', targets: [11], className: ' hidden' },

            { data: 'qoh2', targets: [12], className: ' hidden' },
            { data: 'Drugtype', targets: [13], className: ' hidden' },
            { data: 'conversionqty', targets: [14], className: ' hidden' },
            { data: 'batchid', targets: [15], className: ' hidden' },

            { data: 'purqty', targets: [16], className: ' hidden' },
            { data: 'lsmallqty', targets: [17], className: ' hidden' },
            { data: 'NewUomID', targets: [18], className: ' hidden' },
            { data: 'PrescriptionID', targets: [19], className: ' hidden' },

            { data: 'Deductabletype', targets: [20], className: ' hidden' },
            { data: 'DeductablePerAmounttype', targets: [21], className: ' hidden' },
            { data: 'DeductablePerAmount', targets: [22], className: ' hidden' },
            { data: 'DiscountPerAmountType', targets: [23], className: ' hidden' },

            { data: 'DiscountPerAmount', targets: [24], className: ' hidden' },
            {
                data: 'OrderedItem', targets: [25], className: ' hidden'

            },
            { data: 'OrderedItemid', targets: [26], className: ' hidden' },
            { data: 'temp3', targets: [27], className: ' hidden' },

            { data: 'Name', targets: [28], className: ' hidden ' },
            { data: 'ItemCode', targets: [29], className: ' hidden' },
            { data: 'ID', targets: [30], className: ' hidden' }
        ]
    }).fnDraw();

    SelectItemsTable = $(tblid).DataTable();

    $('.disable').prop('disabled', true);
};
    $(document).on('click', '#Cancel', function (e) {
    window.location.replace(GetAppName() + "/OpIssue/Index");

});
$(document).on('click', '#Save', function (e) {
    Bill("View");
});
function getDeposit() {

    var DepositChkNo = $('#DepositChkNo').val();
    var RegNo = $('#txtpin').val();
    var gIACode = $('#IssueAuthorityCode').val();

    URL = "/OpIssue/CheckDeposit";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { DepositChkNo: DepositChkNo, RegNo: RegNo, gIACode: gIACode },
        success: function (data) {
            ShowMessage(data.NotifyMsg);
            $("#DepositID").val(data.DepositID);
            $("#NetBalance").val(data.NetBalance);
            $("#txtDepositAmt").val(data.NetBalance);
            $("#IPBalance").val(data.IPBalance);
        }
    });
    return false;
}
$(document).on('focus', '.AutoSelectTxt', function () {
    $(this).select();
});
$(document).on('click', '.AutoSelectTxt', function () {
    $(this).select();
});
$(document).on('click', '.PresDtlClass', function () {
    var tr = $(this).parents('tr:first');
    var OrderNo = $(this).closest("tr").find('td:eq(0)').text();
    var Date = $(this).closest("tr").find('td:eq(1)').text();
    var DocID = $(this).closest("tr").find('td:eq(4)').text();
    var MSFPresc = $(this).closest("tr").find('td:eq(7)').text();
    var regNumber = $('#txtpin').val();
    DisplayPrescriptionDtl(OrderNo, Date, regNumber, DocID, MSFPresc);
    $('#btnPrescription').trigger('click');



    
    DisplayDiagnosis(regNumber, OrderNo);
});

$(document).on("click", ".SelectPresDtlClass", function () {
    var tr = $(this).parents('tr:first');
    tr.addClass("SelectedRow");
    var OrderNo = $(this).closest("tr").find('td:eq(7)').text();
    var DrugId = $(this).closest("tr").find('td:eq(0)').text();
    var Qty = $(this).closest("tr").find('td:eq(5)').text();
    MprescriptionID = OrderNo;
    $("#allItemsList option[value='" + DrugId + "']").attr('selected', 'selected');
    $("#btnAddItem").trigger('click');
    $('#Save').focus();      return false;
});

function GetDiscountList() {
    var Str = "select id as ID,Name from OPDiscount where deleted=0 order by name";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/OpIssue/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: Str, AddNone: true }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#DiscountList").append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });

    return false;
}
function GetDoctorList() {
    var Str = "select id as ID,empcode + '-' + name as Name from doctor where deleted =0 order by Name";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/OpIssue/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: Str, AddNone: true }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#DoctorList").append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });

    return false;
}
function GetDisDoctorList() {
                   var Str = "exec MMS_PHARMALIST " + gStationID() + "," + gModelID() + ", 1 ";


    $.ajax({
        type: "POST",
        url: GetAppName() + "/OpIssue/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: Str, AddNone: false }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#DisDoctorList").append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });

    return false;
}
function GetItemList() {
              var Str = "select i.ID as ID,i.ItemCode + ' - ' + i.Name as Name " +
            " from item i left join batchstore b on i.ID=b.ItemID  " +
            " ,BATCH BB " +
            " where i.Deleted=0 and b.Quantity>0 and B.ITEMID=BB.ITEMID AND "
            + " B.BATCHID=BB.BATCHID AND B.BATCHNO=BB.BATCHNO AND "
            + "  b.StationID=";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/OpIssue/LoadList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: Str, GroupBy: " group by i.ID ,i.ItemCode,i.Name  ", OrderBy: "" }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#allItemsList").append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });

    return false;

}
function GetBillType() {
    $("#srchBillTypelist").append(new Option("CASH", 0));
    $("#srchBillTypelist").append(new Option("CREDIT", 1));
    $("select#srchBillTypelist").find("CASH").attr("selected", "selected");
}
  $(document).on('click', '#btnAddItem', function (e) {
    if ($('select#allItemsList option:selected').val() > 0) {
        if (MprescriptionID > 0) {
            ItemSelect(MprescriptionID)
        } else {
            if (ScannerBool == false || ScannerBool == 'False' || ScannerBool == 'false') {
                ItemSelect(0);
            } else {
                ScannerBool_ItemSelect2();
            }
        };
    }
    else {
        if (MprescriptionID > 0) {

            ShowMessage('Please Select another Item. this item has no stock!');
        } else {
            ShowMessage('Please Select an Item First!');
        }
    }

    return false;
});
function ItemSelect(MprescriptionID, GenericID, GenericName, OItemID, OItemName) {
    var ItemSelectParam = {};
    if (GenericID > 0) {
        ItemSelectParam.mItemID = GenericID;
        ItemSelectParam.mItemName = GenericName;
        ItemSelectParam.OrderItemId = OItemID;
        ItemSelectParam.OrderedItemName = OItemName;

    } else {
        ItemSelectParam.mItemID = $('select#allItemsList option:selected').val();
        ItemSelectParam.mItemName = $('select#allItemsList option:selected').html();
    }
    ItemSelectParam.mDeptid = $('#mDeptId').val();
    ItemSelectParam.mAuthorityid = $('#mAuthorityid').val();
    ItemSelectParam.cmbCompany = $('#CompanyId').val();
    ItemSelectParam.mCategoryid = $('#CategoryId').val();
    ItemSelectParam.mGradeId = $('#GradeId').val();
    ItemSelectParam.RegNo = $('#txtpin').val();
    ItemSelectParam.gIACode = $('#IssueAuthorityCode').val();
    ItemSelectParam.optCrPat = $('#BillisCredit').val();
    ItemSelectParam.mPrescription = MprescriptionID;

         var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
    var InsertItemValue = [];      for (var i = 0; i < rows.length; i++) {
        var InsertedItemsList = {};          InsertedItemsList.SNO = $(rows[i]).find("td:eq(0)").html();
                 InsertedItemsList.DrugName = $(rows[i]).find("#DrugName").val();
        InsertedItemsList.BatchNo = $(rows[i]).find("td:eq(2)").html();
        InsertedItemsList.qoh = $(rows[i]).find("td:eq(3)").html();
        InsertedItemsList.Highunit = $(rows[i]).find("td:eq(4)").html();
        InsertedItemsList.price = $(rows[i]).find("td:eq(5)").html();
        InsertedItemsList.tax = $(rows[i]).find("td:eq(6)").html();
                 InsertedItemsList.qty = $(rows[i]).find("#ReqQty").val();
        InsertedItemsList.UnitList = $(rows[i]).find("td:eq(8)").html();
        InsertedItemsList.amount = $(rows[i]).find("td:eq(9)").html();
                 InsertedItemsList.NewUomName = $(rows[i]).find("#UomInputName").val();
        InsertedItemsList.DispatchQty = $(rows[i]).find("td:eq(11)").val();


        InsertedItemsList.qoh2 = $(rows[i]).find("td:eq(12)").html();
        InsertedItemsList.Drugtype = $(rows[i]).find("td:eq(13)").html();
        InsertedItemsList.conversionqty = $(rows[i]).find("td:eq(14)").html();
        InsertedItemsList.batchid = $(rows[i]).find("td:eq(15)").html();
        InsertedItemsList.purqty = $(rows[i]).find("td:eq(16)").html();
        InsertedItemsList.lsmallqty = $(rows[i]).find("td:eq(17)").html();

        InsertedItemsList.NewUOMID = $(rows[i]).find("td:eq(18)").html();

        InsertedItemsList.PrescriptionID = $(rows[i]).find("td:eq(19)").html();
        InsertedItemsList.Deductabletype = $(rows[i]).find("td:eq(20)").html();
        InsertedItemsList.DeductablePerAmounttype = $(rows[i]).find("td:eq(21)").html();
        InsertedItemsList.DeductablePerAmount = $(rows[i]).find("td:eq(22)").html();
        InsertedItemsList.DiscountPerAmountType = $(rows[i]).find("td:eq(23)").html();
        InsertedItemsList.DiscountPerAmount = $(rows[i]).find("td:eq(24)").html();

        InsertedItemsList.OrderedItem = $(rows[i]).find("td:eq(25)").html();


        InsertedItemsList.OrderedItemid = $(rows[i]).find("td:eq(26)").html();
        InsertedItemsList.temp3 = $(rows[i]).find("td:eq(27)").html();
        InsertedItemsList.Name = $(rows[i]).find("td:eq(28)").html();
        InsertedItemsList.ItemCode = $(rows[i]).find("td:eq(29)").html();
        InsertedItemsList.ID = $(rows[i]).find("td:eq(30)").html();
        InsertedItemsList.MinLevel = $(rows[i]).find("td:eq(32)").html();
        
                 if (ItemSelectParam.mItemID == InsertedItemsList.ID) {
            ShowMessage('Item Already Selected !'); return false;
        }
        else {
            InsertItemValue.push(InsertedItemsList);
        }
    }
    ItemSelectParam.InsertItemValue = InsertItemValue;
    ItemSelectParam.GenericID = GenericID;
    URL = "/OpIssue/InsertItem";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify(ItemSelectParam),
        success: function (msg) {
            if (msg.ErrorFlag == null || msg.ErrorFlag == false) {
                if (msg.AlertDrugInteraction == true) {
                    TempDataTbl = msg;
                    callDialog(msg.StrWhole + ", Do you Want to issue this Drug?", msg.ErrMsg, 2);

                }
                else if (msg.AlertItemIssuedAlerdy == true) {
                    TempDataTbl = msg;
                    callDialog("Item already issued to the patient against Prescription, Do you Want to issue this Drug?",
                        msg.ErrMsg, 3);
                }

                else {
                    InsertItemtoBill(msg);
                }
            } else { ShowMessage(msg.ErrMsg); }
        },
        Failure: function (msg) { ShowMessage("Failure:" + msg.Message); }
    });



    return false;


}
function ScannerBool_ItemSelect2() {
    var ItemSelectParam = {};
    ItemSelectParam.mItemID = $('select#allItemsList option:selected').val();
    ItemSelectParam.mItemName = $('select#allItemsList option:selected').html();
    ItemSelectParam.mDeptid = $('#mDeptId').val();
    ItemSelectParam.mAuthorityid = $('#mAuthorityid').val();
    ItemSelectParam.cmbCompany = $('#CompanyId').val();
    ItemSelectParam.mCategoryid = $('#CategoryId').val();
    ItemSelectParam.mGradeId = $('#GradeId').val();
    ItemSelectParam.RegNo = $('#txtpin').val();
    ItemSelectParam.gIACode = $('#IssueAuthorityCode').val();
    ItemSelectParam.optCrPat = $('#BillisCredit').val();
    ItemSelectParam.mPrescription = MprescriptionID;

         var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
    var InsertItemValue = [];      for (var i = 0; i < rows.length; i++) {
        var InsertedItemsList = {};          InsertedItemsList.SNO = $(rows[i]).find("td:eq(0)").html();
                 InsertedItemsList.DrugName = $(rows[i]).find("#DrugName").val();
        InsertedItemsList.BatchNo = $(rows[i]).find("td:eq(2)").html();
        InsertedItemsList.qoh = $(rows[i]).find("td:eq(3)").html();
        InsertedItemsList.Highunit = $(rows[i]).find("td:eq(4)").html();
        InsertedItemsList.price = $(rows[i]).find("td:eq(5)").html();
        InsertedItemsList.tax = $(rows[i]).find("td:eq(6)").html();
                 InsertedItemsList.qty = $(rows[i]).find("#ReqQty").val();
        InsertedItemsList.UnitList = $(rows[i]).find("td:eq(8)").html();
        InsertedItemsList.amount = $(rows[i]).find("td:eq(9)").html();
                 InsertedItemsList.NewUomName = $(rows[i]).find("#UomInputName").val();
        InsertedItemsList.DispatchQty = $(rows[i]).find("td:eq(11)").val();


        InsertedItemsList.qoh2 = $(rows[i]).find("td:eq(12)").html();
        InsertedItemsList.Drugtype = $(rows[i]).find("td:eq(13)").html();
        InsertedItemsList.conversionqty = $(rows[i]).find("td:eq(14)").html();
        InsertedItemsList.batchid = $(rows[i]).find("td:eq(15)").html();
        InsertedItemsList.purqty = $(rows[i]).find("td:eq(16)").html();
        InsertedItemsList.lsmallqty = $(rows[i]).find("td:eq(17)").html();

        InsertedItemsList.NewUOMID = $(rows[i]).find("td:eq(18)").html();

        InsertedItemsList.PrescriptionID = $(rows[i]).find("td:eq(19)").html();
        InsertedItemsList.Deductabletype = $(rows[i]).find("td:eq(20)").html();
        InsertedItemsList.DeductablePerAmounttype = $(rows[i]).find("td:eq(21)").html();
        InsertedItemsList.DeductablePerAmount = $(rows[i]).find("td:eq(22)").html();
        InsertedItemsList.DiscountPerAmountType = $(rows[i]).find("td:eq(23)").html();
        InsertedItemsList.DiscountPerAmount = $(rows[i]).find("td:eq(24)").html();

        InsertedItemsList.OrderedItem = $(rows[i]).find("td:eq(25)").html();


        InsertedItemsList.OrderedItemid = $(rows[i]).find("td:eq(26)").html();
        InsertedItemsList.temp3 = $(rows[i]).find("td:eq(27)").html();
        InsertedItemsList.Name = $(rows[i]).find("td:eq(28)").html();
        InsertedItemsList.ItemCode = $(rows[i]).find("td:eq(29)").html();
        InsertedItemsList.ID = $(rows[i]).find("td:eq(30)").html();
        InsertedItemsList.MinLevel = $(rows[i]).find("td:eq(32)").html();
        
                 if (ItemSelectParam.mItemID == InsertedItemsList.ID) {
                         var CellQty = parseInt($(rows[i]).find("#ReqQty").val());
            CellQty = CellQty + 1;
            $(this).find('.edit-user').trigger('click');              $(rows[i]).find("#ReqQty").val(CellQty);
            var tr = $(rows[i]);
            var UList = tr.find(".UomAdd").find('option:selected').text();
            tr.find("#UomInputName").val(UList);
            var UListID = tr.find(".UomAdd").find('option:selected').val();
                         var lsamllQty = tr.find("td:eq(17)").text();
            var ItemID = tr.find("td:eq(30)").text();
            var BatchID = tr.find("td:eq(15)").text();
            var ConverQty = tr.find("td:eq(14)").text();
            var Qoh = tr.find("td:eq(3)").text();
            var Qoh2 = tr.find("td:eq(12)").text();
            var Price = tr.find("td:eq(5)").text();
            var InsertedQty = tr.find(".ScanReqQTY").val();
            var Amount = tr.find("td:eq(9)").text();
            var Tax = tr.find("td:eq(6)").text();
            var UnitID = UListID;
            $.ajax({
                url: GetAppName() + "/OpIssue/ConvertQtyxxxx",
                type: "POST",
                cache: false,
                dataType: 'json',
                data: { lsamllQty: lsamllQty, ItemID: ItemID, BatchID: BatchID, ConverQty: ConverQty, Qoh: Qoh, Qoh2: Qoh2, Price: Price, InsertedQty: InsertedQty, Amount: Amount, Tax: Tax, UnitID: UnitID },
                success: function (data) {
                    tr.find("#ReqQty").val(data.DispatchQty);
                    tr.find("#lblReqQty").val(data.DispatchQty);
                    tr.find("td:eq(18)").text(data.NewUomID);
                    tr.find("td:eq(9)").text(data.amount);
                    console.log(data.DispatchQty);
                },
                Failure: function (data) {
                    ShowMessage("Failure:" + data.Message);
                }
            });
            $('.save-user').trigger('click');
            $('#allItemsList').select2('val', '0');
            return false;
        }
        else {
            InsertItemValue.push(InsertedItemsList);
        }
    }
    ItemSelectParam.InsertItemValue = InsertItemValue;
    URL = "/OpIssue/InsertItem";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify(ItemSelectParam),
        success: function (msg) {
            if (msg.ErrorFlag == null || msg.ErrorFlag == false) {
                if (msg.AlertDrugInteraction == true) {
                    TempDataTbl = msg;
                    callDialog(msg.StrWhole + ", Do you Want to issue this Drug?", msg.ErrMsg, 2);

                }
                else if (msg.AlertItemIssuedAlerdy == true) {
                    TempDataTbl = msg;
                    callDialog("Item already issued to the patient against Prescription, Do you Want to issue this Drug?",
                        msg.ErrMsg, 3);
                }

                else {
                    InsertItemtoBill(msg);
                    $('.save-user').trigger('click');
                    $('#allItemsList').select2('val', '0');

                }
            } else { ShowMessage(msg.ErrMsg); }
        },
        Failure: function (msg) { ShowMessage("Failure:" + msg.Message); }
    });
    return false;
}

function callDialog(txt1, txt2, i) {
    DialCalledBy = i;       
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
    else {

        if (DialCalledBy == 2) {
            if (TempDataTbl.AlertItemIssuedAlerdy == true) {
                callDialog("Item already issued to the patient against Prescription, Do you want to Issue this Item?", TempDataTbl.ErrMsg, 3);
            }
            else {
                InsertItemtoBill(TempDataTbl);
            }
        }
        if (DialCalledBy == 3) {
            InsertItemtoBill(TempDataTbl);
        }
        if (DialCalledBy == 4) {

            if (TempDataTbl.BatchExpiryDialog == true) {
                GetDialogAnswer = false;
                callDialog(TempDataTbl.BatchExpiryDialogTXT, TempDataTbl.ErrMsg, 5);
            }
            else if (TempDataTbl.BatchExpiryDialog != true) {
                showBillNetBox(TempDataTbl);
            }
        }
        if (DialCalledBy == 6) {
            showBillNetBox(TempDataTbl);
        }

        GetDialogAnswer = false;
        return 0;
    }

}
$(document).on('click', '#dialogYes', function () {
    if (DialCalledBy == 1)      {
        var docid = $('#DoctorList').val();
        GetDialogAnswer = true;
        var txtPIN = $("#OrgnizationIssueCode").val() + $('#txtpin').val();
        URL = "/OpIssue/InsertNewLOA";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            cache: false,
            dataType: 'json',
            data: { PIN: txtPIN, DocID: docid },
            success: function (data) {
                ShowMessage(data);
            }
        });
                 $('#jDialog').dialog('close');
        GetDialogAnswer = false;
    } else if (DialCalledBy == 2)     {
                 $('#jDialog').dialog('close');
        GetDialogAnswer = true;
        callDialog("", "", 2);
    } else if (DialCalledBy == 3) {
                 $('#jDialog').dialog('close');
        GetDialogAnswer = true;
        callDialog("", "", 3);
    } else if (DialCalledBy == 4) {
        $('#jDialog').dialog('close');
        GetDialogAnswer = true;
        callDialog("", "", 4);
    }
    else if (DialCalledBy == 5) {
        $('#jDialog').dialog('close');
        GetDialogAnswer = true;
        callDialog("", "", 5);
        showBillNetBox(TempDataTbl);
    }
    else if (DialCalledBy == 6) {
        $('#jDialog').dialog('close');
        GetDialogAnswer = true;
        callDialog("", "", 6);
        showBillNetBox(TempDataTbl);
    }
    return false;
});
$(document).on('click', '#dialogNo', function () {
    GetDialogAnswer = false;
         $('#jDialog').dialog('close');

    if (DialCalledBy == 2 || DialCalledBy == 3 || DialCalledBy == 4 || DialCalledBy == 5 || DialCalledBy == 6) {
                 TempDataTbl = null;
        GetDialogAnswer = false;
                 $('#jDialog').dialog('close');
    }

});

function InsertItemtoBill(itm) {
    var InsertItemValue = [];
    InsertItemValue = itm.InsertItemValue;
    render_InsertSelectedItem('#SelectedItemtbl', InsertItemValue);
    TempDataTbl = null;      $('#Save').focus();  
    $('.edit-user').trigger('click');  
    return false;
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
            {
                data: 'DrugName', targets: [1], width: '40%',
                'mRender': function (data, type, full) {
                    return '<input type="text" id="DrugName" value="' + data +
                        '" class=" display-mode disable" />' +
                        '<select id="GenericList" class="GenericAdd select edit-mode hidden" />';
                }

            },
            { data: 'BatchNo', targets: [2], className: ' hidden' },
            { data: 'qoh', targets: [3], width: '5%' },

            { data: 'Highunit', targets: [4], className: '', width: '12%', },
            { data: 'price', targets: [5], className: 'Align_Right_Col', width: '7%', },
            { data: 'tax', targets: [6], className: 'Align_Right_Col', width: '2%', },
            {
                data: 'qty', targets: [7], className: 'Align_Right_Col_Red', width: '7%',
                'mRender': function (data, type, full) {
                    return '<span class="display-mode">' +
                    '<input type="text" id="lblReqQty" value="' + data + '" class="display-mode disable" style="Align_Right_Col" />' +
                    '</span>' +
                    '<input type="text" id="ReqQty" value="' + data + '" class="edit-mode hidden AutoSelectTxt ScanReqQTY" />';
                }
            },

            {
                data: 'NewUomName', targets: [8], className: '', width: '15%',
                'mRender': function (data, type, full) {
                    return '<input type="text" id="UomInputName" value="' + data +
                        '" class=" display-mode disable" />' +
                        '<select id="UomList" class="UomAdd select edit-mode hidden" />';
                }
            },
            {
                data: 'amount', targets: [9], className: 'Align_Right_Col_Red .NumRoundTwo', width: '10%',
            },
            { data: 'NewUomName', targets: [10], className: ' hidden' },
            { data: 'DispatchQty', targets: [11], className: ' hidden' },

            { data: 'qoh2', targets: [12], className: ' hidden' },
            { data: 'Drugtype', targets: [13], className: ' hidden' },
            { data: 'conversionqty', targets: [14], className: ' hidden' },
            { data: 'batchid', targets: [15], className: ' hidden' },

            { data: 'purqty', targets: [16], className: ' hidden' },
            { data: 'lsmallqty', targets: [17], className: ' hidden' },
            { data: 'NewUomID', targets: [18], className: ' hidden' },
            { data: 'PrescriptionID', targets: [19], className: ' hidden' },

            { data: 'Deductabletype', targets: [20], className: ' hidden' },
            { data: 'DeductablePerAmounttype', targets: [21], className: ' hidden' },
            { data: 'DeductablePerAmount', targets: [22], className: ' hidden' },
            { data: 'DiscountPerAmountType', targets: [23], className: ' hidden' },

            { data: 'DiscountPerAmount', targets: [24], className: ' hidden' },
            {
                data: 'OrderedItem', targets: [25], className: ' hidden'

            },
            { data: 'OrderedItemid', targets: [26], className: ' hidden' },
            { data: 'temp3', targets: [27], className: ' hidden' },

            { data: 'Name', targets: [28], className: ' hidden ' },
            { data: 'ItemCode', targets: [29], className: ' hidden' },
            { data: 'ID', targets: [30], className: ' hidden' },
            {
                data: 'Action', targets: [31], className: 'TAC',
                "mRender": function (data, type, full) {
                    return '<a href="#"><span class="icon-ok save-user edit-mode hidden"></span></a> ' +
                    '<a href="#"><span class="icon-pencil edit-user display-mode"></span></a> ' +
                    '<a href="#"><span class="icon-trash cancel-user edit-mode hidden"></span></a>';
                }
            },
            { data: 'MinLevel', targets: [32], className: ' hidden' },

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

         var ItemID = $(this).closest("tr").find('td:eq(30)').text();
    var CurrentUOM = $(this).closest("tr").find('td:eq(18)').text();


    
    


    
    
    return false;
});
$(document).on('click', '.save-user', function (e) {
    var tr = $(this).parents('tr:first');
    var ReqQty = tr.find("#ReqQty").val();
    if (ReqQty.length > 0) { tr.find("#lblReqQty").val(ReqQty); }

    $(".edit-mode").addClass("hidden");
    $(".display-mode").removeClass("hidden");



    e.preventDefault();

         $('#PatientName').focus();
    $('.select2-choice').focus();
    $('#allItemsList').select2("open");
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

$(document).on("change", ".UomAdd", function (e) {
    ChangeInfo(this);
});
$(document).on("change", ".GenericAdd", function (e) {
    var tr = $(this).parents('tr:first');
    var GenericID = tr.find(".GenericAdd").find('option:selected').val();
    var GenericName = tr.find(".GenericAdd").find('option:selected').text();
    var OrderItemID = tr.find("td:eq(26)").text();
    var OrderItemName = tr.find("td:eq(25)").text();
    tr.find("#DrugName").val(GenericName);
    ItemSelect(MprescriptionID, GenericID, GenericName, OrderItemID, OrderItemName);


});
$(document).on("change", "#ReqQty", function (e) {
    ChangeInfo(this);
});
function ChangeInfo(obj) {

    var tr = $(obj).parents('tr:first');
    var UList = tr.find(".UomAdd").find('option:selected').text();
    tr.find("#UomInputName").val(UList);

    var UListID = tr.find(".UomAdd").find('option:selected').val();

         var tr = $(obj).parents('tr:first');
    var lsamllQty = tr.find("td:eq(17)").text();
    var ItemID = tr.find("td:eq(30)").text();
    var BatchID = tr.find("td:eq(15)").text();
    var ConverQty = tr.find("td:eq(14)").text();
    var Qoh = tr.find("td:eq(3)").text();
    var Qoh2 = tr.find("td:eq(12)").text();
    var Price = tr.find("td:eq(5)").text();
    var InsertedQty = tr.find("#ReqQty").val();
    console.log("reqQty: " + InsertedQty);
    var Amount = tr.find("td:eq(9)").text();
    var Tax = tr.find("td:eq(6)").text();
    var UnitID = UListID;
    $.ajax({
        url: GetAppName() + "/OpIssue/ConvertQtyxxxx",
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { lsamllQty: lsamllQty, ItemID: ItemID, BatchID: BatchID, ConverQty: ConverQty, Qoh: Qoh, Qoh2: Qoh2, Price: Price, InsertedQty: InsertedQty, Amount: Amount, Tax: Tax, UnitID: UnitID },
        success: function (data) {
            tr.find("#ReqQty").val(data.DispatchQty);
            tr.find("td:eq(18)").text(data.NewUomID);
            tr.find("td:eq(9)").text(data.amount);

        },
        Failure: function (data) {
            ShowMessage("Failure:" + data.Message);
        }
    });
    return false;

}

$(document).on('change', '#allItemsList', function (e) {
    var data = $('select#allItemsList option:selected').val();

     

    CallBatchView(data);

    return false;
});
function CallBatchView(itemid) {
    URL = "/Item/ItemLookupBatch";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { ItemID: itemid, Cond: " and b.quantity>0 " },
        success: function (data) {
            $('#lblQTY').text(parseFloat(data.QOH).toFixed(2));
            $('#lblROL').text(parseFloat(data.ROL).toFixed(2));
            $('#lblROQ').text(parseFloat(data.ROQ).toFixed(2));
            $('#lblMinlvl').text(parseFloat(data.MinLevel).toFixed(2));
            $('#lblMaxlvl').text(parseFloat(data.MaxLevel).toFixed(2));
            $('#lblUOM').text(data.UOM);
            $('#lbllocation').text(data.location);
            var Btl = data.BatchInfo;
            render_BatchTable('#tblBatch', Btl);

        }
    });
    $("#btnAddItem").trigger('click');
    return false;
}
function render_BatchTable(tableID, dataxc) {
    $(tableID).dataTable().fnClearTable();
    $(tableID).dataTable().fnDestroy();
    for (var i = 0; i < dataxc.length; i++) {
        $(tableID).append('<tr class="odd">' +
                 '<td class=" "> ' + dataxc[i].slno + '</td>' +
                 '<td class=" "> ' + dataxc[i].BatchNo + '</td>' +
                 '<td class=" "> ' + dataxc[i].Quantity + '</td>' +
                 '<td class=" "> ' + dataxc[i].ExpiryDate + '</td>' +
                 '<td class=" "> ' + parseFloat(dataxc[i].SellingPrice).toFixed(2) + '</td>' +
                 '</tr>'
         );
    }



    MyTbl2 = $(tableID).DataTable();



};

function GetPatient(DocID) {
         var FireAlertMessage = false;
    var txtPIN = $("#OrgnizationIssueCode").val() + $('#txtpin').val();
    URL = "/OpIssue/GetPatient";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { PIN: txtPIN, DocID: DocID },
        success: function (data) {
            $('.ControlDivCls').show();              if (data.PatientNotFound == false) {
                $('#Registrationno').val(data.Registrationno);
                $('#OperatorName').val(data.OperatorName);
                $('#PatientName').val(data.PatientName);
                $('#SexTitle').val(data.SexTitle);
                $('#Sex').val(data.Sex);
                $('#Age').val(data.Age);
                $('#AgeTitle').val(data.AgeTitle);
                $('#Agetype').val(data.Agetype);
                $('#CompanyId').val(data.CompanyId);
                $('#CategoryId').val(data.CategoryId);
                $("#GradeId").val(data.GradeId);
                $('#mDeptId').val(data.mDeptId);
                $('#mAuthorityid').val(data.mAuthorityid);
                $('#mLOAConsultation').val(data.mLOAConsultation);
                $('#IssueAuthorityCode').val(data.IssueAuthorityCode);

                $('#Vip').val(data.Vip);
                $('#CheckEmpPin').val(data.CheckEmpPin);
                if (data.Vip == true)
                { $('.VIPCLASS').show(); } else { $('.VIPCLASS').hide(); }

                MsgTbl = $('#AlertTbl').DataTable();

                $.each(data.PHalerts, function (i, Item) {
                    FireAlertMessage = true;
                    MsgTbl.row.add([
                        Item.Slno,
                        Item.Message,
                        Item.Operator,
                        Item.Station
                    ]).draw();

                });

                $("#DoctorList option[value='" + data.DoctorId + "']").attr('selected', 'selected');
                $("#DoctorList").trigger('change');
                $('#DocIdHolder').val(data.DoctorId);
                LoadPrescription(data.DoctorId);

                $("#DiscountList option[value='" + data.DiscountTypeID + "']").attr('selected', 'selected');
                $("#DiscountList").trigger('change');
                $("#DisReason").val(data.DisReason);
                $("#disAuthoriseID").val(data.disAuthoriseID);

                
                                 $("#CategoryName").val(data.CategoryName);
                $("#CategoryName2").val(data.CategoryName);

                                 $("#CompanyName").val(data.CompanyName);
                $("#CompanyName2").val(data.CompanyName);


                
                $("#lblDeductableText").val(data.lblDeductableText);
                $("#lblLoaBal").val(parseFloat(data.lblLoaBal).toFixed(2));
                $("#lbldate").val(data.lbldate);
                $("#lblgrade").val(data.lblgrade);
                $("#TxtLOAletterno").val(data.TxtLOAletterno);
                $("#lblIdExpiry").val(data.lblIdExpiry);
                $("#TxtInsCardno").val(data.TxtInsCardno);
                $("#lblLOAAmt").val(parseFloat(data.lblLOAAmt).toFixed(2));
                $("#lblLoaDays").val(data.lblLoaDays);
                $("#TxtCompanyRemarks").html("<p>" + data.TxtCompanyRemarks + "</p>");
                $("#BillisCredit").val(data.BillisCredit);

                
                var TempAllgeryItemID = 0;
                $.each(data.Allergey, function (i, Item) {
                    $('#DrugAllergyList').append(new Option(Item.Name, Item.ID));
                    TempAllgeryItemID = Item.ID;
                });
                $('#DrugAllergyList option[value="' + TempAllgeryItemID + '"]').attr("selected", 'selected');
                $("#DrugAllergyList").trigger('change');

                
                TempAllgeryItemID = 0;
                $.each(data.OtherAllergey, function (i, Item) {
                    $('#OtherAllergyList').append(new Option(Item.Name, Item.ID));
                    TempAllgeryItemID = Item.ID;
                });
                $('#OtherAllergyList option[value="' + TempAllgeryItemID + '"]').attr("selected", 'selected');
                $("#OtherAllergyList").trigger('change');
                TempAllgeryItemID = 0; 

                
                if (data.mAuthorityid == 0 && data.DoctorId > 0) {
                    if (data.ErrMsg != null) {
                        callDialog("No LOA For this Doctor,Do you want to make an Approval ?", data.ErrMsg, 1);
                    } else { callDialog("No LOA For this Doctor,Do you want to make an Approval ?", null, 1); }
                                                                                                                                                                                                                                  }

                if (data.BillisCredit == false) {
                    $("#cashbtn").trigger('click');
                } else {
                    $("#creditbtn").trigger('click');
                                     }

                if (data.ErrMsg != null && data.mAuthorityid != 0) {
                    ShowMessage(data.ErrMsg);
                    if (FireAlertMessage == true) {
                        notify("ALERT", "Please Check Alert Section");
                        $("#HeaderAlert").trigger("click");
                    }
                }
            }
            else {
                if (data.BillisCredit == false) {
                    $("#cashbtn").trigger('click');
                                         $("#creditbtn").attr("disabled", "disabled");
                } else {
                    $("#creditbtn").trigger('click');
                    $("#cashbtn").attr("disabled", "disabled");
                }
                if (FireAlertMessage == true) {
                    notify("ALERT", "Please Check Alert Section");
                    $("#HeaderAlert").trigger("click");
                }
                ShowMessage(data.ErrMsg);
                window.location.replace(GetAppName() + "/OpIssue?Msg=" + data.ErrMsg);
            }

        },
        Failure: function (data) {
            ShowMessage("Failure:" + data.Message);
                     }
    });


    return false;


};
function GetCOMPANY(DocID) {
    var txtPIN = $("#OrgnizationIssueCode").val() + $('#txtpin').val();
    URL = "/OpIssue/GetPatient";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { PIN: txtPIN, DocID: DocID },
        success: function (data) {
            if (data.PatientNotFound == false) {


                $("#DoctorList option[value='" + data.DoctorId + "']").attr('selected', 'selected');
                $("#DoctorList").trigger('change');
                $('#DocIdHolder').val(data.DoctorId);


                $("#DiscountList option[value='" + data.DiscountTypeID + "']").attr('selected', 'selected');
                $("#DiscountList").trigger('change');
                $("#DisReason").val(data.DisReason);
                $("#disAuthoriseID").val(data.disAuthoriseID);

                
                                 $("#CategoryId").val(data.CategoryId);
                $("#CategoryName").val(data.CategoryName);
                $("#CategoryName2").val(data.CategoryName);

                                 $("#CompanyId").val(data.CompanyId);
                $("#CompanyName").val(data.CompanyName);
                $("#CompanyName2").val(data.CompanyName);

                
                $("#lblDeductableText").val(data.lblDeductableText);
                $("#lblLoaBal").val(data.lblLoaBal);
                $("#lbldate").val(data.lbldate);
                $("#lblgrade").val(data.lblgrade);
                $("#GradeId").val(data.GradeId);
                $("#TxtLOAletterno").val(data.TxtLOAletterno);
                $("#lblIdExpiry").val(data.lblIdExpiry);
                $("#TxtInsCardno").val(data.TxtInsCardno);
                $("#lblLOAAmt").val(data.lblLOAAmt);
                $("#lblLoaDays").val(data.lblLoaDays);
                $("#TxtCompanyRemarks").html("<p>" + data.TxtCompanyRemarks + "</p>");

                $("#BillisCredit").val(data.BillisCredit);





                
                if (data.mAuthorityid == 0 && data.DoctorId > 0) {
                    $('#dialogMSG').text("No LOA For this Doctor,Do you want to make an Approval ?");
                                         $('#jDialog').dialog('open');
                    $('.ui-dialog-buttonpane').hide();
                    $('.dialogOtherAlert').hide();
                    if (data.ErrMsg != null) {
                        $('#dialogAlert').text(data.ErrMsg);
                                                 $('#jDialog').dialog('open');
                        $('.ui-dialog-buttonpane').hide();
                        $('.dialogOtherAlert').show();
                    }
                }

                if (data.BillisCredit == false) {
                    $("#cashbtn").trigger('click');
                } else {
                    $("#creditbtn").trigger('click');
                }

                if (data.ErrMsg != null && data.mAuthorityid != 0) {
                    ShowMessage(data.ErrMsg);
                }
            } else {
                if (data.BillisCredit == false) {
                    $("#cashbtn").trigger('click');
                                         $("#creditbtn").attr("disabled", "disabled");
                } else {
                    $("#creditbtn").trigger('click');
                    $("#cashbtn").attr("disabled", "disabled");
                }

                ShowMessage(data.ErrMsg);
            }

        },
        Failure: function (data) {
            ShowMessage("Failure:" + data.Message);
        }
    });
    return false;


};

function LoadPrescription(DocID) {
    URL = "/OpIssue/LoadPrescription";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { mRegNo: $('#txtpin').val(), DocID: DocID },
        success: function (data) {
            render_Prescription('#tblPrescription', data);

        }
    });
    return false;
};
function render_Prescription(tableID, dataxc) {


    PresTbl = $(tableID).dataTable({
        destroy: true,
        data: dataxc,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
            {
                data: 'OrderNo', className: '', targets: [0],
                "mRender": function (data, type, full) {
                                                              return '<a href="#" class="PresDtlClass">' + data + '</a>';
                }


            },
            { data: 'DateTime', targets: [1], className: 'Align_Left' },              { data: 'Operator', className: 'cAR-align-center', targets: [2] },              { data: 'Station', className: 'Align_Rigt Date', targets: [3] },              { data: 'DoctorID', targets: [4], className: 'hidden' },             { data: 'Status', className: 'cAR-align-center hidden', targets: [5] },
            { data: 'VisitId', targets: [6], className: 'Align_Left hidden' },          { data: 'MPrescriptionF', targets: [7], className: 'Align_Left hidden' } 

        ]
    }).fnDraw();


    PresTbl = $(tableID).DataTable();

};
function DisplayPrescriptionDtl(OrderNo, Ddate, RegNumber, docid, MPrescriptionF) {
    URL = "/OpIssue/DisplayPrescription";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: {
            OrderNo: OrderNo, Ddate: Ddate, RegNumber: RegNumber,
            docid: docid, MPrescriptionF: MPrescriptionF
        },
        success: function (data) {
            render_PrescriptionDTL('#tblPrescriptionDtl', data);
            MprescriptionID = parseInt(data[0].OrderNo);
            FromPresToBill(MprescriptionID);
            return false;

        }
    });
    return false;
}
function render_PrescriptionDTL(tableID, dataxc) {
    PresDtlTbl = $(tableID).dataTable({
        destroy: true,
        data: dataxc,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
            { data: 'drugid', className: 'cAR-align-center hidden', targets: [0] },
            {
                data: 'Name', targets: [1], className: 'Align_Left', "mRender": function (data, type, full) {
                                                              return '<a href="#" class="SelectPresDtlClass">' + data + '</a>';
                }
            },              { data: 'strength', className: 'cAR-align-center', targets: [2], width: '10px' },              { data: 'RouteofAdmin', className: 'Align_Rigt Date', targets: [3] },              { data: 'frequency', targets: [4], },             { data: 'issquantity', targets: [5], },             { data: 'Duration', className: 'cAR-align-center ', targets: [6] },
            { data: 'OrderNo', targets: [7], className: 'Align_Left ' },          { data: 'lquantity', targets: [8], className: 'Align_Left ' },          { data: 'GenericName', targets: [9], className: 'Align_Left ' },         { data: 'DateTime', targets: [10], className: 'Align_Left hidden ' }


        ]
    }).fnDraw();


    PresDtlTbl = $(tableID).DataTable();
};
function FromPresToBill(MprescriptionID) {
    var Presrows = $('#tblPrescriptionDtl').dataTable().fnGetNodes();
    var PresDrugList = [];
    for (var j = 0; j < Presrows.length; j++) {
        
        var ItemSelectParam = {};
        ItemSelectParam.mItemID = $(Presrows[j]).find("td:eq(0)").html();
        ItemSelectParam.mItemName = $(Presrows[j]).find("td:eq(1)").html();
        ItemSelectParam.mDeptid = $('#mDeptId').val();
        ItemSelectParam.mAuthorityid = $('#mAuthorityid').val();
        ItemSelectParam.cmbCompany = $('#CompanyId').val();
        ItemSelectParam.mCategoryid = $('#CategoryId').val();
        ItemSelectParam.mGradeId = $('#GradeId').val();
        ItemSelectParam.RegNo = $('#txtpin').val();
        ItemSelectParam.gIACode = $('#IssueAuthorityCode').val();
        ItemSelectParam.optCrPat = $('#BillisCredit').val();
        ItemSelectParam.mPrescription = MprescriptionID;

                 var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();
        var InsertItemValue = [];          for (var i = 0; i < rows.length; i++) {
            var InsertedItemsList = {};              InsertedItemsList.SNO = $(rows[i]).find("td:eq(0)").html();
                         InsertedItemsList.DrugName = $(rows[i]).find("#DrugName").val();
            InsertedItemsList.BatchNo = $(rows[i]).find("td:eq(2)").html();
            InsertedItemsList.qoh = $(rows[i]).find("td:eq(3)").html();
            InsertedItemsList.Highunit = $(rows[i]).find("td:eq(4)").html();
            InsertedItemsList.price = $(rows[i]).find("td:eq(5)").html();
            InsertedItemsList.tax = $(rows[i]).find("td:eq(6)").html();
                         InsertedItemsList.qty = $(rows[i]).find("#ReqQty").val();
            InsertedItemsList.UnitList = $(rows[i]).find("td:eq(8)").html();
            InsertedItemsList.amount = $(rows[i]).find("td:eq(9)").html();
                         InsertedItemsList.NewUomName = $(rows[i]).find("#UomInputName").val();
            InsertedItemsList.DispatchQty = $(rows[i]).find("td:eq(11)").val();


            InsertedItemsList.qoh2 = $(rows[i]).find("td:eq(12)").html();
            InsertedItemsList.Drugtype = $(rows[i]).find("td:eq(13)").html();
            InsertedItemsList.conversionqty = $(rows[i]).find("td:eq(14)").html();
            InsertedItemsList.batchid = $(rows[i]).find("td:eq(15)").html();
            InsertedItemsList.purqty = $(rows[i]).find("td:eq(16)").html();
            InsertedItemsList.lsmallqty = $(rows[i]).find("td:eq(17)").html();

            InsertedItemsList.NewUOMID = $(rows[i]).find("td:eq(18)").html();

            InsertedItemsList.PrescriptionID = $(rows[i]).find("td:eq(19)").html();
            InsertedItemsList.Deductabletype = $(rows[i]).find("td:eq(20)").html();
            InsertedItemsList.DeductablePerAmounttype = $(rows[i]).find("td:eq(21)").html();
            InsertedItemsList.DeductablePerAmount = $(rows[i]).find("td:eq(22)").html();
            InsertedItemsList.DiscountPerAmountType = $(rows[i]).find("td:eq(23)").html();
            InsertedItemsList.DiscountPerAmount = $(rows[i]).find("td:eq(24)").html();


            InsertedItemsList.OrderedItem = $(rows[i]).find("td:eq(25)").html();


            InsertedItemsList.OrderedItemid = $(rows[i]).find("td:eq(26)").html();
            InsertedItemsList.temp3 = $(rows[i]).find("td:eq(27)").html();
            InsertedItemsList.Name = $(rows[i]).find("td:eq(28)").html();
            InsertedItemsList.ItemCode = $(rows[i]).find("td:eq(29)").html();
            InsertedItemsList.ID = $(rows[i]).find("td:eq(30)").html();

            
                         if (ItemSelectParam.mItemID == InsertedItemsList.ID) {
                ShowMessage('Item Already Selected !'); return false;
            }
            else {
                InsertItemValue.push(InsertedItemsList);
            }
        }
        ItemSelectParam.InsertItemValue = InsertItemValue;
        
        PresDrugList.push(ItemSelectParam);
    }




    URL = "/OpIssue/ListInsertItem";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        dataType: 'json',
        data: JSON.stringify(PresDrugList),
        success: function (msg) {
            console.log(msg);
            $('#txtErrorPrescription').val('');             var GetMSG = msg[0].ErrMsg;
            var len = msg[0].ErrMsg.length;
            if (len > 1 || GetMSG != '')
            { $('#txtErrorPrescription').html(GetMSG); }             render_InsertSelectedItem('#SelectedItemtbl', msg);
            findItemWithError();
            return false;
        },
        Failure: function (msg) { ShowMessage("Failure:" + msg.Message); return false; }
    });
    return false;
}
function findItemWithError() {
    var found = false;
    var Presrows = $('#tblPrescriptionDtl').dataTable().fnGetNodes();
    for (var i = 0; i < Presrows.length; i++) {
        var Billrows = $('#SelectedItemtbl').dataTable().fnGetNodes();
        for (var j = 0; j < Billrows.length; j++) {

            if ($(Billrows[j]).find("td:eq(30)").html() ==
                $(Presrows[i]).find("td:eq(0)").html()) {
                found = true;
            }
        }

        if (found == false) {
                         $(Presrows[i]).children('td,th').css('background-color', 'lightpink');
        }
        found = false;  
    }



}

function DisplayDiagnosis(pinno, orderId) {

    URL = "/OpIssue/DisplayDiagnosis";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: {
            pinno: pinno, orderId: orderId
        },
        success: function (data) {
            render_Diagnosis('#DignosisTbl', data);

        }
    });
    return false;
}
function render_Diagnosis(tableID, dataxc) {
    DiagnosisTbl = $(tableID).dataTable({
        destroy: true,
        data: dataxc,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
            { data: 'VisitId', className: 'cAR-align-center', targets: [0], width: '20%' },
            { data: 'ICDCode', targets: [1], className: 'Align_Left', width: '30%' },
            { data: 'Description', className: 'cAR-align-center', targets: [2], width: '50%' }
        ]
    }).fnDraw();


    DiagnosisTbl = $(tableID).DataTable();

}

function Bill(ActionType) {
         var Patient = {};
    var lblNetAmount = 0.0;
         var rows = $('#SelectedItemtbl').dataTable().fnGetNodes();

    if (rows.length > 0) {
        var InsertItemValue = [];          for (var i = 0; i < rows.length; i++) {
            var Price = $(rows[i]).find("td:eq(5)").html();
                         if (Price <= 0)
            { return false; ShowMessage("Price Not Defined for the selected item(s)."); }


            
            var qty = $(rows[i]).find("#ReqQty").val();
            if (qty <= 0)
            { ShowMessage("Enter the Quantity for the selected items."); return false; }


            var InsertedItemsList = {};              InsertedItemsList.SNO = $(rows[i]).find("td:eq(0)").html();
                         InsertedItemsList.DrugName = $(rows[i]).find("#DrugName").val();
            InsertedItemsList.BatchNo = $(rows[i]).find("td:eq(2)").html();
            InsertedItemsList.qoh = $(rows[i]).find("td:eq(3)").html();
            InsertedItemsList.Highunit = $(rows[i]).find("td:eq(4)").html();
            InsertedItemsList.price = $(rows[i]).find("td:eq(5)").html();
            InsertedItemsList.tax = $(rows[i]).find("td:eq(6)").html();
                         InsertedItemsList.qty = $(rows[i]).find("#ReqQty").val();
            InsertedItemsList.UnitList = $(rows[i]).find("td:eq(8)").html();
            InsertedItemsList.amount = $(rows[i]).find("td:eq(9)").html();
            
            lblNetAmount = parseFloat(lblNetAmount) + parseFloat(InsertedItemsList.amount);

                         InsertedItemsList.NewUomName = $(rows[i]).find("#UomInputName").val();
            InsertedItemsList.DispatchQty = $(rows[i]).find("td:eq(11)").val();


            InsertedItemsList.qoh2 = $(rows[i]).find("td:eq(12)").html();
            InsertedItemsList.Drugtype = $(rows[i]).find("td:eq(13)").html();
            InsertedItemsList.conversionqty = $(rows[i]).find("td:eq(14)").html();
            InsertedItemsList.batchid = $(rows[i]).find("td:eq(15)").html();
            InsertedItemsList.purqty = $(rows[i]).find("td:eq(16)").html();
            InsertedItemsList.lsmallqty = $(rows[i]).find("td:eq(17)").html();

            InsertedItemsList.NewUOMID = $(rows[i]).find("td:eq(18)").html();

            var mPresid = parseInt($(rows[i]).find("td:eq(19)").html());
            if (mPresid > 0) { Patient.mPresid = parseInt(mPresid); }

            InsertedItemsList.PrescriptionID = $(rows[i]).find("td:eq(19)").html();
            InsertedItemsList.Deductabletype = $(rows[i]).find("td:eq(20)").html();
            InsertedItemsList.DeductablePerAmounttype = $(rows[i]).find("td:eq(21)").html();
            InsertedItemsList.DeductablePerAmount = $(rows[i]).find("td:eq(22)").html();
            InsertedItemsList.DiscountPerAmountType = $(rows[i]).find("td:eq(23)").html();
            InsertedItemsList.DiscountPerAmount = $(rows[i]).find("td:eq(24)").html();

            InsertedItemsList.OrderedItem = $(rows[i]).find("td:eq(25)").html();
            InsertedItemsList.OrderedItemid = $(rows[i]).find("td:eq(26)").html();
            InsertedItemsList.temp3 = $(rows[i]).find("td:eq(27)").html();
            InsertedItemsList.Name = $(rows[i]).find("td:eq(28)").html();
            InsertedItemsList.ItemCode = $(rows[i]).find("td:eq(29)").html();
            InsertedItemsList.ID = $(rows[i]).find("td:eq(30)").html();
            InsertedItemsList.MinLevel = $(rows[i]).find("td:eq(32)").html();
            InsertItemValue.push(InsertedItemsList);
        }
        Patient.InsertItemValue = InsertItemValue;
    } else {
        ShowMessage('Please select at least one Drug.');
        return 0;
    }

    var CbtnStatus = $('#cashbtn').attr('checked');
    if (CbtnStatus == 'checked') {
        Patient.BillisCredit = false;
    }
    else {
        Patient.BillisCredit = true;

        var Catid = $("#CategoryId").val();
        if (parseInt(Catid) <= 0) {
            ShowMessage('Category or Company information is not available.');
            return false;
        }
    }

    
    var docid = $('#DoctorList').val();
    var desDocid = $('#DisDoctorList').val();
    if (docid <= 0 && desDocid <= 0) {
        ShowMessage("Please select prescribed doctor and dispatching doctor.");
        return 0;
    } else if (docid <= 0 && desDocid > 0) {
        ShowMessage("Please select prescribed doctor.");
        return 0;
    } else if (docid > 0 && desDocid <= 0) {
        ShowMessage("Please select dispatching doctor.");
        return 0;
    }
    
    var lblNetAmount2 = parseFloat(lblNetAmount);
    var lblBalDeposit2 = parseFloat($('#txtDepositAmt').val());

    if (lblBalDeposit2 > 0 && lblNetAmount2 > lblBalDeposit2) {
        ShowMessage("Bill Amount should be Less then Deposit Amount.");
        return 0;
    } else if (lblNetAmount2 == 0) {
        ShowMessage("Bill Amount should Not be Zero.");
        return 0;
    }


    Patient.mLOAConsultation = $('#mLOAConsultation').val();
    Patient.CompanyId = $('#CompanyId').val();
    Patient.CategoryId = $("#CategoryId").val();
    Patient.GradeId = $("#GradeId").val();
    Patient.mDeptId = $("#mDeptId").val();
    Patient.Registrationno = $('#Registrationno').val();
    Patient.IssueAuthorityCode = $('#IssueAuthorityCode').val();
    Patient.mAuthorityid = $('#mAuthorityid').val();
    Patient.DoctorId = $('#DocIdHolder').val();
    Patient.DispatchDoctorID = $('#DisDoctorList').val();
    Patient.lblNetAmount = parseFloat(lblNetAmount);
    Patient.lblBalDeposit = parseFloat($('#txtDepositAmt').val());
    Patient.mdisper = parseFloat($('#mdisper').val());

    if (ActionType == 'View') {
        URL = "/OpIssue/Bill";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: JSON.stringify(Patient),
            success: function (msg) {
                TempDataTbl = msg;
                if (msg.PrescriptionDialog == true) {
                    callDialog(msg.PrescriptionDialogTxt + " Do you want to continue?", msg.ErrMsg, 4);
                }
                else if (msg.BatchExpiryDialog == true)
                { callDialog(msg.BatchExpiryDialogTXT, msg.ErrMsg, 5); }
                else if (msg.QOHMinLevelFlag == true) {
                    callDialog(
                        "QOH will be reduced by 25 % of Minlevel for the following item(s).("
                    + msg.ErrMsg + ") Do you want to continue?"
                        , null, 6);
                }
                else if (msg.ErrMsg != null && msg.ErrMsg != '') {
                    ShowMessage(msg.ErrMsg);
                } else {
                    showBillNetBox(msg);
                }

            }
        });
    } else if (ActionType == 'Save') {

        Patient.lblCreditBillAmount = $('#lblCreditBillAmount').text();
        Patient.lblCreditDiscount = $('#lblCreditDiscount').text();
        Patient.lblNetAmount = $('#lblNetAmount').text();
        Patient.lbldedamt = $('#lbldedamt').text();
        Patient.txtAmountToBeCollected = $('#txtAmountToBeCollected').text();
        Patient.lblBalance = $('#lblBalance').text();
        Patient.DepositChkNo = $('#DepositChkNo').val();
                 Patient.mDeductper = $('#mDeductper').text();




        var DonateStatus = $('#chkDonate').attr('checked');
        if (DonateStatus == 'checked') {
            Patient.lbldonationAMT = $('#lbldonationAMT').text();

        } else {
            Patient.lbldonationAMT = 0.0;
        }
        
        Patient.PatientName = $('#PatientName').val();
        Patient.DoctorName = $('select#DoctorList option:selected').text();
        Patient.AgeTitle = $('#AgeTitle').val();
        Patient.Agetype = $('#Agetype').val();
        Patient.Age = $('#Age').val();
        Patient.Sex = $('#Sex').val();

        URL = "/OpIssue/SaveBill";
        $.ajax({
            url: GetAppName() + URL,
            type: "POST",
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: JSON.stringify(Patient),
            success: function (msg) {
                if (msg.ErrMsg != null && msg.ErrMsg != '') {
                    ShowMessage(msg.ErrMsg);
                } else {
                                         $('#ViewDialog').dialog('close');
                    window.location.replace(GetAppName() + "/OpIssue/Index?Msg=" + msg.sPrefix);


                    
                     

                }
            }
        });
    }
    return false;
}
function showBillNetBox(msg) {
    $('#txtTotalAmount').text(Num(msg.TotalAmount, 2, 1));
    $('#lblCreditBillAmount').text(Num(msg.lblCreditBillAmount, 2, 1));
    $('#lblCreditDiscount').text(Num(msg.lblCreditDiscount, 2, 1));
    $('#lblNetAmount').text(Num(msg.lblNetAmount, 2, 1));
    $('#lbldedamt').text(Num(msg.lbldedamt, 2, 1));
    $('#txtAmountToBeCollected').text(Num(msg.txtAmountToBeCollected, 2, 1));
    $('#lblBalance').text(Num(msg.lblBalance, 2, 1));
    $('#lbldonationAMT').text(Num(msg.lbldonationAMT, 2, 1));
    $('#mDeductper').text(Num(msg.mDeductper, 2, 1));

    
    $('#mdisper').val(msg.mdisper);

    $('#ViewDialog').dialog('open');
    $('.ui-dialog-buttonpane').hide();

}
$(document).on('change', '#chkDonate', function (e) {
    var totamt = 0.0;
    totamt = +($('#txtAmountToBeCollected').text());
    var donamt = 0.0;
    donamt = +($('#lbldonationAMT').text());

    var DonateStatus = $('#chkDonate').attr('checked');
    if (donamt > 0) {
        if (DonateStatus == 'checked') {
            totamt = totamt + donamt;
            $('#txtAmountToBeCollected').text(totamt);
        } else {
            totamt = totamt - donamt;
            $('#txtAmountToBeCollected').text(totamt);
        }
    }
    return false;
});

$(document).on('click', '#BilldialogYes', function (e) {
    Bill("Save");
});
$(document).on('click', '#BilldialogNo', function (e) {
    $('#ViewDialog').dialog('close');
});

$(document).on('keydown', '.UomAdd', function (event) {
    if (event.which == 37) {          $('#ReqQty').focus().select();
    }
    if (event.which == 39 || event.which == 13) {          $('.save-user').trigger('click');

    }
});

 $(document).on('keydown', '#ReqQty', function (event) {
    if (event.which == 39 || event.which == 13) {          $('.UomAdd').focus().select();
    }
               });


$(document).on("click", "#OpenPrescriptionHeader", function (e) {
    $('#ModelPresOrderHeader').dialog('open');
});
$(document).on("click", "#OpenDiagnosis", function (e) {
    $('#ModelDiagnosis').dialog('open');
});
$(document).on("click", "#OpenPHALERT", function (e) {
    $('#ModelAlert').dialog('open');
});
$(document).on("click", "#OpenCreditDetail", function (e) {
    $('#ModelCompanyDtl').dialog('open');
});

$(document).on("click", "#btnPrescription", function (e) {
    $('#ModelViewOrder').dialog('open');
    $('#ModelPresOrderHeader').dialog('close');
});
$(document).on("click", "#btnViewBatchDetail", function (e) {
    $('#ModelBatchDtl').dialog('open');
});


    
$(document).on('change', '#ScannerList', function () {
    var Rst = $("#ScannerList").val();
    ScannerBool = Rst;
    URL = "/OpIssue/ScannerOnOFF";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: {
            reslt: Rst
        },
        success: function (data) {
            return false;

        }
    });
    return false;

});
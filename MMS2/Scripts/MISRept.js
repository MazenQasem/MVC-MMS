 var ViewtempTbl;
var SelectItemsTable;
$(document).ready(function () {
    $('.ViewTxtCSS').attr("readonly", "readonly");

    HideDivs();
    var ReportID = 0;
    ReportID = $('#ReportNo').val();
    ReportString = $('#ReportStrID').val();
    switch (ReportID) {
        case '1223': $('#CashSummaryDiv').show(); break;
        case '1164': $('#CashIssueDetailsDiv').show(); break;
        case '1168': $('#SummaryCashCreditReceiptsDiv').show(); break;
        case '1236': $('#PhOperatorWiseDispensesDiv').show(); break;
        case '1176': $('#ItemWiseIssuesDetailsDiv').show(); break;
        case '1103': $('#ItemLedgerSheetDiv').show(); break;
        case '1104': $('#ItemLedgerSheetPriceDiv').show(); break;
        case '1114': $('#ExpiryDateReportDiv').show(); break;
        case '1189': $('#ISBSDiv').show(); break;
        case '1087': $('#ItemMasterListDiv').show(); break;
        case '1234': $('#IPWiseIssueDiv').show(); break;
        case '1241': $('#ErrMedicationSummary').show(); break;
        case '1101': $('#AdjIssDiv').show(); break;
        case '1102': $('#AdjRecDiv').show(); break;
        case '1233': $('#ADTDiv').show(); break;
        case '1240': $('#AntiUsedDiv').show(); break;
        case '0': {
            switch (ReportString) {
                case 'ActiveProfile': $('#ActiveProfileDiv').show(); break;
            }
        }
    }
    SetParameters(ReportID, ReportString);
    LoadCommonList();

    $(".txtPIN").keydown(function (e) {
        var len = 0;
                 len = $(".txtPIN").val().length;
        if (len >= 10) { e.preventDefault(); }

                                    
                 if (e.keyCode == 13) {
            var nn = GetNameOrVal("select title + '.' + firstname + ' ' +  middlename + ' ' + lastname + ' ' + familyname as name "
                + " from patient where registrationno='" + $('.txtPIN').val() + "'", "name", 'PTName');
            if (!(nn == "")) { MazAlert("PIN Not Found!"); $('.txtPIN').focus(); return false; }
        }


                 if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                         (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                         (e.keyCode >= 35 && e.keyCode <= 40)) {
                         return;
        }
                 if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }


    });
    $(".txtPIN").focusout(function (e) {

        var len = 0;
        len = $(".txtPIN").val().length;
        if (len >= 10) { e.preventDefault(); }
        var nn = GetNameOrVal("select title + '.' + firstname + ' ' +  middlename + ' ' + lastname + ' ' + familyname as name "
            + " from patient where registrationno='" + $('.txtPIN').val() + "'", "name", 'PTName');


    });
         $(".txtPIN16").keydown(function (e) {
        var len = 0;
                 len = $(".txtPIN16").val().length;
        if (len >= 10) { e.preventDefault(); }

                                    
                 if (e.keyCode == 13) {
            var nn = GetNameOrVal("select title + '.' + firstname + ' ' +  middlename + ' ' + lastname + ' ' + familyname as name "
                + " from patient where registrationno='" + $('.txtPIN16').val() + "'", "name", 'PTName');
            if (!(nn == "")) { MazAlert("PIN Not Found!"); $('.txtPIN16').focus(); return false; }
        }


                 if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                         (e.keyCode == 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                         (e.keyCode >= 35 && e.keyCode <= 40)) {
                         return;
        }
                 if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }


    });
    $(".txtPIN16").focusout(function (e) {

        var len = 0;
        len = $(".txtPIN16").val().length;
        if (len >= 10) { e.preventDefault(); }
        var nn = GetNameOrVal("select title + '.' + firstname + ' ' +  middlename + ' ' + lastname + ' ' + familyname as name "
            + " from patient where registrationno='" + $('.txtPIN16').val() + "'", "name", 'PTName');


    });
});
function HideDivs() {
    $('#CashSummaryDiv').hide();
    $('#CashIssueDetailsDiv').hide();
    $('#SummaryCashCreditReceiptsDiv').hide();
    $('#PhOperatorWiseDispensesDiv').hide();
    $('#ItemWiseIssuesDetailsDiv').hide();
    $('#ItemLedgerSheetDiv').hide();
    $('#ItemLedgerSheetPriceDiv').hide();
    $('#ExpiryDateReportDiv').hide();
    $('#ISBSDiv').hide();
    $('#ItemMasterListDiv').hide();
    $('#IPWiseIssueDiv').hide();
    $('#ErrMedicationSummary').hide();
    $('#ActiveProfileDiv').hide();
    $('#AdjIssDiv').hide();
    $('#AdjRecDiv').hide();
    $('#ADTDiv').hide();
    $('#AntiUsedDiv').hide();
    return false;
}
function SetParameters(ReportID, ReportString) {
    switch (ReportID) {
        case '1223':
            {
                var xDate = new Date();
                $('#FromDate').datepicker("setDate", xDate);
                break;
            }
        case '1164':
            {
                $('#txtPIN').focus();
                var xDate = new Date();
                $('#FromDate2').datepicker("setDate", xDate).datepicker("option", "dateFormat", 'M yy');;
                break;
            }
        case '1168':
            {
                var xDate = new Date();
                $('#FromDate3').datepicker("setDate", xDate);
                $('#ListBillType').append('<option selected="selected" value="1">Cash</option>');
                $('#ListBillType').append('<option selected="" value="2">Credit</option>');
                LoadList("#EmpList", " select id as ID,Employeeid + '   - ' + name as Name from employee where deleted=0", ' ', ' order by name ', false);
                $('#EmpList').append('<option selected="selected" value="0">All</option>');
                break;
            }
        case '1236':
            {
                var xDate = new Date();
                $('#FromDate4').datepicker("setDate", xDate);
                $('#FromDate4').inputmask("yyyy-mm-dd");

                $('#ToDate4').datepicker("setDate", xDate);
                $('#ToDate4').inputmask("yyyy-mm-dd");

                $('#ListBillType4').append('<option selected="selected" value="1">IP</option>');
                $('#ListBillType4').append('<option selected="" value="2">OP</option>');

                $('#ReprtList4').append('<option selected="selected" value="1">Dispensed Summary</option>');
                $('#ReprtList4').append('<option selected="" value="2">Dispensed Detail</option>');

                $('#OPRadio4').trigger('click');
                $('#uniform-OPRadio4 >span').addClass('checked');
                $('#OPRadio4').val(1);


                $('#FromTime4').inputmask({
                    mask: "h:s",
                    placeholder: "hh:mm",
                    alias: "datetime",
                    hourFormat: "24"
                }).val("12:00");

                $('#ToTime4').inputmask({
                    mask: "h:s",
                    placeholder: "hh:mm",
                    alias: "datetime",
                    hourFormat: "24"
                }).val("00:00");;


                LoadList("#StationList4", " select  ID,Name from Station where deleted=0 ", "", " order by Name ", false, parseInt(gStationID()));

                LoadList("#EmpList4", "SELECT distinct(U.ID) as ID,EmployeeID + '   - ' + U.NAME as Name  "
                    + " FROM L_USERROLES LU,employee U,  L_ROLEFunctions LR "
                    + " WHERE U.ID = LU.USER_ID AND LU.ROLE_ID=LR.ROLE_ID  and "
                    + " LR.STATION_ID = " + gStationID()
                    + " And LR.MODULE_ID =  " + gModelID() + " ", "", "", false, 0, true);


                LoadList("#PHList4", " SELECT E.ID as ID,E.EMPLOYEEID + '   - ' + E.FIRSTNAME + ' ' + E.MIDDLENAME + ' ' + E.LASTNAME AS Name "
                    + "  FROM EMPLOYEE E,DEPARTMENT D WHERE E.CategoryID=12 and E.DEPARTMENTID = D.ID AND E.DELETED =0 AND D.DEPTCODE in ('PH','PHA') ",
                    "", " ORDER BY FIRSTNAME,MIDDLENAME,LASTNAME ", false, 0, true);


                break;
            }
        case '1176':
            {
                var xDate = new Date();
                $('#FromDate5').datepicker("setDate", xDate);
                $('#FromDate5').inputmask("yyyy-mm-dd");

                $('#ToDate5').datepicker("setDate", xDate);
                $('#ToDate5').inputmask("yyyy-mm-dd");
                $('#txtItemID5').hide();
                break;
            }
        case '1103':
            {
                var xDate = new Date();
                $('#FromDate6').datepicker("setDate", xDate);
                $('#FromDate6').inputmask("yyyy-mm-dd");

                $('#ToDate6').datepicker("setDate", xDate);
                $('#ToDate6').inputmask("yyyy-mm-dd");
                break;
            }
        case '1104':
            {
                var xDate = new Date();
                $('#FromDate7').datepicker("setDate", xDate);
                $('#FromDate7').inputmask("yyyy-mm-dd");

                $('#ToDate7').datepicker("setDate", xDate);
                $('#ToDate7').inputmask("yyyy-mm-dd");

                LoadList("#CategoryList7", "select ID,Name from Itemgroup", "", " order by Name ", false, 0, true);
                $('#IssueTypeList7')
                    .append('<option selected="selected" value="0">All</option>')
                    .append('<option selected="" value="1">Departmental Issue</option>')
                    .append('<option selected="" value="2">Indent Issue</option>')
                    .append('<option selected="" value="3">Issue to Patient</option>')
                    .val('0');
                break;
            }
        case '1114':
            {
                var xDate = new Date();
                $('#FromDate8').datepicker("setDate", xDate);
                $('#FromDate8').inputmask("yyyy-mm-dd");

                $('#ToDate8').datepicker("setDate", xDate);
                $('#ToDate8').inputmask("yyyy-mm-dd");

                LoadList("#CategoryList8", "select ID,Name from Itemgroup", "", " order by Name ", false, 0, true);
                $('#OrderByList8')
                    .append('<option selected="selected" value="0">Item Code</option>')
                    .append('<option selected="" value="1">Last Digit</option>')
                    .append('<option selected="" value="2">Expiry Date</option>')
                    .val('0');
                break;
            }
        case '1189':
            {
                LoadList("#SupplierList9", "select ID,Name from supplier", "", " order by Name ", false, 0, false);
                $('#OrderByList9')
                    .append('<option selected="selected" value="0">Total Stock = 0</option>')
                    .append('<option selected="" value="1">Total Stock > 0</option>')
                    .append('<option selected="" value="2">Total Stock = All</option>')
                    .val('0');
                break;
            }
        case '1087':
            {
                LoadList("#CategoryList10", "select ID,Name from Itemgroup", "", " order by Name ", false, 0, false);
                $('#QtyList10')
                    .append('<option selected="selected" value="0">QOH Stock = All</option>')
                    .append('<option value="1">QOH Stock > 0</option>')
                    .val('1');
                $('#OrderByList10')
                   .append('<option selected="selected" value="0">Item Name</option>')
                   .append('<option  value="1">Item Code</option>')
                   .val('0');
                LoadList("#ShelfList10", "select ID,Name from Shelf where stationid =" + gStationID(), "", " order by Name ", false, 0, true);
                $('#FromDate10').inputmask("yyyy-mm-dd");
                $('#ToDate10').inputmask("yyyy-mm-dd");




                break;
            }
        case '1234':
            {
                LoadList("#StationList11", "select Distinct s.ID as ID,s.Name as Name "
                    + " from Station s,bed b where s.Id=b.StationId and b.Ipid>0 ", " ", " order by Name ", false, 0, true);

                LoadList('#BedList11', "select i.ipid as ID,i.issueauthoritycode +'.' + right('0000000000'+ ltrim(cast(i.RegistrationNo as varchar(10))),10)  "
                + " + '        <---->        '+ 'Bed:' + b.name as Name "
                + " from Inpatient i,bed b where i.ipid=b.ipid ", " ", " order by b.name  ", false, 0, true);


                var xDate = new Date();
                $('#FromDate11').datepicker("setDate", xDate);
                $('#FromDate11').inputmask("yyyy-mm-dd");

                $('#ToDate11').datepicker("setDate", xDate);
                $('#ToDate11').inputmask("yyyy-mm-dd");



                break;
            }
        case '1241': {
            $('#FromDate12').datepicker("setDate", new Date());
            break;
        }
        case '1101': {
            var xDate = new Date();
            $('#FromDate14').datepicker("setDate", xDate);
            $('#FromDate14').inputmask("yyyy-mm-dd");

            $('#ToDate14').datepicker("setDate", xDate);
            $('#ToDate14').inputmask("yyyy-mm-dd");
            LoadList("#CategoryList14", "select ID,Name from Itemgroup", "", " order by Name ", false, 0, true);
            break;
        }
        case '1102': {
            var xDate = new Date();
            $('#FromDate15').datepicker("setDate", xDate);
            $('#FromDate15').inputmask("yyyy-mm-dd");

            $('#ToDate15').datepicker("setDate", xDate);
            $('#ToDate15').inputmask("yyyy-mm-dd");

            LoadList("#CategoryList15", "select ID,Name from Itemgroup", "", " order by Name ", false, 0, true);
            break;
        }
        case '1233': {
            var xDate = new Date();
            $('#FromDate16').datepicker("setDate", xDate);
            $('#FromDate16').inputmask("yyyy-mm-dd");

            $('#ToDate16').datepicker("setDate", xDate);
            $('#ToDate16').inputmask("yyyy-mm-dd");
            LoadList("#StationList16", " select  ID,Name from Station where deleted=0 ", "", " order by Name ", false, 0, true);
            $('.DateDIV16').hide();
            break;
        }
        case '1240':
            {
                var xDate = new Date();
                $('#FromDate17').datepicker("setDate", xDate).datepicker("option", "dateFormat", 'M yy');;
                break;
            }
        case '0': {
            switch (ReportString) {
                case 'ActiveProfile': {
                    LoadList("#StationList13", "select Distinct s.ID as ID,s.Name as Name "
                    + " from Station s where deleted = 0 ", " ", " order by Name ", false, 0, true);

                    LoadList("#PinList13", " SELECT (ip.issueauthoritycode + '.' + "
                    + " replicate('0',10-len(registrationno)) + cast(registrationno as varchar)) as Name, ip.IPID as ID"
                    + " FROM InPatient ip,Bed WHERE ip.IPID = Bed.IPID AND Bed.Status = 5 ", "", " order by ip.registrationno ", false, 0, true)
                }
            }


        }
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

$(document).on('click', '#btnCashSummaryPrint', function () {
    var EDate = new Date().getDay() + 30;
    var Str = " Select a.CashBillno,a.creditbillno companybillno,a.NetAmount, "
        + " a.PinNo,a.datetime,e.Employeeid as Userid,a.Prefix "
        + " from CashIssue a,Employee e where a.stationid = " + $('#gStationID').val()
        + " and a.OperatorId = e.id   "
        + " and a.billtype = 1 and a.datetime >= '" + $('#FromDate').val() + "' "
        + " and a.datetime < DATEADD(day,1,'" + $('#FromDate').val() + "') "
        + " order by CashBillno,creditbillno ";

    PrintOrderStr(Str, 'ReportDialog', "/MISRept/CashSummaryPrint", "DivRpt");
    return false;
});

$(document).on('click', '#btnCashIssueDetailsPrint', function () {
              var Str = " select b.pinno as opid,b.name,co.code, b.prefix + cast(("
    + " ISNULL(case when ((b.cashbillno is null) or (b.cashbillno=0)) then b.creditbillno "
    		+ " when ((b.creditbillno is null) or (b.creditbillno=0)) then b.cashbillno   end,0)) as varchar(20)) as order1, "
    + " b.prefix,b.datetime as datetime1,c.Quantity as quantity, d.name as description,c.price as price, "
    + " u.name as unit,s.name  as Stationname "
    + " from CashIssue b,CashIssuedetail c,item d,packing u,Company co,station s "
    + " where b.pinno = '" + $("#gIACode").val() + "." + $("#txtPIN").val() + "' and  "
    + " b.id=c.billno and s.id=b.stationid "
    + " and c.serviceid=d.id and c.unitid=u.id and co.id =* b.comp_id  "
         + " order by b.id ";


    PrintOrderStr(Str, 'ReportDialog', "/MISRept/CashIssueDetailsPrint", "DivRpt");
    return false;
});

$(document).on('click', '#btnSummaryCashCreditReceiptsPrint', function () {
    var empleng = $('select#EmpList option:selected').val();

    var cond = "";
    if (empleng > 0) {
        cond = " and e.id= " + empleng;
    }
    var EDate = new Date().getDay() + 30;
    if (parseInt($('#ListBillType').val()) == 1) {
        var Str = "Select a.CashBillno,a.creditbillno companybillno,a.NetAmount, "
        + " a.PinNo,a.datetime,e.Employeeid as Userid,a.Prefix "
        + " from CashIssue a,Employee e "
        + " where a.stationid = " + gStationID() + "  and a.OperatorId = e.id   and a.billtype =  " + $('#ListBillType').val()
        + " and a.datetime >= '" + $('#FromDate3').val() + "' and a.datetime < DATEADD(day,1,'" + $('#FromDate3').val() + "') "
        + cond
        + " order by CashBillno,creditbillno";
    } else {
        var Str = "Select a.creditbillno as CashBillno,a.NetAmount, "
     + " a.PinNo,a.datetime,e.Employeeid as Userid,a.Prefix "
     + " from CashIssue a,Employee e "
     + " where a.stationid = " + gStationID() + "  and a.OperatorId = e.id   and a.billtype =  " + $('#ListBillType').val()
     + " and a.datetime >= '" + $('#FromDate3').val() + "' and a.datetime < DATEADD(day,1,'" + $('#FromDate3').val() + "') "
     + cond
     + " order by creditbillno";
    }

    PrintOrderStr(Str, 'ReportDialog', "/MISRept/SummaryCashCreditReceiptsPrint", "DivRpt");
    return false;
});

$(document).on('click', '#btnPhOperatorWiseDispensesPrint', function () {

    var billType = parseInt($('#ListBillType4').val());      var OperatorID = parseInt($('#EmpList4').val());
    var ReportTypeID = parseInt($('#ReprtList4').val());       var PHarID = parseInt($('#PHList4').val());
    var Stationid = parseInt($('#StationList4').val());
    var FromDate = $('#FromDate4').val() + ' 00:00';
    var ToDate = $('#ToDate4').val() + ' 23:59';
    var FromTime = new Date($('#FromDate4').val() + ' ' + $('#FromTime4').val());
    var ToTime = new Date($('#FromDate4').val() + ' ' + $('#ToTime4').val());
    var RadioID = $('input:radio[name=EmpList4Radio]:checked').val();
         var prfix = ""; var Title2 = ""; var Title1 = ""; var ExLine = 0;

    var Cond = "  ";
    if (billType == 1) {
        if (OperatorID > 0) {
            Cond = " and DO.OperatorID=" + OperatorID;
        }

        if (PHarID > 0) {
            Cond = " and DO.PharmacistID=" + PHarID;
        }
    } else if (billType == 2) {
        if (OperatorID > 0) {
            Cond = " and CI.OperatorID=" + OperatorID;
        }

        if (PHarID > 0) {
            Cond = " and CI.DispatchDoctorID=" + PHarID;
        }


    }

    if (RadioID == 1) {
        prfix = "EO";
        Title2 = " OPERATOR ";
    } else {
        prfix = "EP";
        Title2 = "PHARMACIST";
    }

    if (ReportTypeID == 1)      {
        Title1 = " SUMMARY ";
        ExLine = 0;
    } else {
        Title1 = " DETAIL ";
        ExLine = 30;
    }
    var StrSql = "";
    var ReportType = 0;
    if (billType == 1 && ReportTypeID == 1) {
        ReportType = 1;
        StrSql = "Select " + prfix + ".EmployeeID as PEMployeeID," + prfix + ".FIRSTNAME," + prfix + ".MIDDLENAME," + prfix + ".LASTNAME , "
        + " count(Case  When P.CompanyID=1 then 'CASH' End ) as  'CASH', count(Case  When P.CompanyID<>1 then 'COMPANY' End ) "
        + " as  'COMPANY' from  drugorder DO,  Employee EP ,Employee EO,Doctor D,Station S,AllInpatients P  where   DO.OperatorID=EO.ID "
        + "  and  DO.PharmacistID=EP.ID and DO.Dispatched<>1 and DO.StationID=S.ID and DO.IPID =P.IPID and  DO.DoctorID=D.ID  "
        + "  and DO.ToStationID=" + Stationid + " "
        + "  and (Do.DispatchedDateTime between  '" + FromDate + "' and  '" + ToDate + "') "
        + "  and DATEPART(hh,Do.DispatchedDateTime)>= " + FromTime.getHours() + " and (case when DATEPART(hh,Do.DispatchedDateTime)= " + FromTime.getHours()
        + " then DATEPART(mi,Do.DispatchedDateTime) else 60 end) > " + FromTime.getMinutes() + " "
        + " and DATEPART(hh,Do.DispatchedDateTime)<= " + ToTime.getHours() + " and (case when DATEPART(hh,Do.DispatchedDateTime)= " + ToTime.getHours()
        + " then DATEPART(mi,Do.DispatchedDateTime) else -1 end) < " + ToTime.getMinutes() + "  " + Cond + " "
        + " Group by " + prfix + ".EmployeeID," + prfix + ".FIRSTNAME," + prfix + ".MIDDLENAME," + prfix + ".LASTNAME order by " + prfix + ".EmployeeID ";
    } else if (billType == 2 && ReportTypeID == 1) {
        ReportType = 2;
        StrSql = "Select  " + prfix + ".EmployeeID as PEMployeeID," + prfix + ".FIRSTNAME," + prfix + ".MIDDLENAME," + prfix
        + ".LASTNAME ,Count(Case  When  CI.BillType=1  then 1 end) as 'CASH' ,"
        + " Count(Case when CI.BillType=2 then 1 end) as 'COMPANY'    from  CashIssue CI,Employee EO, Employee EP ,Doctor D,Company Q  where CI.Comp_Id*=Q.ID and "
        + " CI.OperatorID=EO.ID and CI.DispatchDoctorID=EP.ID and  CI.DoctorID=D.ID   and CI.StationID=" + Stationid + "  and  "
        + " (CI.DateTime between  '" + FromDate + "' and  '" + ToDate + "')  and DATEPART(hh,CI.DateTime)>= " + FromTime.getHours() + " "
        + " and (case when DATEPART(hh,CI.DateTime)=" + FromTime.getHours() + "  then DATEPART(mi,CI.DateTime) else 60 end) >" + FromTime.getMinutes()
        + " and DATEPART(hh,CI.DateTime)<= " + ToTime.getHours() + " "
        + " and (case when DATEPART(hh,CI.DateTime)=" + ToTime.getHours()
        + " then DATEPART(mi,CI.DateTime) else -1 end) <" + ToTime.getMinutes() + "  and CancelYesNo=0    " + Cond + " "
        + " Group by " + prfix + ".EmployeeID," + prfix + ".FIRSTNAME," + prfix + ".MIDDLENAME," + prfix + ".LASTNAME  order by " + prfix + ".EmployeeID ";
    } else if (billType == 1 && ReportTypeID == 2) {
        ReportType = 3;
        

                                                                                                            
        StrSql = "Select Do.ID,S.Prefix,Do.StationSLNo, EP.EmployeeID as PEmployeeID,D.EmpCode,EO.EmployeeID as OEmployeeID, "
      + " DO.DispatchedDateTime, DD.Price as 'Price',DD.DispatchQuantity,I.ItemCode,P.RegistrationNO,isnull(Q.Code,'0000') as  Company   from  drugorder DO "
      + " ,DrugOrderDetail DD,Item I ,Employee EO, Employee EP ,Doctor D,Station S,AllInpatients P,Company Q where P.CompanyID=Q.ID  "
      + " and Do.Id=DD.OrderID and DD.ServiceID=I.ID and DO.OperatorID=EO.ID and DO.PharmacistID=EP.ID and DO.Dispatched<>1 and "
      + "  DO.StationID=S.ID and DO.IPID =P.IPID and DO.DoctorID=D.ID   and DO.ToStationID=" + Stationid + " "
      + " and  (Do.DispatchedDateTime between  '" + FromDate + "' and  dateadd(d,1,'" + FromDate + "')) "
      + " and DATEPART(hh,Do.DispatchedDateTime)>= " + FromTime.getHours() + " and (case when DATEPART(hh,Do.DispatchedDateTime)=" + FromTime.getHours() + " "
      + " then DATEPART(mi,Do.DispatchedDateTime) else 60 end) >" + FromTime.getMinutes()
      + " and DATEPART(hh,Do.DispatchedDateTime)<= " + ToTime.getHours() + " "
      + " and (case when DATEPART(hh,Do.DispatchedDateTime)=" + ToTime.getHours()
      + " then DATEPART(mi,Do.DispatchedDateTime) else -1 end) <" + ToTime.getMinutes() + " " + Cond
      + " order by CompanyID,DO.DispatchedDateTime Desc,PEmployeeID";
        $('#ToDate4').val($('#FromDate4').val());
    } else if (billType == 2 && ReportTypeID == 2) {
        ReportType = 4;
        

                                                                                                                    StrSql = "Select CI.ID,CI.Prefix,(Case  When  CI.BillType=1 then  CI.CashBillNo else CI.CreditBillNo end) as BillNo, EP.EmployeeID as PEmployeeID,"
       + " D.EmpCode,EO.EmployeeID as OEmployeeID,CI.DateTime,  CID.Quantity,I.ItemCode,CI.RegNO, isnull(Q.Code,'0000' ) as  Company,CI.Comp_ID ,CI.Cash_Credit, "
       + " CI.BillType  from  CashIssue CI,cashIssueDetail CID,Item I ,Employee EO, Employee EP ,Doctor D,Company Q  where CI.Comp_Id*=Q.ID and  CI.Id=CID.BillNo "
       + " and CID.ServiceID=I.ID and CI.OperatorID=EO.ID and CI.DispatchDoctorID=EP.ID and  CI.DoctorID=D.ID   and CI.StationID=" + Stationid + " "
       + "  and  (CI.DateTime between  '" + FromDate + "' and  dateadd(d,1,'" + FromDate + "')) "
       + " and DATEPART(hh,CI.DateTime)>= " + FromTime.getHours()
       + " and (case when DATEPART(hh,CI.DateTime)=" + FromTime.getHours()
       + " then DATEPART(mi,CI.DateTime) else 60 end) >" + FromTime.getMinutes() + " "
       + " and DATEPART(hh,CI.DateTime)<= " + ToTime.getHours()
       + " and (case when DATEPART(hh,CI.DateTime)=" + ToTime.getHours()
       + " then DATEPART(mi,CI.DateTime) else -1 end) <" + ToTime.getMinutes() + " and CancelYesNo=0   " + Cond + " "
       + " order by CI.DateTime Desc,PEmployeeID ";
        $('#ToDate4').val($('#FromDate4').val());
    }
         var Title = Title1 + " OF DISPENSED MEDICINE BY " + Title2;
    var TitleDate = 'From : ' + $('#FromDate4').val() + '    To : ' + $('#ToDate4').val();
    var TitleSheft = 'Sheft : ' + $('#FromTime4').val() + '    To : ' + $('#ToTime4').val();
    var TitleStation = 'Station Name : ' + GetListTxt('#StationList4');

         PrintOrderList({ Str: StrSql, ReportType: ReportType, Title: Title, TitleDate: TitleDate, TitleSheft: TitleSheft, TitleStation: TitleStation }, 'ReportDialog', "/MISRept/PhOperatorWiseDispensesPrint", "DivRpt");

    return false;
});
$(document).on('click', '#PHList4', function () {

    SelectListValue('#EmpList4', 0);
});
$(document).on('click', '#EmpList4', function () {
    SelectListValue('#PHList4', 0);
});


$(document).on('click', '#btnItemWiseIssuesDetailsPrint', function () {
    var FromDate = $('#FromDate5').val();
    var ToDate = $('#ToDate5').val();
    $('#txtItemID5').show();
    GetNameOrVal("Select ID from Item Where Itemcode='" + $('#txtItem5').val().trim() + "' ", "ID", "txtItemID5");
    var iId = 0;
    iId = parseInt($('#txtItemID5').val());
    $('#txtItemID5').hide();
    if (iId > 0) {

                 var StrSql = " select a.prefix,a.pinno,a.billtype, a.prefix + cast(a.cashbillno as varchar(100)) as cashbillno "
                  + " ,a.prefix  +cast(a.creditbillno as varchar(100)) as creditbillno,b.serviceid, a.name, "
                  + " a.datetime,sum(b.quantity)as quantity,a.cancelyesno from "
                  + " cashissue a, cashissuedetail b where a.id=b.billno and "
                  + " a.datetime>='" + FromDate + "' and "
                  + " a.datetime< DATEADD(d,1,'" + ToDate + "') "
                  + " and a.stationid='" + gStationID() + "' and b.serviceid=" + iId
                  + " group by a.billtype,b.serviceid,a.name,a.datetime,a.cashbillno,a.creditbillno,a.cancelyesno,a.pinno,a.prefix";

        var Title = " MEDICINE ISSUES  ";
        var TitleDate = 'From : ' + $('#FromDate5').val() + '    To : ' + $('#ToDate5').val();
        var TitleStation = 'Station Name : ' + gStationName();
        PrintOrderList({ Str: StrSql, Title: Title, TitleDate: TitleDate, TitleStation: TitleStation }, 'ReportDialog', "/MISRept/ItemWiseIssuesDetailPrint", "DivRpt");

    }

    return false;
});

$(document).on('click', '#btnItemLedgerSheetPrint', function () {

    var FromDate = $('#FromDate6').val();
    var ToDate = $('#ToDate6').val();
    var ItemCode = $('#txtItem6').val();

    PrintOrderList({ FromDate: FromDate, ToDate: ToDate, ItemCode: ItemCode }, 'ReportDialog',
        "/MISRept/ItemLedgerSheetPrint", "DivRpt");

    return false;
});

$(document).on('click', '#btnItemLedgerSheetPricePrint', function () {

    var FromDate = $('#FromDate7').val();
    var ToDate = $('#ToDate7').val();
    var ItemCode = $('#txtItem7').val();
    var CatID = parseInt($('#CategoryList7').val());
    if (new Date($('#ToDate7').val()) < new Date($('#FromDate7').val())) { ShowMessage("To date can't be less than the from date"); return false; }

    var RadioID = parseInt($('#IssueTypeList7').val());
    var RadioText = $('#txtIssueType7').val();
    if (RadioID == 0) { RadioText = ""; }
    if (RadioID != 0 && RadioText == "") { ShowMessage("Please Enter Issue No#"); return false; }



    PrintOrderList({
        FromDate: FromDate, ToDate: ToDate, ItemCode: ItemCode,
        CatID: CatID, RadioID: RadioID, RadioText: RadioText
    }, 'ReportDialog', "/MISRept/ItemLedgerSheetPricePrint", "DivRpt");

    return false;
});

$(document).on('click', '#btnExpiryDateReportPrint', function () {

    var FromDate = $('#FromDate8').val();
    var ToDate = $('#ToDate8').val();
    var CatID = parseInt($('#CategoryList8').val());
    if (new Date($('#ToDate8').val()) < new Date($('#FromDate8').val())) { ShowMessage("To date can't be less than the from date"); return false; }
    var RadioID = parseInt($('#OrderByList8').val());
    var chkAsDate;
    var chkPrice;
    if ($('#chkAsOfDate').is(':checked')) { chkAsDate = 1; } else { chkAsDate = 0; }
    if ($('#chkPrice').is(':checked')) { chkPrice = 1; } else { chkPrice = 0; }
    PrintOrderList({
        FromDate: FromDate, ToDate: ToDate, CatID: CatID, RadioID: RadioID, chkAsDate: chkAsDate, chkPrice: chkPrice
    }, 'ReportDialog', "/MISRept/ExpiryDateReportPrint", "DivRpt");

    return false;
});

    
    

             

  
 
 



$(document).on('click', '#btnItemMasterListPrint', function () {

    var CatID = parseInt($('#CategoryList10').val());
    var prefix = $('#txtPrefix').val();
    var suffix = $('#txtSuffix').val();
    var QtyList = parseInt($('#QtyList10').val());
    var OrderList = parseInt($('#OrderByList10').val());
    var FromDate = $('#FromDate10').val();
    var ToDate = $('#ToDate10').val();

    var chkWithoutLoc = 0;
    if ($('#chkWithoutLoc').is(':checked')) { chkWithoutLoc = 1; } else { chkWithoutLoc = 0; }
    var chkWithExpiry = 0;
    if ($('#chkWithExpiry').is(':checked')) { chkWithExpiry = 1; } else { chkWithExpiry = 0; }
    var chkCost = 0;
    if ($('#chkCost').is(':checked')) { chkCost = 1; } else { chkCost = 0; }
    var ShelfID = parseInt($('#ShelfList10').val());

    PrintOrderList({
        CatID: CatID, prefix: prefix, suffix: suffix
        , QtyList: QtyList, OrderList: OrderList, FromDate: FromDate, ToDate: ToDate
        , chkWithoutLoc: chkWithoutLoc, chkWithExpiry: chkWithExpiry, chkCost: chkCost
        , ShelfID: ShelfID
    }, 'ReportDialog', "/MISRept/ItemMasterListPrint", "DivRpt");

    return false;
});


$(document).on('click', '#btnIPWiseIssuePrint', function () {

    var StationID = parseInt($('#StationList11').val());
    var IPID = parseInt($('#BedList11').val());
    var FromDate = $('#FromDate11').val();
    var ToDate = $('#ToDate11').val();


    PrintOrderList({
        StationID: StationID, IPID: IPID, FromDate: FromDate, ToDate: ToDate
    }, 'ReportDialog', "/MISRept/IPWiseIssuePrint", "DivRpt");

    return false;
});
$(document).on('change', '#StationList11', function (e) {
    var StID = parseInt($('#StationList11').val());
    $('#BedList11').find('option').remove();
    if (StID > 0) {
        LoadList('#BedList11', "select i.ipid as ID,i.issueauthoritycode +'.' + right('0000000000'+ ltrim(cast(i.RegistrationNo as varchar(10))),10)  "
                + " + '        <---->        '+ 'Bed:' + b.name as Name "
                + " from Inpatient i,bed b where i.ipid=b.ipid and b.StationId = " + StID, " ", " order by b.name  ", false, 0, true);
    } else {

        LoadList('#BedList11', "select i.ipid as ID,i.issueauthoritycode +'.' + right('0000000000'+ ltrim(cast(i.RegistrationNo as varchar(10))),10)  "
            + " + '        <---->        '+ 'Bed:' + b.name as Name "
            + " from Inpatient i,bed b where i.ipid=b.ipid ", " ", " order by b.name  ", false, 0, true);

    }
});

 $(document).on('click', '#btnISBSPrint', function () {
    var SupID = parseInt($('#SupplierList9').val());
    var RadioID = parseInt($('#OrderByList9').val());
    if (SupID < 1 || RadioID < 0) { ShowMessage("Please select Supplier/Option !"); return false; }
         $('#ISBSiframe').prop('src', GetAppName() + '/Aspx/ReportView.aspx?SupIDx=' + SupID + '&RadioIDx=' + RadioID);
    return false;
});

 $(document).on('click', '#btnErrMedicationSummary', function () {
    var Date = $('#FromDate12').val();
    $('#ErrMedicationSummaryiframe').prop('src', GetAppName() + '/Aspx/ErrMedSummary.aspx?ddate=' + Date);
    return false;
});

         

$(document).on('change', '#StationList13', function () {
    var stationid = parseInt($('#StationList13').val());
    if (stationid > 0) {
        $('#PinList13').MazClearList();
        LoadList("#PinList13", " SELECT (ip.issueauthoritycode + '.' + "
                  + " replicate('0',10-len(registrationno)) + cast(registrationno as varchar)) as Name, ip.IPID as ID"
                  + " FROM InPatient ip,Bed WHERE ip.IPID = Bed.IPID AND Bed.Status = 5 and STATIONID=" + stationid, "", " order by ip.registrationno ", false, 0, true)
    }
    else {
        LoadList("#PinList13", " SELECT (ip.issueauthoritycode + '.' + "
              + " replicate('0',10-len(registrationno)) + cast(registrationno as varchar)) as Name, ip.IPID as ID"
              + " FROM InPatient ip,Bed WHERE ip.IPID = Bed.IPID AND Bed.Status = 5 ", "", " order by ip.registrationno ", false, 0, true)

    }

});
$(document).on('change', '#PinList13', function () {

    var ipid = parseInt($('#PinList13').val());
    var stationid = parseInt($('#StationList13').val());

    if (ipid > 0 && stationid == 0) {
        var ipidStation = 0;
        ipidStation = GetNameOrValVar("select stationid from bed where ipid=" + ipid, "stationid", ipidStation);
        SelectListValue('#StationList13', ipidStation);
    }

});

$(document).on('click', '#btnAdjIssReportPrint', function () {

    var CatID = parseInt($('#CategoryList14').val());
    var chkCost = $('#chkWithCost14').MazGetCheckStatus();
    var FromDate = $('#FromDate14').val();
    var ToDate = $('#ToDate14').val();


    PrintOrderList({
        CatID: CatID, chkCost: chkCost, FromDate: FromDate, ToDate: ToDate
    }, 'ReportDialog', "/MISRept/AdjIssueRPTSheetPrint", "DivRpt");

    return false;
});

$(document).on('click', '#btnAdjRecReportPrint', function () {

    var CatID = parseInt($('#CategoryList15').val());
    var chkCost = $('#chkWithCost15').MazGetCheckStatus();
    var FromDate = $('#FromDate15').val();
    var ToDate = $('#ToDate15').val();


    PrintOrderList({
        CatID: CatID, chkCost: chkCost, FromDate: FromDate, ToDate: ToDate
    }, 'ReportDialog', "/MISRept/AdjRecRPTSheetPrint", "DivRpt");

    return false;
});

$(document).on('change', '#ReportList16', function () {
    var TypeID = parseInt($('#ReportList16').val());
    if (TypeID == 1) {
        $('.DateDIV16').show();
    } else { $('.DateDIV16').hide(); }

    if (TypeID == 0) {
        $('.txtPIN16').show();
    } else {
        $('.txtPIN16').hide();
    }

    return false;
});
$(document).on('click', '#btnADTPrint', function () {
    var TypeID = parseInt($('#ReportList16').val());
    var StationID = parseInt($('#StationList16').val());
    var STName = GetListTxt('#StationList16');
    var PIN = $('.txtPIN16').val();

    var FromDate = $('#FromDate16').val() + ' ' + GetTime();
    var ToDate = $('#ToDate16').val() + ' ' + GetTime();

    PrintOrderList({ TypeID: TypeID, StationID: StationID, STName: STName, PIN: PIN, FromDate: FromDate, ToDate: ToDate },
        'ReportDialog', "/MISRept/ADTPrint", "DivRpt");

    return false;
});


$(document).on('click', '#btnAntiUsedPrint', function () {
    var FromDate = $('#FromDate17').val();
    PrintOrderList({ FromDate: FromDate },
        'ReportDialog', "/MISRept/AntiUsedPrint", "DivRpt");

    return false;
});



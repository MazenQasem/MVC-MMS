 
 

var MyTbl;
var UOMtbl;
var Tbl2;
var IsSearch = false;
$(document).ready(function () {
    $('.ViewTxtCSS').attr("readonly", "readonly");
    $('#CategoryID').attr('disabled', true);

    SetUpImg();
    SetPackingListName();
    LoadRack();
    $("#SelectRack").val($("#SelectRack option:first").val());
    LoadPacking();
    GetBatchTable();
    SelectRack();

    var listSelect = document.getElementById("UOMChieldSelected1");
    $('#UOMCH2').val(listSelect.options[listSelect.selectedIndex].text);
    var listSelect = document.getElementById("UOMChieldSelected2");
    $('#UOMCH3').val(listSelect.options[listSelect.selectedIndex].text);
    var listSelect = document.getElementById("UOMChieldSelected3");
    $('#UOMCH4').val(listSelect.options[listSelect.selectedIndex].text);

    $("#Strength_no").focusout(function () {
        var t = parseFloat($("#Strength_no").val());
        $("#Strength_no").val(t.toFixed(2));
    });


    if ($('#txtItemID').val() > 0) {
        LoadHoldingStation($('#txtItemID').val());
        $('#HideSelectedList').hide();
        LoadSupplierItem($('#txtItemID').val());
        LoadGenericItem($('#txtItemID').val());
    }
});

function SetUpImg() {
    
    $("#sendStorageModal").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });
    $("#openStorageModal").click(function () {
        $("#sendStorageModal").dialog('open');
    });

    
    $("#sendPackingModal").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });
    $("#openPackingModal").click(function () {
        $("#sendPackingModal").dialog('open');
    });

    $("#sendPackModifygModal").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });
    $("#openPackModifygModal").click(function () {
        $("#sendPackModifygModal").dialog('open');
        $("#sendPackingModal").dialog('close');
        $("#openModifyUOMModal").trigger('click');
        LoadAllpack();
    });

    
    $("#sendSupplierModal").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });

    $("#openSupplierModal").click(function () {
        $("#sendSupplierModal").dialog('open');
    });

    
    $("#sendBatchModal").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });
    $("#openBatchModal").click(function () {
        $("#sendBatchModal").dialog('open');
    });

    
    $("#sendUpdateStorageModal").dialog({
        autoOpen: false,
        modal: true,
        width: 600,
        open: function () {
                         fix();
        }
    });
    $("#openUpdateStorageModal").click(function () {
        $("#sendUpdateStorageModal").dialog('open');
    });


         
    $("#sendClinicalModal").dialog({
        autoOpen: false,
        modal: true,
        width: 1200,
        open: function () {
                         fix();
        }
    });
    $("#openClinicalModal").click(function () {
        $("#sendClinicalModal").dialog('open');
    });



}
function LoadHoldingStation(itmid) {
         $("#HideSelectedList >option").each(function () {
        AddTOHodlingStor($(this).val())
    });
    $('#HideSelectedList').hide();
}
function AddTOHodlingStor(stationid) {
    var SourceVal = 0;
    var DestinVal = 0;
    SourceVal = stationid;
    $("#ms-msc .ms-selectable .ms-list >li").each(function () {
                 DestinVal = $(this).attr("ms-value");
        if (DestinVal == SourceVal) {
            $(this).click();
            return false;
        }
    });
}


function LoadSupplierItem(itmid) {
    var SqlStr = "Select a.Id as ID,a.Name as Name from Supplier a,SupplierItem b where a.ID=b.SupplierID  and b.ItemId=" + itmid;
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                                 AddTOItemSupplier(Item.ID);
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });


}
function AddTOItemSupplier(supplierid) {
    var SourceVal = 0;
    var DestinVal = 0;
    SourceVal = supplierid;
         $("#ms-msc1 .ms-selectable .ms-list >li").each(function () {
                 DestinVal = $(this).attr("ms-value");

        if (DestinVal == SourceVal) {
            $(this).click();
            return false;
        }
    });
}

function LoadGenericItem(itmid) {
    var SqlStr = "Select a.Id as ID,a.Name as Name from m_Generic a,ItemGeneric b where a.ID=b.genericid and b.itemid=" + itmid;
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            var StrOption = "";
            $.each(msg, function (i, Item) {
                                 StrOption = StrOption + Item.ID + ",";
            });
            var list = [];
            list = StrOption;
            console.log(list);
            $('#GGenricList').select2('val', list.split(','));
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });


}
function AddTOItemGeneric(genericid) {
    var SourceVal = 0;
    var DestinVal = 0;
    SourceVal = genericid;

         $("#ms-msc2 .ms-selectable .ms-list >li").each(function () {
                 DestinVal = $(this).attr("ms-value");

        if (DestinVal == SourceVal) {
            $(this).click();
            return false;
        }
    });
}


$(document).on('change', '#PackingList', function () {
    var vv = $("select#PackingList option:selected").val();
    var txt = $("select#PackingList option:selected").text();
    $('#UOMName').val(txt);
    $("#UnitID").val(vv);
});
function SetPackingListName() {
    $('#PackingList > option').each(function (i) {
        var txt = $("#UOMName").val();
        var txt2 = $(this).text();
        if (txt == txt2) {
            $(this).attr("selected", "selected");             $("#UnitID").val($(this).val());
        }

    });
}

function LoadRack() {
    var SqlStr = "select Id,name from Rack where stationid=" + gStationID();
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr, AddNone: true }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#SelectRack").append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });

}
function SelectRack(e) {
    var SqlStr = "Select RackID as ID,ShelfID as Name,1 as selected from ItemLocation where ItemId=" + $('#txtItemID').val()
    + " and stationid=";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadOptionList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#SelectRack option[value='" + Item.ID + "']").attr('selected', 'selected');
                $("#SelectRack").trigger('change');
            });
        }
    });
    return false;
}
function SelectShelf(e) {
    var SqlStr = "Select RackID as ID,ShelfID as Name,1 as Selected from ItemLocation where ItemId=" + $('#txtItemID').val()
    + " and stationid=";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadOptionList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            $.each(msg, function (i, Item) {

                
                var rows = $('#tblShelf').dataTable().fnGetNodes();
                                 for (var i = 0; i < rows.length; i++) {
                    if (Item.Name == $(rows[i]).find("td:eq(0)").html()) {
                        $(rows[i]).find("td:eq(1)").find("input").val("true");
                        $(rows[i]).find("td:eq(1)").find("input").attr("checked", "checked");
                    }
                }





            });
        }
    });
    return false;
}
$(document).on("change", "#SelectRack", function (e) {
    var cmbRackID = $('#SelectRack').val();
    var SqlStr = "Select Id,Name,'false' as Selected from Shelf S,RackShelf RS where RS.RackId=" + cmbRackID + " and RS.ShelfId=S.Id and rs.stationid=" + gStationID();
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadShelfList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            render_Table('#tblShelf', msg);
            SelectShelf();
            MyTbl.fnDraw();
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });


});
function render_Table(tableid, datalist) {
    var ItemName;
    MyTbl = $(tableid).dataTable({
        destroy: true,
        data: datalist,
        paging: false,
        ordering: false,
        searching: false,
        autoWidth: false,
        info: false,
        "scrollX": false,
        scrollY: 250,
        processing: false,
        scrollCollapse: false,
        "aoColumnDefs": [
            {
                "aTargets": [0],                 "mData": "ID", "sClass": "hidden",
            },
            {
                "aTargets": [1],
                "mData": "Name", "visible": false,
                "mRender": function (data, type, full) { return ItemName = data; }
            },
            {
                "aTargets": [2], "width": "10%",
                "mData": "Selected",
                "mRender": function (data, type, full) {
                    if (data == "true") {
                                                 alert('match found');
                        return '<input type=\"checkbox\" checked value="' + data + '"><span>' + ItemName + '</span>';

                    } else {
                        return '<input type=\"checkbox\"  value="' + data + '"><span>' + ItemName + '</span>';
                    }

                }
            }


        ]
    });

}




function LoadPacking() {
    var ItemID = $('#txtItemID').val();
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadPackingList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ ItemID: ItemID }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#SelectPacking").append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });

}

$(document).on("change", "#UOMChieldSelected0", function (e) {
    var Parent = $('#UOMChieldSelected0').val();
    LoadNextUOMSelect(Parent, $('#UOMChieldSelected1'));
    $('#UOMChieldSelected1').val($('#UOMChieldSelected1 option:first').val());
});
$(document).on("change", "#UOMChieldSelected1", function (e) {
    var Parent = $('#UOMChieldSelected1').val();
    LoadNextUOMSelect(Parent, $('#UOMChieldSelected2'));
    $('#UOMChieldSelected2').val($('#UOMChieldSelected2 option:first').val());
    $('#UOMCH2').val($('select#UOMChieldSelected1 option:selected').text());
});
$(document).on("change", "#UOMChieldSelected2", function (e) {
    var Parent = $('#UOMChieldSelected2').val();
    LoadNextUOMSelect(Parent, $('#UOMChieldSelected3'));
    $('#UOMChieldSelected3').val($('#UOMChieldSelected3 option:first').val());
    $('#UOMCH3').val($('select#UOMChieldSelected2 option:selected').text());
});
$(document).on("change", "#UOMChieldSelected3", function (e) {
    var Parent = $('#UOMChieldSelected3').val();
    LoadNextUOMSelect(Parent, $('#UOMChieldSelected4'));
    $('#UOMChieldSelected4').val($('#UOMChieldSelected4 option:first').val());
    $('#UOMCH4').val($('select#UOMChieldSelected3 option:selected').text());
});

function LoadNextUOMSelect(Parent, Chield) {
    $(Chield).empty();     var SqlStr = "Select a.packid as ID,b.Name as Name from packingdetail a ,packing b  where parent=" + Parent + " and a.packid=b.id";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr, AddNone: true }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $(Chield).append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });

}


function LoadAllpack() {
    var SqlStr = "SElect id,name from packing order by name";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#SelectAllUOM").append(new Option(Item.Name, Item.ID));
            });
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });

}
$(document).on("change", "#SelectAllUOM", function (e) {
    var cmbAllUOMID = $('#SelectAllUOM').val();
    $('#txtModifyUOMid').val($('select#SelectAllUOM option:selected').val());
    LoadParentUOM(cmbAllUOMID);

});
function LoadParentUOM(UOMID) {
    var SqlStr = "Select ID,name from packing where id<>" + UOMID + " order by name ";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            render_UOMTable('#SelectAllParentUOM', msg);

            SelectUOMParent(this, UOMID);
            UOMtbl.fnDraw();
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });

}
function render_UOMTable(tableid, datalist) {
    var ItemName;
    UOMtbl = $(tableid).dataTable({
        destroy: true,
        data: datalist,
        paging: false,
        ordering: false,
        searching: false,
        autoWidth: false,
        info: false,
        "scrollX": false,
        scrollY: 250,
        processing: false,
        scrollCollapse: false,
        "aoColumnDefs": [
            {
                "aTargets": [0],                 "mData": "ID", "sClass": "hidden",
            },
            {
                "aTargets": [1],
                "mData": "Name", "visible": false,
                "mRender": function (data, type, full) { return ItemName = data; }
            },
            {
                "aTargets": [2], "width": "10%",
                "mData": "Selected",
                "mRender": function (data, type, full) {
                    if (data == "true") {
                                                 alert('match found');
                        return '<input type=\"checkbox\" checked value="' + data + '"><span>' + ItemName + '</span>';

                    } else {
                        return '<input type=\"checkbox\"  value="' + data + '"><span>' + ItemName + '</span>';
                    }

                }
            }


        ]
    });

}
function SelectUOMParent(e, UOMID) {
    var SqlStr = "Select a.parent as ID,b.Name as Name from packingdetail a ,packing b  where packid=" + UOMID + " and a.parent=b.id ";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            $.each(msg, function (i, Item) {

                
                var rows = $('#SelectAllParentUOM').dataTable().fnGetNodes();
                for (var i = 0; i < rows.length; i++) {
                                                              if (Item.ID == $(rows[i]).find("td:eq(0)").html()) {
                        $(rows[i]).find("td:eq(1)").find("input").val("true");
                        $(rows[i]).find("td:eq(1)").find("input").attr("checked", "checked");
                    }
                }
            });
        }
    });
    return false;
}
$(document).on('click', "#openModifyUOMModal", function () {
    $('#UOMModifyForm').show();
    $('#UOMNewForm').hide();
});
$(document).on('click', "#openNewUOMModal", function () {
    $('#UOMModifyForm').hide();
    $('#UOMNewForm').show();
    $('#txtNewUOM').val('');
    $('#txtModifyUOMid').val(0);
    LoadParentUOM(0);
});
$(document).on('click', '#BtnClearNewPackage', function () {
    $("#sendPackingModal").dialog('close');

});
$(document).on('click', '#BtnClearNewPackage1', function () {
    $("#sendPackModifygModal").dialog('close');
    $("#sendPackingModal").dialog('open');
});
$(document).on('click', '#BtnSaveNewPackage1', function () {
    var UOMID = 0;
    UOMID = $('#txtModifyUOMid').val();
    if (UOMID == 0) {
        SaveNewUOM();
    }
    else {
        ModifyUOM();
    }
    return false;

});
function SaveNewUOM() {
    var txt = $('#txtNewUOM').val();
    var TempList = [];
    $('input:checkbox:checked', UOMtbl).each(function () {
        alert($(this).closest('tr').find('td:eq(0)').html());
        TempList.push($(this).closest('tr').find('td:eq(0)').html());
    });

    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/SaveUOM",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ UOMNAME: txt, TempList: TempList }),
        success: function (msg) {
            ShowMessage(JSON.stringify(msg.Message));

        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });
    LoadAllpack();
    return false;
}
function ModifyUOM() {
    var txt = $('select#SelectAllUOM option:selected').text();
    var ID = $('#txtModifyUOMid').val();

    var TempList = [];
    $('input:checkbox:checked', UOMtbl).each(function () {
        alert($(this).closest('tr').find('td:eq(0)').html());
        TempList.push($(this).closest('tr').find('td:eq(0)').html());
    });

    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/SaveUOM",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ UOMNAME: txt, TempList: TempList, UOMID: ID }),
        success: function (msg) {
            ShowMessage(JSON.stringify(msg.Message));
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
        }
    });
    LoadAllpack();
    return false;
}
$(document).on('click', '#BtnInsertPacking', function () {
    var MMS_ItemMaster = {};
    MMS_ItemMaster.UOMChieldSelected0 = $('#UOMChieldSelected0').val();
    MMS_ItemMaster.UOMChieldConvQty1 = $('#UOMChieldConvQty1').val();
    MMS_ItemMaster.UOMChieldSelected1 = $('#UOMChieldSelected1').val();
    MMS_ItemMaster.UOMChieldConvQty2 = $('#UOMChieldConvQty2').val();
    MMS_ItemMaster.UOMChieldSelected2 = $('#UOMChieldSelected2').val();
    MMS_ItemMaster.UOMChieldConvQty3 = $('#UOMChieldConvQty3').val();
    MMS_ItemMaster.UOMChieldSelected3 = $('#UOMChieldSelected3').val();
    MMS_ItemMaster.UOMChieldConvQty4 = $('#UOMChieldConvQty4').val();
    MMS_ItemMaster.UOMChieldSelected4 = $('#UOMChieldSelected4').val();
    MMS_ItemMaster.Id = $('#txtItemID').val();
    var isSuccess = false;

    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/SavePacking",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(MMS_ItemMaster),
        success: function (msg) {
            if (msg.isSuccess == true) {
                isSuccess = true;
                RefreshUOMList();
                ShowMessage(msg.Message);
                $("#sendPackingModal").dialog('close');
                return false;
            }
            else {
                ShowMessage(msg.Message);                 isSuccess = false;
                return false;
            }
        },
        Failure: function (msg) {
                         isSuccess = false;
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });
                        return false;

});

function RefreshUOMList() {

    $("#PackingList").empty();     $("#UOMName").val("");
    $("#UnitID").val("");

    var SqlStr = "select i.packid as ID,p.name as Name from itempacking i, packing p where i.packid = p.id and i.itemid =" + $("#txtItemID").val() + " order by i.slno";
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadListNoStation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ Str: SqlStr }),
        success: function (msg) {
            $.each(msg, function (i, Item) {
                $("#PackingList").append(new Option(Item.Name, Item.ID));
            });
            $('#PackingList').val($('#PackingList option:first').val());
            var vv = $("select#PackingList option:selected").val();
            var txt = $("select#PackingList option:selected").text();
            $('#UOMName').val(txt);
            $("#UnitID").val(vv);
            return false;
        },
        Failure: function (msg) {
            ShowMessage("Failure:" + msg.Message);
            return false;
        }
    });
    return false;
};


$(document).on('click', '#BtnSave', function () {
    if ($('#validate').valid()) {
        var MMS_ItemMaster = {};
        
        MMS_ItemMaster.Id = $('#txtItemID').val();
        MMS_ItemMaster.UOMName = $('#UOMName').val();
        MMS_ItemMaster.UnitID = $('#UnitID').val();         
        MMS_ItemMaster.ItemCode = $('#ItemCode').val();
        MMS_ItemMaster.Name = $('#Name').val();
        MMS_ItemMaster.ItemPrefix = $('#ItemPrefix').val();
        MMS_ItemMaster.iscocktailbool = $('#iscocktailbool').val();
        MMS_ItemMaster.ManufacturerId = $('#ManufacturerId').val();
        MMS_ItemMaster.DrugType = $('input:radio[name=DrugType]:checked').val();
        MMS_ItemMaster.catalogueno = $('#catalogueno').val();
        MMS_ItemMaster.ModelNo = $('#ModelNo').val();
        MMS_ItemMaster.Strength_no = $('#Strength_no').val();
        MMS_ItemMaster.Strength_Unit = $('#Strength_Unit').val();
        MMS_ItemMaster.PartNumber = $('#PartNumber').val();
        MMS_ItemMaster.CategoryID = $('#CategoryID').val();
        MMS_ItemMaster.MaxLevel = $('#MaxLevel').val();
        MMS_ItemMaster.MinLevel = $('#MinLevel').val();
        MMS_ItemMaster.ROL = $('#ROL').val();
        MMS_ItemMaster.strQOH = $('#strQOH').val();
        MMS_ItemMaster.ROQ = $('#ROQ').val();
        MMS_ItemMaster.ProfitCentreID = $('#ProfitCentreID').val();
        MMS_ItemMaster.Tax = $('#Tax').val();
        MMS_ItemMaster.DiscontinueListID = $('#DiscontinueListID').val();
        MMS_ItemMaster.OpeningBalance = $('#OpeningBalance').val();
        MMS_ItemMaster.OpeningBalance = $('#OpeningBalance').val();
        MMS_ItemMaster.ABC = $('input:radio[name=ABC]:checked').val();
        MMS_ItemMaster.FSN = $('input:radio[name=FSN]:checked').val();
        MMS_ItemMaster.VED = $('input:radio[name=VED]:checked').val();
        MMS_ItemMaster.Schedulebool = $('input:radio[name=Schedulebool]:checked').val();
        MMS_ItemMaster.DrugState = $('input:radio[name=DrugState]:checked').val();

        MMS_ItemMaster.EUBbool = $('#EUBbool').val();
        MMS_ItemMaster.FixedAssetbool = $('#FixedAssetbool').val();
        MMS_ItemMaster.NonStockedbool = $('#NonStockedbool').val();
        MMS_ItemMaster.BatchStatusbool = $('#BatchStatusbool').val();
        MMS_ItemMaster.Narcoticbool = $('#Narcoticbool').val();

        MMS_ItemMaster.MRPItembool = $('#MRPItembool').val();
        MMS_ItemMaster.CssdItembool = $('#CssdItembool').val();
        MMS_ItemMaster.CSSDAppbool = $('#CSSDAppbool').val();
        MMS_ItemMaster.Consignmentbool = $('#Consignmentbool').val();
        MMS_ItemMaster.CriticalItembool = $('#CriticalItembool').val();

        MMS_ItemMaster.Approvalbool = $('#Approvalbool').val();
        MMS_ItemMaster.DepartmentIssueBool = $('#DepartmentIssueBool').val();
        MMS_ItemMaster.IndentIssueBool = $('#IndentIssueBool').val();
        MMS_ItemMaster.DuplicateLabelbool = $('#DuplicateLabelbool').val();
        MMS_ItemMaster.Feasibilitybool = $('#Feasibilitybool').val();

        MMS_ItemMaster.Notes = $('#Notes').val();

        
        MMS_ItemMaster.UOMChieldSelected0 = $('#UOMChieldSelected0').val();
        MMS_ItemMaster.UOMChieldConvQty1 = $('#UOMChieldConvQty1').val();
        MMS_ItemMaster.UOMChieldSelected1 = $('#UOMChieldSelected1').val();
        MMS_ItemMaster.UOMChieldConvQty2 = $('#UOMChieldConvQty2').val();
        MMS_ItemMaster.UOMChieldSelected2 = $('#UOMChieldSelected2').val();
        MMS_ItemMaster.UOMChieldConvQty3 = $('#UOMChieldConvQty3').val();
        MMS_ItemMaster.UOMChieldSelected3 = $('#UOMChieldSelected3').val();
        MMS_ItemMaster.UOMChieldConvQty4 = $('#UOMChieldConvQty4').val();
        MMS_ItemMaster.UOMChieldSelected4 = $('#UOMChieldSelected4').val();

        
        
        
        var ItemLocationS = [];
        var rows = $('#tblShelf').dataTable().fnGetNodes();
        var ShelfFound = false;
        for (var i = 0; i < rows.length; i++) {
            var ItemLocation = {};
            ItemLocation.ItemId = $('#txtItemID').val();
            ItemLocation.RackId = $('#SelectRack').val();
            ItemLocation.ShelfId = 0;
            if ($(rows[i]).find("td:eq(1)").find("input").is(":checked")) {
                ShelfFound = true;
                ItemLocation.ShelfId = $(rows[i]).find("td:eq(0)").html();
                ItemLocationS.push(ItemLocation);
            }
        }

        if (ShelfFound == false) { ItemLocationS.push(ItemLocation); }

        MMS_ItemMaster.ItemLocationS = ItemLocationS;

        
        var SelectedStationList = [];
        $("#ms-msc .ms-selection .ms-list >li").each(function () {
            var TempListMdl = {};
            DestinVal = $(this).attr("ms-value");
            TempListMdl.ID = DestinVal;
            SelectedStationList.push(TempListMdl);
        });
        MMS_ItemMaster.SelectedStationList = SelectedStationList;

        
        var AllSupplierList = [];
        $("#ms-msc1 .ms-selection .ms-list >li").each(function () {
            var TempListMdl = {};
            DestinVal = $(this).attr("ms-value");
            TempListMdl.ID = DestinVal;
            AllSupplierList.push(TempListMdl);
        });
        MMS_ItemMaster.AllSupplierList = AllSupplierList;


        
        var AllGenericList = [];
                                                              if ($('#GGenricList').val() != null) {
            var glist = [];
            glist = $('#GGenricList').val();
            $.each(glist, function (index, value) {
                var TempListMdl = {};
                TempListMdl.ID = value;
                AllGenericList.push(TempListMdl);
                             });
        }
        MMS_ItemMaster.AllGenericList = AllGenericList;



        var isSuccess = false;



        $.ajax({
            type: "POST",
            url: GetAppName() + "/Item/UpdateItemMaster",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(MMS_ItemMaster),
            success: function (msg) {

                                                                   if (msg.isSuccess == true) {
                    isSuccess = true;
                                         ShowMessage("Data Saved!");
                    return false;
                }
                else {
                    ShowMessage(msg.Message);                     isSuccess = false;
                    return false;
                }
            },
            Failure: function (msg) {
                                 isSuccess = false;
                ShowMessage("Failure:" + msg.Message);
                return false;
            }
        });
                                            return false;


    }



});


function GetBatchTable() {
    var ItemID = $('#txtItemID').val();
    URL = "/Item/LoadBatchList";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: { ItemID: ItemID },
        success: function (data) {
            render_Table2($('#tblBatch'), data);
        }
    });
}
function render_Table2(tableid, datalist) {
    Tbl2 = $(tableid).dataTable({
        destroy: true,
        data: datalist,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        "scrollX": false,
        scrollY: 250,
        processing: false,
        scrollCollapse: false,
        columnDefs: [
            { data: 'slno', visible: true, targets: [0], title: "slno", width: "5%" },              { data: 'BatchNo', className: 'cAR-align-center', targets: [1], title: "BatchNo", width: "20%" },              { data: 'Quantity', className: 'numbereditor cAR-align-right', targets: [2], title: "Quantity", width: "25%" },              { data: 'ExpiryDate', visible: true, targets: [3], title: "ExpiryDate", width: "50%" }          ]
    }).fnDraw();
}





$(document).on('change', '#ROL', function () {
    var obj = $('#ROL');
    ValidateMax(obj);
});
$(document).on('change', '#ROQ', function () {
    ValidateMax($(this));
});
$(document).on('change', '#MinLevel', function () {
    ValidateMax($(this));
});
function ValidateMax(Itm) {
    if (Itm.val() > $('#MaxLevel').val()) {
        notify('Wrong Entry', 'Value must be less than Max Level Value.');
        Itm.val('0');
    }
}

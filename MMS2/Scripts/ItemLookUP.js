 var MyTbl;
var MyTbl2;
var CategoryListID = -1;
var IsSearch = false;
$(document).ready(function () {


    LoadItemGroup();
    if ($(".select").length > 0) {
        $(".select").on("change", function (e) {
            var ControlID = $(this).attr("id");
            if (ControlID == "CategoryList") {
                CategoryListID = e.val
            }
        })
    }
});

function LoadItemGroup() {
    $.ajax({
        type: "POST",
        url: GetAppName() + "/Item/LoadItemGroup",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $("#CategoryList").empty();
            $.each(msg, function (i, Item) {
                $("#CategoryList").append(new Option(Item.Name, Item.ID))
            });
            return false
        },
        Failure: function (msg) {
            return false
        }
    })
};

$(document).on('click', '#btnView', function () {
    ValidateEntry();
    if (IsSearch == false) {
        return false
    } else {
        var SqlStr = "";
        if ($('select#ItemSelectOperator option:selected').val() == 0 && $('#ItemText').val().length != 0) {
            SqlStr += " and a.name like '" + $('#ItemText').attr("value").trim() + "%'"
        } else if ($('select#ItemSelectOperator option:selected').val() == 1 && $('#ItemText').val().length != 0) {
            SqlStr += " and a.name = '" + $('#ItemText').attr("value").trim() + "'"
        }
        if ($('select#CategorySelectOperator option:selected').val() == 1 && CategoryListID >= 0) {
            SqlStr += " and (b.Parent = " + CategoryListID + " OR " + "b.id = " + CategoryListID + ")"
        }
        if ($('select#SupplierSelectOperator option:selected').val() == 0 && $('#SupplierText').val().length != 0) {
            SqlStr += " and c.name like '" + $('#SupplierText').attr("value").trim() + "%'"
        } else if ($('select#SupplierSelectOperator option:selected').val() == 1 && $('#SupplierText').val().length != 0) {
            SqlStr += " and c.name = '" + $('#SupplierText').attr("value").trim() + "'"
        }
        if ($('select#ManufacturerSelectOperator option:selected').val() == 0 && $('#ManufacturerText').val().length != 0) {
            SqlStr += " and m.name like '" + $('#ManufacturerText').attr("value").trim() + "%'"
        } else if ($('select#ManufacturerSelectOperator option:selected').val() == 1 && $('#ManufacturerText').val().length != 0) {
            SqlStr += " and m.name = '" + $('#ManufacturerText').attr("value").trim() + "'"
        }
        if ($('select#PartSelectOperator option:selected').val() == 0 && $('#PartText').val().length != 0) {
            SqlStr += " and a.PartNumber like '" + $('#PartText').attr("value").trim() + "%'"
        } else if ($('select#PartSelectOperator option:selected').val() == 1 && $('#PartText').val().length != 0) {
            SqlStr += " and a.PartNumber = '" + $('#PartText').attr("value").trim() + "'"
        }
        if ($('select#ItemCodeSelectOperator option:selected').val() == 0 && $('#ItemCodeText').val().length != 0) {
            SqlStr += " and a.itemcode like '" + $('#ItemCodeText').attr("value").trim() + "%'"
        } else if ($('select#ItemCodeSelectOperator option:selected').val() == 1 && $('#ItemCodeText').val().length != 0) {
            SqlStr += " and a.itemcode = '" + $('#ItemCodeText').attr("value").trim() + "'"
        }
        var SqlStr2 = "";
        if ($('#SupplierText').val().length != 0) {
            SqlStr2 = "Select a.id as ID,a.name as ItemName,b.name as GroupName, c.name as SupName,a.itemcode as ItemCode, " + " m.name as ManuName,isnull(A.SellingPrice * isnull(a.conversionqty,1),0) as SellPrice";
            if ($('#ManufacturerText').val().length != 0) {
                SqlStr2 += " from item a, itemgroup b, supplier c,supplieritem s,Manufacturer m " + " where a.manufacturerid = m.id and s.itemid = a.id  and c.id = s.supplierid and a.categoryid=b.id " + SqlStr
            } else {
                SqlStr2 += " from item a, itemgroup b, supplier c,supplieritem s,Manufacturer m " + " where a.manufacturerid *= m.id and s.itemid = a.id  and c.id = s.supplierid and a.categoryid=b.id" + SqlStr
            }
        } else {
            SqlStr2 = "Select a.id as ID,a.name as ItemName,b.name as GroupName,' ' as SupName,a.itemcode as ItemCode, " + " m.name as ManuName,isnull(A.SellingPrice * isnull(a.conversionqty,1),0) as SellPrice";
            if ($('#ManufacturerText').val().length != 0) {
                SqlStr2 += " from item a, itemgroup b,Manufacturer m  where  a.manufacturerid = m.id and a.deleted=0 and a.categoryid=b.id" + SqlStr
            } else {
                SqlStr2 += " from item a, itemgroup b,Manufacturer m  where  a.manufacturerid *= m.id and a.deleted=0 and a.categoryid=b.id" + SqlStr
            }
        }
        CreateTable(SqlStr2);
        return false
    }; if (IsSearch == false) {
        return false
    }
});

function ValidateEntry() {
    var ValidateFlag = false;
    var DivExist = $('.formErrorContent');
    if (DivExist.length) {
        ValidateFlag = false
    } else {
        ValidateFlag = true
    } if ($('#ItemText').val().length == 0 && CategoryListID == -1 && $('#SupplierText').val().length == 0 && $('#ManufacturerText').val().length == 0 && $('#PartText').val().length == 0 && $('#ItemCodeText').val().length == 0) {
        IsSearch = false;
        ShowMessage('please enter some data for search ')
    } else {
        if (ValidateFlag == true) {
            IsSearch = true
        } else {
            IsSearch = false
        }
    }
    return false
};

function CreateTable(Str) {
    URL = "/Item/ItemLookupView";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: {
            str: Str
        },
        success: function (data) {
            render_Table('#DataTables_Table_1', data)
        }
    })
}

function render_Table(tableID, dataxc) {
    var st = '<ul><li><a class="ViewBatch" href="#fModal-Batch" role="button"  data-toggle="modal">Batches</a></li>' + '<li><a href="#" class="ViewDTL">Detail</a></li></ul>';
    MyTbl = $(tableID).DataTable({
        destroy: true,
        data: dataxc,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        "scrollX": false,
        scrollY: 250,
        processing: false,
        scrollCollapse: false,
        columnDefs: [{
            name: 'Dtl',
            defaultContent: st,
            "width": "10%",
            targets: [-1]
        }, {
            name: 'ID',
            className: 'cAR-align-center',
            "width": "5%",
            targets: [0]
        }, {
            name: 'ItemCode',
            targets: [1],
            className: 'cAR-align-left',
            "width": "10%"
        }, {
            name: 'ItemName',
            className: 'cAR-align-center',
            targets: [2],
            "width": "30%"
        }, {
            name: 'GroupName',
            className: 'cAR-align-right',
            targets: [3],
            "width": "15%"
        }, {
            name: 'SupName',
            className: 'listeditor cAR-align-right',
            targets: [4],
            "width": "10%"
        }, {
            name: 'ManuName',
            className: 'cAR-align-center',
            targets: [5],
            "width": "15%"
        }, {
            name: 'SellPrice',
            targets: [6],
            "width": "5%"
        }]
    })
};
$(document).on('click', '.ViewBatch', function (e) {
    var data = MyTbl.row($(this).parents('tr')).data();
    CallBatchView(data[0]);
    return false
});

function CallBatchView(itemid) {
    URL = "/Item/ItemLookupBatch";
    $.ajax({
        url: GetAppName() + URL,
        type: "POST",
        cache: false,
        dataType: 'json',
        data: {
            ItemID: itemid
        },
        success: function (data) {
            $('#lblQTY').text(data.QOH);
            $('#lblROL').text(data.ROL);
            $('#lblROQ').text(data.ROQ);
            $('#lblMinlvl').text(data.MinLevel);
            $('#lblMaxlvl').text(data.MaxLevel);
            $('#lblUOM').text(data.UOM);
            $('#lbllocation').text(data.location);
            var Btl = data.BatchInfo;
            render_BatchTable('#tblBatch', Btl)
        }
    });
    return false
}

function render_BatchTable(tableID, dataxc) {
    for (var i = 0; i < dataxc.length; i++) {
        $(tableID).append('<tr class="odd">' + '<td class=" "><a href="#">' + dataxc[i].slno + '</a></td>' + '<td class=" "><a href="#">' + dataxc[i].BatchNo + '</a></td>' + '<td class=" "><a href="#">' + dataxc[i].Quantity + '</a></td>' + '<td class=" "><a href="#">' + dataxc[i].ExpiryDate + '</a></td>' + '<td class=" "><a href="#">' + dataxc[i].SellingPrice + '</a></td>' + '</tr>')
    }
    MyTbl2 = $(tableID).DataTable({
        destroy: true,
        paging: false,
        ordering: false,
        searching: false,
        info: false,
        scrollY: 250,
        processing: false,
        scrollCollapse: false,
        scrollX: true,
    })
};
$(document).on('click', '.ViewDTL', function (e) {
    var data = MyTbl.row($(this).parents('tr')).data();
    var url = "";
    var page = $('#PageIS').text();
    if (page == "ItemStore") {
        url = GetAppName() + "/Item/ItemSubStoreView?itmid=" + data[0];
        window.location.replace(url)
    } else {
        url = GetAppName() + "/Item/Item?itmid=" + data[0];
        window.location.replace(url)
    }
    return false
});
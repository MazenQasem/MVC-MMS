    

function TestCreateTable(tableID, TabID, TransOrOpt, RackOrTransNo, ShelfID) {
         var URL = "";
    tbl = null;
    if (TabID == 0) {
        $('input:radio[name=r_gItem]:checked').val(TransOrOpt);
        $("#Rack1").val(RackOrTransNo);
        $("#Shelf1").val(ShelfID);
        URL = "/BatchLocator/GetDetails0";
    } else if (TabID == 1) { URL = "/BatchLocator/GetDetails1"; }
    else if (TabID == 2) { URL = "/BatchLocator/GetDetails2"; }
         tbl = $(tableID).dataTable({
        "bFilter": false,
        "bPaginate": true,
        "bRetrieve": true,
        "bDestroy": true,
        "sPaginationType": "full_numbers",
        "bServerSide": true,
        "sAjaxSource": URL,
        "bProcessing": false,
        "bDestroy": true,
        "aoColumns": [
                  {
                      "sName": "ID", "bSearchable": false, "bSortable": false, "sClass": "hidden", "aTargets": [0]
                  },
                  { "sName": "SNO", "bSearchable": false, "bSortable": false, "aTargets": [1] },
                  { "sName": "ItemCode", "aTargets": [2] },
                  { "sName": "ItemName", "aTargets": [3] },
                  { "sName": "BatchNo", "aTargets": [4] },
                  { "sName": "Quantity", "bSearchable": false, "bSortable": false, "aTargets": [5] },
                  {
                      "sName": "Rack", "aTargets": [6], "sClass": "EditMe1"
                  },
                  {
                      "sName": "Shelf", "aTargets": [7], "sClass": "EditMe2"
                  },
                  { "sName": "RackID", "aTargets": [8], "sClass": "hidden" },
                  { "sName": "ShelfID", "aTargets": [9], "sClass": "hidden" },
                  { "sName": "ExpiryDate", "bSearchable": false, "aTargets": [10] },
                  { "sName": "CostPrice", "bSearchable": false, "bSortable": false, "aTargets": [11] },
                  { "sName": "SellPrice", "bSearchable": false, "bSortable": false, "aTargets": [12] },
                  { "sName": "ItemID", "bSearchable": false, "bSortable": false, "sClass": "hidden", "aTargets": [13] },
                  { "sName": "StationID", "bSearchable": false, "bSortable": false, "sClass": "hidden", "aTargets": [14] },
                  { "sName": "receiptid", "bSearchable": false, "bSortable": false, "sClass": "hidden", "aTargets": [15] },
                  { "sName": "BatchID", "bSearchable": false, "bSortable": false, "sClass": "hidden", "aTargets": [16] }],
        "fnServerData": function (sScource, aoData, fnCallback) {
            var name = "";
            var TransTypeOrOption = 0;
            var RackOrTransNo = 0;
            if (TabID == 0) {
                TransTypeOrOption = $('input:radio[name=r_gItem]:checked').val();
                RackOrTransNo = $("#Rack1").val();
                name = $.trim($("#txtSearch0").val());
            }
            if (TabID == 1) {
                TransTypeOrOption = $("#TransList1").val();
                RackOrTransNo = $("#TransListNo1").val();
                name = $.trim($("#txtSearch1").val());
            }
            if (TabID == 2) {
                TransTypeOrOption = $("#TransList2").val();
                RackOrTransNo = $("#TransListNo2").val();
                name = $.trim($("#txtSearch2").val());
            }

            aoData.push({ "name": "Name", "value": name });
            aoData.push({ "name": "TabID", "value": TabID });
            aoData.push({ "name": "TransTypeOrOption", "value": TransTypeOrOption });
            aoData.push({ "name": "RackOrTransNo", "value": RackOrTransNo });
            aoData.push({ "name": "ShelfID", "value": $("#Shelf1").val() });
            $.ajax({
                "dataType": 'json',
                "type": "GET",
                "data": aoData,
                "url": sScource,
                "success": fnCallback
            });
        }
    })
        .makeEditable({
        sUpdateURL: function (value, settings) {
            return (value);          },

        "aoColumns": [null,null,null,null,null,null,
        {
            indicator: 'Saving Browser...',
                                                   onblur: 'submit',
                         fnOnCellUpdated: function (sStatus, sValue, settings) {
                             }
        }, {
            indicator: 'Saving Browser...',
                                                               onblur: 'submit',
                         fnOnCellUpdated: function (sStatus, sValue, settings) {
                             }
        }]
    })
    ;


          
};


function load_compgrade(compid) {
    $.ajax({
        url: "/BatchLocator/ForTestOnly_GetDetails",
        type: "POST",
        cache: false,
        dataType: 'json',
        success: function (data) {
            render_compeditgrade(data.GL);
        }
    });
}

function render_compeditgrade(datalist) {
    gradetbl = $("#DataTables_Tab0").DataTable({
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
            { data: 'ID', className: 'cAR-align-center', defaultContent: '<input type="checkbox" id="gradcbox" />', targets: [0] },
            { data: 'SNO', visible: false, targets: [1] },              { data: 'ItemCode', targets: [2] },              { data: 'ItemName', className: 'cAR-align-center', targets: [3] },              { data: 'BatchNo', className: 'cAR-align-center', targets: [4] },              { data: 'Quantity', className: 'numbereditor cAR-align-right', targets: [5] },              { data: 'Rack', className: 'numbereditor cAR-align-right', targets: [6] },              { data: 'Shelf', className: 'numbereditor cAR-align-right', targets: [7] },              { data: 'RackID', className: 'numbereditor2 cAR-align-right', targets: [8] },              { data: 'ShelfID', className: 'numbereditor cAR-align-right', targets: [9] },              { data: 'ExpiryDate', visible: false, targets: [10] },              { data: 'CostPrice', visible: false, targets: [11] },              { data: 'SellPrice', visible: false, targets: [12] },              { data: 'ItemId', defaultContent: 0, visible: false, targets: [13] },              { data: 'StationID', visible: false, targets: [14] },              { data: 'receiptid', visible: false, targets: [15] },              { data: 'BatchID', visible: false, targets: [16] }          ]
    });

    set_editables_tbl();
    init_editables_tbl();
}

function set_editables_tbl() {
    $.editable.addInputType('numberinput', {
        element: function (settings, original) {
            cellIndexC = tbl.cell(original).index();
            rowVal = tbl.cell(cellIndexC.row, 5).data();
            if (parseInt(rowVal) == 1) {
                input = $('<input type="text" class="cAR-editable-input" />');
            } else {
                input = $('<input type="text" class="cAR-editable-input cAR-disabled" style="outline:1px solid #e1e1e1 !important;" readonly="true"/>');
            }
            $(this).append(input);
            return (input);
        },
        plugin: function (settings, original) {
            $(this).find('input')
                .inputmask("decimal", { radixPoint: ".", autoGroup: true, groupSeparator: ",", groupSize: 3, digits: 2, allowMinus: false, allowPlus: true })
                .keydown(function (e) {
                    var cellIndex = tbl.cell(original).index();
                    if (e.keyCode == 40) {                          $('.numbereditor', tbl.rows().nodes()).find('form').submit();
                        if (cellIndex.row < tbl.rows().data().length - 1) {
                            $(tbl.cell(cellIndex.row + 1, cellIndex.column).node()).click();
                        }
                    }
                    else if (e.keyCode == 38) {                          $('.numbereditor', tbl.rows().nodes()).find('form').submit();
                        if (cellIndex.row > 0) {
                            $(tbl.cell(cellIndex.row - 1, cellIndex.column).node()).click();
                        }
                    }
                    else if (e.keyCode == 39) {                          $('.numbereditor', tbl.rows().nodes()).find('form').submit();
                        if ($(this).caret().start == $(this).val().length) {
                            $(tbl.cell(cellIndex.row, cellIndex.column + 1).node()).click();
                        }
                    }
                    else if (e.keyCode == 37) {                          $('.numbereditor', tbl.rows().nodes()).find('form').submit();
                        if ($(this).caret().end == 0) {
                            $(tbl.cell(cellIndex.row, cellIndex.column - 1).node()).click();
                        }
                    }
                    else if (e.keyCode == 13) {                          $('.numbereditor', tbl.rows().nodes()).find('form').submit();
                        $(tbl.cell(cellIndex.row, cellIndex.column + 1).node()).click();
                    }
                });
        }
    });

    $.editable.addInputType('numberinput2', {
        element: function (settings, original) {
            cellIndexC = tbl.cell(original).index();
            alert(cellIndexC);
            rowVal = tbl.cell(cellIndexC.row, 4).data();
            if (parseInt(rowVal) == 1) {
                input = $('<input type="text" class="cAR-editable-input" />');
            } else {
                input = $('<input type="text" class="cAR-editable-input cAR-disabled" style="outline:1px solid #e1e1e1 !important;" readonly="true"/>');
            }
            $(this).append(input);
            return (input);
        },
        plugin: function (settings, original) {
            $(this).find('input')
                .inputmask("integer", { allowMinus: false, allowPlus: true, mask: "9{1,2}", greedy: false })
                .keydown(function (e) {
                    var cellIndex = tbl.cell(original).index();
                    if (e.keyCode == 40) {                          $('.numbereditor2', tbl.rows().nodes()).find('form').submit();
                        if (cellIndex.row < tbl.rows().data().length - 1) {
                            $(tbl.cell(cellIndex.row + 1, cellIndex.column).node()).click();
                        }
                    }
                    else if (e.keyCode == 38) {                          $('.numbereditor2', tbl.rows().nodes()).find('form').submit();
                        if (cellIndex.row > 0) {
                            $(tbl.cell(cellIndex.row - 1, cellIndex.column).node()).click();
                        }
                    }
                    else if (e.keyCode == 39) {                          $('.numbereditor2', tbl.rows().nodes()).find('form').submit();
                        if ($(this).caret().start == $(this).val().length) {
                            $(tbl.cell(cellIndex.row, cellIndex.column + 1).node()).click();
                        }
                    }
                    else if (e.keyCode == 37) {                          $('.numbereditor2', tbl.rows().nodes()).find('form').submit();
                        if ($(this).caret().end == 0) {
                            $(tbl.cell(cellIndex.row, cellIndex.column - 1).node()).click();
                        }
                    }
                    else if (e.keyCode == 13) {                          $('.numbereditor2', tbl.rows().nodes()).find('form').submit();
                        $(tbl.cell(cellIndex.row, cellIndex.column + 1).node()).click();
                    }
                });
        }
    });
}

function init_editables_tbl() {

    $('.numbereditor', tbl.rows().nodes()).editable(function (sVal, settings) {

        var cell1 = tbl.cell($(this).closest('td')).index();
        tbl.cell(cell1.row, cell1.column).data(sVal);
        return sVal;
    },
    {
        "type": 'numberinput', "style": 'display: inline;',
        "onblur": function () {
            $('.numbereditor', tbl.rows().nodes()).find('form').submit();
            rowIndexEXd = tbl.cell($(this).closest('td')).index().row;
            per = tbl.cell(rowIndexEXd, 3).data();
            amt = tbl.cell(rowIndexEXd, 4).data();
                     },
        "onreset": function () { },
        "event": 'click', "submit": '', "cancel": '', "placeholder": '', "cssclass": "coleditor"
    });

    $('.numbereditor2', tbl.rows().nodes()).editable(function (sVal, settings) {
        var cell2 = DEServiceTableIP.cell($(this).closest('td')).index();
                                   return sVal;
    },
    {
        "type": 'numberinput2', "style": 'display: inline;',
        "onblur": function () {
            $('.numbereditor2', tbl.rows().nodes()).find('form').submit();
            rowIndexEXd = tbl.cell($(this).closest('td')).index().row;
            per = tbl.cell(rowIndexEXd, 3).data();
            amt = tbl.cell(rowIndexEXd, 4).data();
                     },
        "onreset": function () { },
        "event": 'click', "submit": '', "cancel": '', "placeholder": '', "cssclass": "coleditor"
    });

}

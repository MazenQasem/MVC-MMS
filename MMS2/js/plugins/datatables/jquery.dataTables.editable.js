
(function ($) {

    $.fn.makeEditable = function (options) {

        var iDisplayStart = 0;

        function fnGetCellID(cell) {
                                                                                                        
            return properties.fnGetRowID($(cell.parentNode));
        }

        function _fnSetRowIDInAttribute(row, id, overwrite) {
                                                                                                                                  
            if (overwrite) {
                row.attr("id", id);
            } else {
                if (row.attr("id") == null || row.attr("id") == "")
                    row.attr("id", id);
            }
        }

        function _fnGetRowIDFromAttribute(row) {
                                                                                                                                  
            return row.attr("id");
        }

        function _fnSetRowIDInFirstCell(row, id) {
                                                                                                                                  
            $("td:first", row).html(id);
        }


        function _fnGetRowIDFromFirstCell(row) {
                                                                                                                                  
            return $("td:first", row).html();

        }

                 var oTable;
                 var oAddNewRowButton, oDeleteRowButton, oConfirmRowAddingButton, oCancelRowAddingButton;
                 var oAddNewRowForm;

                 var properties;

        function _fnShowError(errorText, action) {
                                                                 
            alert(errorText);
        }

        function _fnStartProcessingMode() {
                                       
            if (oTable.fnSettings().oFeatures.bProcessing) {
                $(".dataTables_processing").css('visibility', 'visible');
            }
        }

        function _fnEndProcessingMode() {
                                                    
            if (oTable.fnSettings().oFeatures.bProcessing) {
                $(".dataTables_processing").css('visibility', 'hidden');
            }
        }

        var sOldValue, sNewCellValue, sNewCellDislayValue;

        function fnApplyEditable(aoNodes) {
                                                    
            if (properties.bDisableEditing)
                return;
            var oDefaultEditableSettings = {
                event: 'dblclick',

                "onsubmit": function (settings, original) {
                    sOldValue = original.revert;
                    sNewCellValue = null;
                    sNewCellDisplayValue = null;
                    iDisplayStart = fnGetDisplayStart();

                    if (settings.type == "text" || settings.type == "select" || settings.type == "textarea") {
                        var input = $("input,select,textarea", this);
                        sNewCellValue = $("input,select,textarea", $(this)).val();
                        if (input.length == 1) {
                            var oEditElement = input[0];
                            if (oEditElement.nodeName.toLowerCase() == "select" || oEditElement.tagName.toLowerCase() == "select")
                                sNewCellDisplayValue = $("option:selected", oEditElement).text();                              else
                                sNewCellDisplayValue = sNewCellValue;
                        }

                        if (!properties.fnOnEditing(input, settings, original.revert, fnGetCellID(original)))
                            return false;
                        var x = settings;

                                                 if (settings.oValidationOptions != null) {
                            input.parents("form").validate(settings.oValidationOptions);
                        }
                        if (settings.cssclass != null) {
                            input.addClass(settings.cssclass);
                        }
                        if (settings.cssclass == null && settings.oValidationOptions == null) {
                            return true;
                        } else {
                            if (!input.valid() || 0 == input.valid())
                                return false;
                            else
                                return true;
                        }

                    }

                    properties.fnStartProcessingMode();
                },
                "submitdata": function (value, settings) {
                                                              var id = fnGetCellID(this);
                    var rowId = oTable.fnGetPosition(this)[0];
                    var columnPosition = oTable.fnGetPosition(this)[1];
                    var columnId = oTable.fnGetPosition(this)[2];
                    var sColumnName = oTable.fnSettings().aoColumns[columnId].sName;
                    if (sColumnName == null || sColumnName == "")
                        sColumnName = oTable.fnSettings().aoColumns[columnId].sTitle;
                    var updateData = null;
                    if (properties.aoColumns == null || properties.aoColumns[columnId] == null) {
                        updateData = $.extend({},
                                            properties.oUpdateParameters,
                                            {
                                                "id": id,
                                                "rowId": rowId,
                                                "columnPosition": columnPosition,
                                                "columnId": columnId,
                                                "columnName": sColumnName
                                            });
                    }
                    else {
                        updateData = $.extend({},
                                            properties.oUpdateParameters,
                                            properties.aoColumns[columnId].oUpdateParameters,
                                            {
                                                "id": id,
                                                "rowId": rowId,
                                                "columnPosition": columnPosition,
                                                "columnId": columnId,
                                                "columnName": sColumnName
                                            });
                    }
                    return updateData;
                },
                "callback": function (sValue, settings) {
                    properties.fnEndProcessingMode();
                    var status = "";
                    var aPos = oTable.fnGetPosition(this);

                    var bRefreshTable = !oSettings.oFeatures.bServerSide;
                    $("td.last-updated-cell", oTable.fnGetNodes()).removeClass("last-updated-cell");
                    if (sValue.indexOf(properties.sFailureResponsePrefix) > -1) {
                        oTable.fnUpdate(sOldValue, aPos[0], aPos[2], bRefreshTable);
                        $("td.last-updated-cell", oTable).removeClass("last-updated-cell");
                        $(this).addClass("last-updated-cell");
                        properties.fnShowError(sValue.replace(properties.sFailureResponsePrefix, "").trim(), "update");
                        status = "failure";
                    } else {

                        if (properties.sSuccessResponse == "IGNORE" ||
                            (properties.aoColumns != null
                                && properties.aoColumns[aPos[2]] != null
                                && properties.aoColumns[aPos[2]].sSuccessResponse == "IGNORE") ||
                            (sNewCellValue == null) || (sNewCellValue == sValue) ||
                            properties.sSuccessResponse == sValue) {
                            if (sNewCellDisplayValue == null) {
                                                                 oTable.fnUpdate(sValue, aPos[0], aPos[2], bRefreshTable);
                            } else {
                                oTable.fnUpdate(sNewCellDisplayValue, aPos[0], aPos[2], bRefreshTable);
                            }
                            $("td.last-updated-cell", oTable).removeClass("last-updated-cell");
                            $(this).addClass("last-updated-cell");
                            status = "success";
                        } else {
                            oTable.fnUpdate(sOldValue, aPos[0], aPos[2], bRefreshTable);
                            properties.fnShowError(sValue, "update");
                            status = "failure";
                        }
                    }

                    properties.fnOnEdited(status, sOldValue, sNewCellDisplayValue, aPos[0], aPos[1], aPos[2]);
                    if (settings.fnOnCellUpdated != null) {
                        settings.fnOnCellUpdated(status, sValue, aPos[0], aPos[2], settings);
                    }

                    fnSetDisplayStart();
                    if (properties.bUseKeyTable) {
                        var keys = oTable.keys;
                        
                        setTimeout(function () { keys.block = false; }, 0);
                    }
                },
                "onerror": function () {
                    properties.fnEndProcessingMode();
                    properties.fnShowError("Cell cannot be updated", "update");
                    properties.fnOnEdited("failure");
                },

                "onreset": function () {
                    if (properties.bUseKeyTable) {
                        var keys = oTable.keys;
                        
                        setTimeout(function () { keys.block = false; }, 0);
                    }

                },
                "height": properties.sEditorHeight,
                "width": properties.sEditorWidth
            };

            var cells = null;

            if (properties.aoColumns != null) {

                for (var iDTindex = 0, iDTEindex = 0; iDTindex < oSettings.aoColumns.length; iDTindex++) {
                    if (oSettings.aoColumns[iDTindex].bVisible) {                         if (properties.aoColumns[iDTEindex] == null) {
                                                         iDTEindex++;
                            continue;
                        }
                                                 cells = $("td:nth-child(" + (iDTEindex + 1) + ")", aoNodes);

                        var oColumnSettings = oDefaultEditableSettings;
                        oColumnSettings = $.extend({}, oDefaultEditableSettings, properties.oEditableSettings, properties.aoColumns[iDTEindex]);
                        iDTEindex++;
                        var sUpdateURL = properties.sUpdateURL;
                        try {
                            if (oColumnSettings.sUpdateURL != null)
                                sUpdateURL = oColumnSettings.sUpdateURL;
                        } catch (ex) {
                        }
                                                 cells.each(function () {
                            if (!$(this).hasClass(properties.sReadOnlyCellClass)) {
                                $(this).editable(sUpdateURL, oColumnSettings);
                            }
                        });
                    }

                }              } else {
                cells = $('td:not(.' + properties.sReadOnlyCellClass + ')', aoNodes);
                cells.editable(properties.sUpdateURL, $.extend({}, oDefaultEditableSettings, properties.oEditableSettings));
            }
        }

        function fnOnRowAdding(event) {
                                                    
            if (properties.fnOnAdding()) {
                if (oAddNewRowForm.valid()) {
                    iDisplayStart = fnGetDisplayStart();
                    properties.fnStartProcessingMode();

                    if (properties.bUseFormsPlugin) {
                                                 $(oAddNewRowForm).ajaxSubmit({
                            dataType: 'xml',
                            success: function (response, statusString, xhr) {
                                if (xhr.responseText.toLowerCase().indexOf("error") != -1) {
                                    properties.fnEndProcessingMode();
                                    properties.fnShowError(xhr.responseText.replace("Error", ""), "add");
                                    properties.fnOnAdded("failure");
                                } else {
                                    fnOnRowAdded(xhr.responseText);
                                }

                            },
                            error: function (response) {
                                properties.fnEndProcessingMode();
                                properties.fnShowError(response.responseText, "add");
                                properties.fnOnAdded("failure");
                            }
                        }
                        );

                    } else {

                        var params = oAddNewRowForm.serialize();
                        $.ajax({
                            'url': properties.sAddURL,
                            'data': params,
                            'type': properties.sAddHttpMethod,
                            'dataType': properties.sAddDataType,
                            success: fnOnRowAdded,
                            error: function (response) {
                                properties.fnEndProcessingMode();
                                properties.fnShowError(response.responseText, "add");
                                properties.fnOnAdded("failure");
                            }
                        });
                    }
                }
            }
            event.stopPropagation();
            event.preventDefault();
        }

        function _fnOnNewRowPosted(data) {
                          
            return true;
        }


        function fnOnRowAdded(data) {
                                                                 
            properties.fnEndProcessingMode();

            if (properties.fnOnNewRowPosted(data)) {

                var oSettings = oTable.fnSettings();
                if (!oSettings.oFeatures.bServerSide) {
                    jQuery.data(oAddNewRowForm, 'DT_RowId', data);
                    var values = fnTakeRowDataFromFormElements(oAddNewRowForm);


                    var rtn;
                                         if (oSettings.aoColumns != null && isNaN(parseInt(oSettings.aoColumns[0].mDataProp))) {
                        rtn = oTable.fnAddData(rowData);
                    }
                    else {
                        rtn = oTable.fnAddData(values);
                    }

                    var oTRAdded = oTable.fnGetNodes(rtn);
                                         properties.fnSetRowID($(oTRAdded), data, true);
                                         fnApplyEditable(oTRAdded);

                    $("tr.last-added-row", oTable).removeClass("last-added-row");
                    $(oTRAdded).addClass("last-added-row");
                } 
                                 oAddNewRowForm.dialog('close');
                $(oAddNewRowForm)[0].reset();
                $(".error", $(oAddNewRowForm)).html("");

                fnSetDisplayStart();
                properties.fnOnAdded("success");
                if (properties.bUseKeyTable) {
                    var keys = oTable.keys;
                    
                    setTimeout(function () { keys.block = false; }, 0);
                }
            }
        }

        function fnOnCancelRowAdding(event) {
                                                                 
                         $(oAddNewRowForm).validate().resetForm();               $(oAddNewRowForm)[0].reset();

            $(".error", $(oAddNewRowForm)).html("");
            $(".error", $(oAddNewRowForm)).hide();   
                         oAddNewRowForm.dialog('close');
            event.stopPropagation();
            event.preventDefault();
        }


        function fnDisableDeleteButton() {
                                       
            if (properties.bUseKeyTable) {
                return;
            }
            if (properties.oDeleteRowButtonOptions != null) {
                                 oDeleteRowButton.button("option", "disabled", true);
            } else {
                oDeleteRowButton.attr("disabled", "true");
            }
        }

        function fnEnableDeleteButton() {
                                       
            if (properties.oDeleteRowButtonOptions != null) {
                                 oDeleteRowButton.button("option", "disabled", false);
            } else {
                oDeleteRowButton.removeAttr("disabled");
            }
        }

        var nSelectedRow, nSelectedCell;
        var oKeyTablePosition;


        function _fnOnRowDeleteInline(e) {

            var sURL = $(this).attr("href");
            if (sURL == null || sURL == "")
                sURL = properties.sDeleteURL;

            e.preventDefault();
            e.stopPropagation();

            iDisplayStart = fnGetDisplayStart();

            nSelectedCell = ($(this).parents('td'))[0];
            jSelectedRow = ($(this).parents('tr'));
            nSelectedRow = jSelectedRow[0];

            jSelectedRow.addClass(properties.sSelectedRowClass);

            var id = fnGetCellID(nSelectedCell);
            if (properties.fnOnDeleting(jSelectedRow, id, fnDeleteRow)) {
                fnDeleteRow(id, sURL);
            }
        }


        function _fnOnRowDelete(event) {
                                                    
            event.preventDefault();
            event.stopPropagation();

            iDisplayStart = fnGetDisplayStart();

            nSelectedRow = null;
            nSelectedCell = null;

            if (!properties.bUseKeyTable) {
                if ($('tr.' + properties.sSelectedRowClass + ' td', oTable).length == 0) {
                                         _fnDisableDeleteButton();
                    return;
                }
                nSelectedCell = $('tr.' + properties.sSelectedRowClass + ' td', oTable)[0];
            } else {
                nSelectedCell = $('td.focus', oTable)[0];

            }
            if (nSelectedCell == null) {
                fnDisableDeleteButton();
                return;
            }
            if (properties.bUseKeyTable) {
                oKeyTablePosition = oTable.keys.fnGetCurrentPosition();
            }
            var id = fnGetCellID(nSelectedCell);
            var jSelectedRow = $(nSelectedCell).parent("tr");
            nSelectedRow = jSelectedRow[0];
            if (properties.fnOnDeleting(jSelectedRow, id, fnDeleteRow)) {
                fnDeleteRow(id);
            }
        }

        function _fnOnDeleting(tr, id, fnDeleteRow) {
                                                                                                                     
            return confirm("Are you sure that you want to delete this record?");;
        }


        function fnDeleteRow(id, sDeleteURL) {
                                                                 
            var sURL = sDeleteURL;
            if (sDeleteURL == null)
                sURL = properties.sDeleteURL;
            properties.fnStartProcessingMode();
            var data = $.extend(properties.oDeleteParameters, { "id": id });
            $.ajax({
                'url': sURL,
                'type': properties.sDeleteHttpMethod,
                'data': data,
                "success": fnOnRowDeleted,
                "dataType": properties.sDeleteDataType,
                "error": function (response) {
                    properties.fnEndProcessingMode();
                    properties.fnShowError(response.responseText, "delete");
                    properties.fnOnDeleted("failure");

                }
            });
        }



        function fnOnRowDeleted(response) {
                                                    
            properties.fnEndProcessingMode();
            var oTRSelected = nSelectedRow;
            
            if (response == properties.sSuccessResponse || response == "") {
                oTable.fnDeleteRow(oTRSelected);
                fnDisableDeleteButton();
                fnSetDisplayStart();
                if (properties.bUseKeyTable) {
                    oTable.keys.fnSetPosition(oKeyTablePosition[0], oKeyTablePosition[1]);
                }
                properties.fnOnDeleted("success");
            }
            else {
                properties.fnShowError(response, "delete");
                properties.fnOnDeleted("failure");
            }
        }



        
        function _fnOnDeleted(result) { }

        function _fnOnEditing(input) { return true; }
        function _fnOnEdited(result, sOldValue, sNewValue, iRowIndex, iColumnIndex, iRealColumnIndex) {

        }

        function fnOnAdding() { return true; }
        function _fnOnAdded(result) { }

        var oSettings;
        function fnGetDisplayStart() {
            return oSettings._iDisplayStart;
        }

        function fnSetDisplayStart() {
                                       
                                                                                                   }

        function _fnOnBeforeAction(sAction) {
            return true;
        }

        function _fnOnActionCompleted(sStatus) {

        }

        function fnGetActionSettings(sAction) {
                          
            if (properties.aoTableAction)
                properties.fnShowError("Configuration error - aoTableAction setting are not set", sAction);
            var i = 0;

            for (i = 0; i < properties.aoTableActions.length; i++) {
                if (properties.aoTableActions[i].sAction == sAction)
                    return properties.aoTableActions[i];
            }

            properties.fnShowError("Cannot find action configuration settings", sAction);
        }


        function fnPopulateFormWithRowCells(oForm, oTR) {
                                       
            var iRowID = oTable.fnGetPosition(oTR);

            var id = properties.fnGetRowID($(oTR));

            $(oForm).validate().resetForm();
            jQuery.data($(oForm)[0], 'DT_RowId', id);
            $("input.DT_RowId", $(oForm)).val(id);
            jQuery.data($(oForm)[0], 'ROWID', iRowID);
            $("input.ROWID", $(oForm)).val(iRowID);


            var oSettings = oTable.fnSettings();
            var iColumnCount = oSettings.aoColumns.length;


            $("input:text[rel],input:radio[rel][checked],input:hidden[rel],select[rel],textarea[rel],input:checkbox[rel]",
                                    $(oForm)).each(function () {
                                        var rel = $(this).attr("rel");

                                        if (rel >= iColumnCount)
                                            properties.fnShowError("In the form is placed input element with the name '" + $(this).attr("name") + "' with the 'rel' attribute that must be less than a column count - " + iColumnCount, "action");
                                        else {
                                            var sCellValue = oTable.fnGetData(oTR)[rel];
                                            if (this.nodeName.toLowerCase() == "select" || this.tagName.toLowerCase() == "select") {

                                                if (this.multiple == true) {
                                                    var aoSelectedValue = new Array();
                                                    aoCellValues = sCellValue.split(",");
                                                    for (i = 0; i <= this.options.length - 1; i++) {
                                                        if (jQuery.inArray(this.options[i].text.toLowerCase().trim(), aoCellValues) != -1) {
                                                            aoSelectedValue.push(this.options[i].value);
                                                        }
                                                    }
                                                    $(this).val(aoSelectedValue);
                                                } else {
                                                    for (i = 0; i <= this.options.length - 1; i++) {
                                                        if (this.options[i].text.toLowerCase() == sCellValue.toLowerCase()) {
                                                            $(this).val(this.options[i].value);
                                                        }
                                                    }
                                                }

                                            }
                                            else if (this.nodeName.toLowerCase() == "span" || this.tagName.toLowerCase() == "span")
                                                $(this).html(sCellValue);
                                            else {
                                                if (this.type == "checkbox") {
                                                    if (sCellValue == "true") {
                                                        $(this).attr("checked", true);
                                                    }
                                                } else {
                                                    if (this.type == "radio") {
                                                        if (this.value == sCellValue) {
                                                            this.checked = true;
                                                        }
                                                    } else {
                                                        this.value = sCellValue;
                                                    }
                                                }
                                            }

                                                                                                                                                                               }
                                    });



        }  
        function fnTakeRowDataFromFormElements(oForm) {
                                                    
            var iDT_RowId = jQuery.data(oForm, 'DT_RowId');
            var iColumnCount = oSettings.aoColumns.length;

            var values = new Array();
            var rowData = new Object();

            $("input:text[rel],input:radio[rel][checked],input:hidden[rel],select[rel],textarea[rel],span.datafield[rel],input:checkbox[rel]", oForm).each(function () {
                var rel = $(this).attr("rel");
                var sCellValue = "";
                if (rel >= iColumnCount)
                    properties.fnShowError("In the add form is placed input element with the name '" + $(this).attr("name") + "' with the 'rel' attribute that must be less than a column count - " + iColumnCount, "add");
                else {
                    if (this.nodeName.toLowerCase() == "select" || this.tagName.toLowerCase() == "select") {
                                                 sCellValue = $.map(
                                             $.makeArray($("option:selected", this)),
                                             function (n, i) {
                                                 return $(n).text();
                                             }).join(",");
                    }
                    else if (this.nodeName.toLowerCase() == "span" || this.tagName.toLowerCase() == "span")
                        sCellValue = $(this).html();
                    else {
                        if (this.type == "checkbox") {
                            if (this.checked)
                                sCellValue = (this.value != "on") ? this.value : "true";
                            else
                                sCellValue = (this.value != "on") ? "" : "false";
                        } else
                            sCellValue = this.value;
                    }
                                         sCellValue = sCellValue.replace("DATAROWID", iDT_RowId);
                    sCellValue = sCellValue.replace(properties.sIDToken, iDT_RowId);
                    if (oSettings.aoColumns != null
                                && oSettings.aoColumns[rel] != null
                                && isNaN(parseInt(oSettings.aoColumns[0].mDataProp))) {
                        rowData[oSettings.aoColumns[rel].mDataProp] = sCellValue;
                    } else {
                        values[rel] = sCellValue;
                    }
                }
            });

            if (oSettings.aoColumns != null && isNaN(parseInt(oSettings.aoColumns[0].mDataProp))) {
                return rowData;
            }
            else {
                return values;
            }


        }  



        function fnSendFormUpdateRequest(nActionForm) {
                          
            var jActionForm = $(nActionForm);
            var sAction = jActionForm.attr("id");

            sAction = sAction.replace("form", "");
            var sActionURL = jActionForm.attr("action");
            if (properties.fnOnBeforeAction(sAction)) {
                if (jActionForm.valid()) {
                    iDisplayStart = fnGetDisplayStart();
                    properties.fnStartProcessingMode();
                    if (properties.bUseFormsPlugin) {

                                                 var oAjaxSubmitOptions = {
                            success: function (response, statusString, xhr) {
                                properties.fnEndProcessingMode();
                                if (response.toLowerCase().indexOf("error") != -1 || statusString != "success") {
                                    properties.fnShowError(response, sAction);
                                    properties.fnOnActionCompleted("failure");
                                } else {
                                    fnUpdateRowOnSuccess(nActionForm);
                                    properties.fnOnActionCompleted("success");
                                }

                            },
                            error: function (response) {
                                properties.fnEndProcessingMode();
                                properties.fnShowError(response.responseText, sAction);
                                properties.fnOnActionCompleted("failure");
                            }
                        };
                        var oActionSettings = fnGetActionSettings(sAction);
                        oAjaxSubmitOptions = $.extend({}, properties.oAjaxSubmitOptions, oAjaxSubmitOptions);
                        $(oActionForm).ajaxSubmit(oAjaxSubmitOptions);

                    } else {
                        var params = jActionForm.serialize();
                        $.ajax({
                            'url': sActionURL,
                            'data': params,
                            'type': properties.sAddHttpMethod,
                            'dataType': properties.sAddDataType,
                            success: function (response) {
                                properties.fnEndProcessingMode();
                                fnUpdateRowOnSuccess(nActionForm);
                                properties.fnOnActionCompleted("success");
                            },
                            error: function (response) {
                                properties.fnEndProcessingMode();
                                properties.fnShowError(response.responseText, sAction);
                                properties.fnOnActionCompleted("failure");
                            }
                        });
                    }
                }
            }
        }

        function fnUpdateRowOnSuccess(nActionForm) {
                                      var bRefreshTable = !oSettings.oFeatures.bServerSide;

            var values = fnTakeRowDataFromFormElements(nActionForm);

            var iRowID = jQuery.data(nActionForm, 'ROWID');
            var oSettings = oTable.fnSettings();
            var iColumnCount = oSettings.aoColumns.length;
            for (var rel = 0; rel < iColumnCount; rel++) {
                if (oSettings.aoColumns != null
                                && oSettings.aoColumns[rel] != null
                                && isNaN(parseInt(oSettings.aoColumns[0].mDataProp))) {
                    sCellValue = rowData[oSettings.aoColumns[rel].mDataProp];
                } else {
                    sCellValue = values[rel];
                }
                if (sCellValue != undefined)
                    oTable.fnUpdate(sCellValue, iRowID, rel, bRefreshTable);
            }

            fnSetDisplayStart();
            $(nActionForm).dialog('close');
            return;

        }


        oTable = this;

        var defaults = {

            sUpdateURL: "UpdateData",
            sAddURL: "AddData",
            sDeleteURL: "DeleteData",
            sAddNewRowFormId: "formAddNewRow",
            oAddNewRowFormOptions: { autoOpen: false, modal: true },
            sAddNewRowButtonId: "btnAddNewRow",
            oAddNewRowButtonOptions: null,
            sAddNewRowOkButtonId: "btnAddNewRowOk",
            sAddNewRowCancelButtonId: "btnAddNewRowCancel",
            oAddNewRowOkButtonOptions: { label: "Ok" },
            oAddNewRowCancelButtonOptions: { label: "Cancel" },
            sDeleteRowButtonId: "btnDeleteRow",
            oDeleteRowButtonOptions: null,
            sSelectedRowClass: "row_selected",
            sReadOnlyCellClass: "read_only",
            sAddDeleteToolbarSelector: ".add_delete_toolbar",
            fnShowError: _fnShowError,
            fnStartProcessingMode: _fnStartProcessingMode,
            fnEndProcessingMode: _fnEndProcessingMode,
            aoColumns: null,
            fnOnDeleting: _fnOnDeleting,
            fnOnDeleted: _fnOnDeleted,
            fnOnAdding: fnOnAdding,
            fnOnNewRowPosted: _fnOnNewRowPosted,
            fnOnAdded: _fnOnAdded,
            fnOnEditing: _fnOnEditing,
            fnOnEdited: _fnOnEdited,
            sAddHttpMethod: 'POST',
            sAddDataType: "text",
            sDeleteHttpMethod: 'POST',
            sDeleteDataType: "text",
            fnGetRowID: _fnGetRowIDFromAttribute,
            fnSetRowID: _fnSetRowIDInAttribute,
            sEditorHeight: "100%",
            sEditorWidth: "100%",
            bDisableEditing: false,
            oDeleteParameters: {},
            oUpdateParameters: {},
            sIDToken: "DT_RowId",
            aoTableActions: null,
            fnOnBeforeAction: _fnOnBeforeAction,
            bUseFormsPlugin: false,
            fnOnActionCompleted: _fnOnActionCompleted,
            sSuccessResponse: "ok",
            sFailureResponsePrefix: "ERROR",
            oKeyTable: null         
        };

        properties = $.extend(defaults, options);
        oSettings = oTable.fnSettings();
        properties.bUseKeyTable = (properties.oKeyTable != null);

        return this.each(function () {
            var sTableId = oTable.dataTableSettings[0].sTableId;
                         if (properties.bUseKeyTable) {
                var keys = new KeyTable({
                    "table": document.getElementById(sTableId),
                    "datatable": oTable
                });
                oTable.keys = keys;

                
                keys.event.action(null, null, function (nCell) {
                    if ($(nCell).hasClass(properties.sReadOnlyCellClass))
                        return;
                    
                    keys.block = true;
                    
                    setTimeout(function () { $(nCell).dblclick(); }, 0);
                                     });
            }






             
            if (oTable.fnSettings().sAjaxSource != null) {
                oTable.fnSettings().aoDrawCallback.push({
                    "fn": function () {
                                                 fnApplyEditable(oTable.fnGetNodes());
                        $(oTable.fnGetNodes()).each(function () {
                            var position = oTable.fnGetPosition(this);
                            var id = oTable.fnGetData(position)[0];
                            properties.fnSetRowID($(this), id);
                        }
                        );
                    },
                    "sName": "fnApplyEditable"
                });

            } else {
                                 fnApplyEditable(oTable.fnGetNodes());
            }

                         oAddNewRowForm = $("#" + properties.sAddNewRowFormId);
            if (oAddNewRowForm.length != 0) {

                                 var oSettings = oTable.fnSettings();
                var iColumnCount = oSettings.aoColumns.length;
                for (i = 0; i < iColumnCount; i++) {
                    if ($("[rel=" + i + "]", oAddNewRowForm).length == 0)
                        properties.fnShowError("In the form that is used for adding new records cannot be found an input element with rel=" + i + " that will be bound to the value in the column " + i + ". See http:                 }


                if (properties.oAddNewRowFormOptions != null) {
                    properties.oAddNewRowFormOptions.autoOpen = false;
                } else {
                    properties.oAddNewRowFormOptions = { autoOpen: false };
                }
                oAddNewRowForm.dialog(properties.oAddNewRowFormOptions);

                                 oAddNewRowButton = $("#" + properties.sAddNewRowButtonId);
                if (oAddNewRowButton.length != 0) {

                    if (oAddNewRowButton.data("add-event-attached") != "true") {
                        oAddNewRowButton.click(function () {
                            oAddNewRowForm.dialog('open');
                        });
                        oAddNewRowButton.data("add-event-attached", "true");
                    }

                } else {
                    if ($(properties.sAddDeleteToolbarSelector).length == 0) {
                        throw "Cannot find a button with an id '" + properties.sAddNewRowButtonId + "', or placeholder with an id '" + properties.sAddDeleteToolbarSelector + "' that should be used for adding new row although form for adding new record is specified";
                    } else {
                        oAddNewRowButton = null;                      }
                }

                                 if (oAddNewRowForm[0].nodeName.toLowerCase() == "form") {
                    oAddNewRowForm.unbind('submit');
                    oAddNewRowForm.submit(function (event) {
                        fnOnRowAdding(event);
                        return false;
                    });
                } else {
                    $("form", oAddNewRowForm[0]).unbind('submit');
                    $("form", oAddNewRowForm[0]).submit(function (event) {
                        fnOnRowAdding(event);
                        return false;
                    });
                }

                                 var aAddNewRowFormButtons = [];

                oConfirmRowAddingButton = $("#" + properties.sAddNewRowOkButtonId, oAddNewRowForm);
                if (oConfirmRowAddingButton.length == 0) {
                                         if (properties.oAddNewRowOkButtonOptions.text == null
                        || properties.oAddNewRowOkButtonOptions.text == "") {
                        properties.oAddNewRowOkButtonOptions.text = "Ok";
                    }
                    properties.oAddNewRowOkButtonOptions.click = fnOnRowAdding;
                    properties.oAddNewRowOkButtonOptions.id = properties.sAddNewRowOkButtonId;
                                         aAddNewRowFormButtons.push(properties.oAddNewRowOkButtonOptions);
                } else {
                    oConfirmRowAddingButton.click(fnOnRowAdding);
                }

                oCancelRowAddingButton = $("#" + properties.sAddNewRowCancelButtonId);
                if (oCancelRowAddingButton.length == 0) {
                                         if (properties.oAddNewRowCancelButtonOptions.text == null
                        || properties.oAddNewRowCancelButtonOptions.text == "") {
                        properties.oAddNewRowCancelButtonOptions.text = "Cancel";
                    }
                    properties.oAddNewRowCancelButtonOptions.click = fnOnCancelRowAdding;
                    properties.oAddNewRowCancelButtonOptions.id = properties.sAddNewRowCancelButtonId;
                                         aAddNewRowFormButtons.push(properties.oAddNewRowCancelButtonOptions);
                } else {
                    oCancelRowAddingButton.click(fnOnCancelRowAdding);
                }
                                 if (aAddNewRowFormButtons.length > 0) {
                    oAddNewRowForm.dialog('option', 'buttons', aAddNewRowFormButtons);
                }
                                                                   oConfirmRowAddingButton = $("#" + properties.sAddNewRowOkButtonId);
                oCancelRowAddingButton = $("#" + properties.sAddNewRowCancelButtonId);

                if (properties.oAddNewRowFormValidation != null) {
                    oAddNewRowForm.validate(properties.oAddNewRowFormValidation);
                }
            } else {
                oAddNewRowForm = null;
            }

                         oDeleteRowButton = $('#' + properties.sDeleteRowButtonId);
            if (oDeleteRowButton.length != 0) {
                if (oDeleteRowButton.data("delete-event-attached") != "true") {
                    oDeleteRowButton.click(_fnOnRowDelete);
                    oDeleteRowButton.data("delete-event-attached", "true");
                }
            }
            else {
                oDeleteRowButton = null;
            }

                                      oAddDeleteToolbar = $(properties.sAddDeleteToolbarSelector);
            if (oAddDeleteToolbar.length != 0) {
                if (oAddNewRowButton == null && properties.sAddNewRowButtonId != ""
                    && oAddNewRowForm != null) {
                    oAddDeleteToolbar.append("<button id='" + properties.sAddNewRowButtonId + "' class='add_row'>Add</button>");
                    oAddNewRowButton = $("#" + properties.sAddNewRowButtonId);
                    oAddNewRowButton.click(function () { oAddNewRowForm.dialog('open'); });
                }
                if (oDeleteRowButton == null && properties.sDeleteRowButtonId != "") {
                    oAddDeleteToolbar.append("<button id='" + properties.sDeleteRowButtonId + "' class='delete_row'>Delete</button>");
                    oDeleteRowButton = $("#" + properties.sDeleteRowButtonId);
                    oDeleteRowButton.click(_fnOnRowDelete);
                }
            }

                         if (oDeleteRowButton != null) {
                if (properties.oDeleteRowButtonOptions != null) {
                    oDeleteRowButton.button(properties.oDeleteRowButtonOptions);
                }
                fnDisableDeleteButton();
            }

                         if (oAddNewRowButton != null) {
                if (properties.oAddNewRowButtonOptions != null) {
                    oAddNewRowButton.button(properties.oAddNewRowButtonOptions);
                }
            }


                         if (oConfirmRowAddingButton != null) {
                if (properties.oAddNewRowOkButtonOptions != null) {
                    oConfirmRowAddingButton.button(properties.oAddNewRowOkButtonOptions);
                }
            }

                         if (oCancelRowAddingButton != null) {
                if (properties.oAddNewRowCancelButtonOptions != null) {
                    oCancelRowAddingButton.button(properties.oAddNewRowCancelButtonOptions);
                }
            }

                          
            if (!properties.bUseKeyTable) {
                                                  $("tbody", oTable).click(function (event) {
                    if ($(event.target.parentNode).hasClass(properties.sSelectedRowClass)) {
                        $(event.target.parentNode).removeClass(properties.sSelectedRowClass);
                        if (oDeleteRowButton != null) {
                            fnDisableDeleteButton();
                        }
                    } else {
                        $(oTable.fnSettings().aoData).each(function () {
                            $(this.nTr).removeClass(properties.sSelectedRowClass);
                        });
                        $(event.target.parentNode).addClass(properties.sSelectedRowClass);
                        if (oDeleteRowButton != null) {
                            fnEnableDeleteButton();
                        }
                    }
                });
            } else {
                oTable.keys.event.focus(null, null, function (nNode, x, y) {

                });
            }

            if (properties.aoTableActions != null) {
                for (var i = 0; i < properties.aoTableActions.length; i++) {
                    var oTableAction = $.extend({ sType: "edit" }, properties.aoTableActions[i]);
                    var sAction = oTableAction.sAction;
                    var sActionFormId = oTableAction.sActionFormId;

                    var oActionForm = $("#form" + sAction);
                    if (oActionForm.length != 0) {
                        var oFormOptions = { autoOpen: false, modal: true };
                        oFormOptions = $.extend({}, oTableAction.oFormOptions, oFormOptions);
                        oActionForm.dialog(oFormOptions);
                        oActionForm.data("action-options", oTableAction);

                        var oActionFormLink = $(".table-action-" + sAction);
                        if (oActionFormLink.length != 0) {

                            oActionFormLink.live("click", function () {


                                var sClass = this.className;
                                var classList = sClass.split(/\s+/);
                                var sActionFormId = "";
                                var sAction = "";
                                for (i = 0; i < classList.length; i++) {
                                    if (classList[i].indexOf("table-action-") > -1) {
                                        sAction = classList[i].replace("table-action-", "");
                                        sActionFormId = "#form" + sAction;
                                    }
                                }
                                if (sActionFormId == "") {
                                    properties.fnShowError("Cannot find a form with an id " + sActionFormId + " that should be associated to the action - " + sAction, sAction)
                                }

                                var oTableAction = $(sActionFormId).data("action-options");

                                if (oTableAction.sType == "edit") {

                                                                         var oTR = ($(this).parents('tr'))[0];
                                    fnPopulateFormWithRowCells(oActionForm, oTR);
                                }
                                $(oActionForm).dialog('open');
                            });
                        }

                        oActionForm.submit(function (event) {

                            fnSendFormUpdateRequest(this);
                            return false;

                        });


                        var aActionFormButtons = new Array();

                                                                          var oActionFormCancel = $("#form" + sAction + "Cancel", oActionForm);
                        if (oActionFormCancel.length != 0) {
                            aActionFormButtons.push(oActionFormCancel);
                            oActionFormCancel.click(function () {

                                var oActionForm = $(this).parents("form")[0];
                                                                 $(oActionForm).validate().resetForm();                                   $(oActionForm)[0].reset();

                                $(".error", $(oActionForm)).html("");
                                $(".error", $(oActionForm)).hide();                                   $(oActionForm).dialog('close');
                            });
                        }

                                                 $("button", oActionForm).button();
                        



                    }




                }              }  

        });
    };
})(jQuery);


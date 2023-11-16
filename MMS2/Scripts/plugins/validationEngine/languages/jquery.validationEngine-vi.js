(function($){
    $.fn.validationEngineLanguage = function(){
    };
    $.validationEngineLanguage = {
        newLang: function(){
            $.validationEngineLanguage.allRules = {
                "required": {                      "regex": "none",
                    "alertText": "* Trường này bắt buộc",
                    "alertTextCheckboxMultiple": "* Vui lòng chọn một tùy chọn",
                    "alertTextCheckboxe": "* Checkbox này bắt buộc",
                    "alertTextDateRange": "* Cả hai trường ngày tháng đều bắt buộc"
                },
                "requiredInFunction": { 
                    "func": function(field, rules, i, options){
                        return (field.val() == "test") ? true : false;
                    },
                    "alertText": "* Giá trị của trường phải là test"
                },
                "dateRange": {
                    "regex": "none",
                    "alertText": "* Không đúng ",
                    "alertText2": "Khoảng ngày tháng"
                },
                "dateTimeRange": {
                    "regex": "none",
                    "alertText": "* Không đúng ",
                    "alertText2": "Khoảng thời gian"
                },
                "minSize": {
                    "regex": "none",
                    "alertText": "* Tối thiểu ",
                    "alertText2": " số ký tự được cho phép"
                },
                "maxSize": {
                    "regex": "none",
                    "alertText": "* Tối đa ",
                    "alertText2": " số ký tự được cho phép"
                },
				"groupRequired": {
                    "regex": "none",
                    "alertText": "* Bạn phải điền một trong những trường sau"
                },
                "min": {
                    "regex": "none",
                    "alertText": "* Giá trị nhỏ nhất là "
                },
                "max": {
                    "regex": "none",
                    "alertText": "* Giá trị lớn nhất là "
                },
                "past": {
                    "regex": "none",
                    "alertText": "* Ngày kéo dài tới "
                },
                "future": {
                    "regex": "none",
                    "alertText": "* Ngày đã qua "
                },	
                "maxCheckbox": {
                    "regex": "none",
                    "alertText": "* Tối đa ",
                    "alertText2": " số tùy chọn được cho phép"
                },
                "minCheckbox": {
                    "regex": "none",
                    "alertText": "* Vui lòng chọn ",
                    "alertText2": " các tùy chọn"
                },
                "equals": {
                    "regex": "none",
                    "alertText": "* Giá trị các trường không giống nhau"
                },
                "creditCard": {
                    "regex": "none",
                    "alertText": "* Số thẻ tín dụng sai"
                },
                "phone": {
                                         "regex": /^([\+][0-9]{1,3}[\ \.\-])?([\(]{1}[0-9]{2,6}[\)])?([0-9\ \.\-\/]{3,20})((x|ext|extension)[\ ]?[0-9]{1,4})?$/,
                    "alertText": "* Số điện thoại sai"
                },
                "email": {
                                         "regex": /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/,
                    "alertText": "* Địa chỉ thư điện tử sai"
                },
                "integer": {
                    "regex": /^[\-\+]?\d+$/,
                    "alertText": "* Không đúng là số nguyên"
                },
                "number": {
                                         "regex": /^[\-\+]?((([0-9]{1,3})([,][0-9]{3})*)|([0-9]+))?([\.]([0-9]+))?$/,
                    "alertText": "* Không đúng là số thập phân"
                },
                "date": {
                    "regex": /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])$/,
                    "alertText": "* Ngày sai, phải có định dạng YYYY-MM-DD"
                },
                "ipv4": {
                    "regex": /^((([01]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))[.]){3}(([0-1]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))$/,
                    "alertText": "* Địa chỉ IP sai"
                },
                "url": {
                    "regex": /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i,
                    "alertText": "* URL sai"
                },
                "onlyNumberSp": {
                    "regex": /^[0-9\ ]+$/,
                    "alertText": "* Chỉ điền số"
                },
                "onlyLetterSp": {
                    "regex": /^[a-zA-Z\ \']+$/,
                    "alertText": "* Chỉ điền chữ"
                },
                "onlyLetterNumber": {
                    "regex": /^[0-9a-zA-Z]+$/,
                    "alertText": "* Không được chứa ký tự đặc biệt"
                },
                                 "ajaxUserCall": {
                    "url": "ajaxValidateFieldUser",
                                         "extraData": "name=eric",
                    "alertText": "* Tên này được dùng",
                    "alertTextLoad": "* Đang xác nhận, vui lòng chờ"
                },
				"ajaxUserCallPhp": {
                    "url": "phpajax/ajaxValidateFieldUser.php",
                                         "extraData": "name=eric",
                                         "alertTextOk": "* Tên người dùng này có thể dùng được",
                    "alertText": "* Tên người dùng này đã được sử dụng",
                    "alertTextLoad": "* Đang xác nhận, vui lòng chờ"
                },
                "ajaxNameCall": {
                                         "url": "ajaxValidateFieldName",
                                         "alertText": "* Tên này được dùng",
                                         "alertTextOk": "* Tên này có thể dùng",
                                         "alertTextLoad": "* Đang xác nhận, vui lòng chờ"
                },
				 "ajaxNameCallPhp": {
	                     	                    "url": "phpajax/ajaxValidateFieldName.php",
	                     	                    "alertText": "* Tên này được dùng",
	                     	                    "alertTextLoad": "* Đang xác nhận, vui lòng chờ"
	                },
                "validate2fields": {
                    "alertText": "* Vui lòng nhập vào HELLO"
                },
	                             "dateFormat":{
                    "regex": /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])$|^(?:(?:(?:0?[13578]|1[02])(\/|-)31)|(?:(?:0?[1,3-9]|1[0-2])(\/|-)(?:29|30)))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(?:(?:0?[1-9]|1[0-2])(\/|-)(?:0?[1-9]|1\d|2[0-8]))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(0?2(\/|-)29)(\/|-)(?:(?:0[48]00|[13579][26]00|[2468][048]00)|(?:\d\d)?(?:0[48]|[2468][048]|[13579][26]))$/,
                    "alertText": "* Ngày sai"
                },
                 				"dateTimeFormat": {
	                "regex": /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])\s+(1[012]|0?[1-9]){1}:(0?[1-5]|[0-6][0-9]){1}:(0?[0-6]|[0-6][0-9]){1}\s+(am|pm|AM|PM){1}$|^(?:(?:(?:0?[13578]|1[02])(\/|-)31)|(?:(?:0?[1,3-9]|1[0-2])(\/|-)(?:29|30)))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^((1[012]|0?[1-9]){1}\/(0?[1-9]|[12][0-9]|3[01]){1}\/\d{2,4}\s+(1[012]|0?[1-9]){1}:(0?[1-5]|[0-6][0-9]){1}:(0?[0-6]|[0-6][0-9]){1}\s+(am|pm|AM|PM){1})$/,
                    "alertText": "* Ngày sai hoặc định dạng ngày sai",
                    "alertText2": "Định dạng đúng là: ",
                    "alertText3": "mm/dd/yyyy hh:mm:ss AM|PM hay ", 
                    "alertText4": "yyyy-mm-dd hh:mm:ss AM|PM"
	            }
            };
            
        }
    };

    $.validationEngineLanguage.newLang();
    
})(jQuery);

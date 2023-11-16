(function($){
    $.fn.validationEngineLanguage = function(){
    };
    $.validationEngineLanguage = {
        newLang: function(){
            $.validationEngineLanguage.allRules = {
                "required": {                      "regex": "none",
                    "alertText": "* Dette feltet er påkrevd",
                    "alertTextCheckboxMultiple": "* Velg et alternativ",
                    "alertTextCheckboxe": "* Denne boksen er påkrevd",
                    "alertTextDateRange": "* Begge datofelt må fylles ut"
                },
                "requiredInFunction": { 
                    "func": function(field, rules, i, options){
                        return (field.val() == "test") ? true : false;
                    },
                    "alertText": "* Feltet må være lik test"
                },
                "dateRange": {
                    "regex": "none",
                    "alertText": "* Ugyldig ",
                    "alertText2": "Datointervall"
                },
                "dateTimeRange": {
                    "regex": "none",
                    "alertText": "* Ugyldig ",
                    "alertText2": "Tidsintervall"
                },
                "minSize": {
                    "regex": "none",
                    "alertText": "* Minimum ",
                    "alertText2": " bokstaver tillatt"
                },
                "maxSize": {
                    "regex": "none",
                    "alertText": "* Maksimalt ",
                    "alertText2": " bokstaver tillatt"
                },
				"groupRequired": {
                    "regex": "none",
                    "alertText": "* Du må fylle ett av de følgende feltene"
                },
                "min": {
                    "regex": "none",
                    "alertText": "* Minimumsverdi er "
                },
                "max": {
                    "regex": "none",
                    "alertText": "* Maksimumsverdi er "
                },
                "past": {
                    "regex": "none",
                    "alertText": "* Dato før "
                },
                "future": {
                    "regex": "none",
                    "alertText": "* Dato etter "
                },	
                "maxCheckbox": {
                    "regex": "none",
                    "alertText": "* Maksimalt ",
                    "alertText2": " alternativer tillatt"
                },
                "minCheckbox": {
                    "regex": "none",
                    "alertText": "* Vennligst velg ",
                    "alertText2": " alternativer"
                },
                "equals": {
                    "regex": "none",
                    "alertText": "* Feltene samsvarer ikke"
                },
                "creditCard": {
                    "regex": "none",
                    "alertText": "* Ugyldig kredittkortnummer"
                },
                "phone": {
                                         "regex": /^([\+][0-9]{1,3}[\ \.\-])?([\(]{1}[0-9]{2,6}[\)])?([0-9\ \.\-\/]{3,20})((x|ext|extension)[\ ]?[0-9]{1,4})?$/,
                    "alertText": "* Ugyldig telefonnummer"
                },
                "email": {
                                         "regex": /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/,
                    "alertText": "* Ugyldig e-postadresse"
                },
                "integer": {
                    "regex": /^[\-\+]?\d+$/,
                    "alertText": "* Ikke et gyldig heltall"
                },
                "number": {
                                         "regex": /^[\-\+]?((([0-9]{1,3})([,][0-9]{3})*)|([0-9]+))?([\.]([0-9]+))?$/,
                    "alertText": "* Ugyldig desimaltall"
                },
                "date": {                    
                     			"func": function (field) {
					var pattern = new RegExp(/^(\d{4})[\/\-\.](0?[1-9]|1[012])[\/\-\.](0?[1-9]|[12][0-9]|3[01])$/);
					var match = pattern.exec(field.val());
					if (match == null)
					   return false;
	
					var year = match[1];
					var month = match[2]*1;
					var day = match[3]*1;					
					var date = new Date(year, month - 1, day);  	
					return (date.getFullYear() == year && date.getMonth() == (month - 1) && date.getDate() == day);
				},                		
			 "alertText": "* Ugyldig dato, må være i formatet ÅÅÅÅ-MM-DD"
                },
                "ipv4": {
                    "regex": /^((([01]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))[.]){3}(([0-1]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))$/,
                    "alertText": "* Ugyldig IP-adresse"
                },
                "url": {
                    "regex": /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i,
                    "alertText": "* Ugyldig nettadresse"
                },
                "onlyNumberSp": {
                    "regex": /^[0-9\ ]+$/,
                    "alertText": "* Kun tall"
                },
                "onlyLetterSp": {
                    "regex": /^[a-zA-Z\ \']+$/,
                    "alertText": "* Kun bokstaver"
                },
                "onlyLetterNumber": {
                    "regex": /^[0-9a-zA-Z]+$/,
                    "alertText": "* Ingen spesielle tegn er tillatt"
                },
                                 "ajaxUserCall": {
                    "url": "ajaxValidateFieldUser",
                                         "extraData": "name=eric",
                    "alertText": "* Denne brukeren er allerede tatt",
                    "alertTextLoad": "* Validerer, vennligst vent"
                },
				"ajaxUserCallPhp": {
                    "url": "phpajax/ajaxValidateFieldUser.php",
                                         "extraData": "name=eric",
                                         "alertTextOk": "* Denne brukeren er tilgjengelig",
                    "alertText": "* Denne brukeren er allerede tatt",
                    "alertTextLoad": "* Validerer, vennligst vent"
                },
                "ajaxNameCall": {
                                         "url": "ajaxValidateFieldName",
                                         "alertText": "* Dette navnet er allerede tatt",
                                         "alertTextOk": "* Dette navnet er tilgjengelig",
                                         "alertTextLoad": "* Validerer, vennligst vent"
                },
				 "ajaxNameCallPhp": {
	                     	                    "url": "phpajax/ajaxValidateFieldName.php",
	                     	                    "alertText": "* Dette navnet er allerede tatt",
	                     	                    "alertTextLoad": "* Validerer, vennligst vent"
	                },
                "validate2fields": {
                    "alertText": "* Vennligst skriv HELLO"
                },
	                             "dateFormat":{
                    "regex": /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])$|^(?:(?:(?:0?[13578]|1[02])(\/|-)31)|(?:(?:0?[1,3-9]|1[0-2])(\/|-)(?:29|30)))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(?:(?:0?[1-9]|1[0-2])(\/|-)(?:0?[1-9]|1\d|2[0-8]))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^(0?2(\/|-)29)(\/|-)(?:(?:0[48]00|[13579][26]00|[2468][048]00)|(?:\d\d)?(?:0[48]|[2468][048]|[13579][26]))$/,
                    "alertText": "* Ugyldig dato"
                },
                 				"dateTimeFormat": {
	                "regex": /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])\s+(1[012]|0?[1-9]){1}:(0?[1-5]|[0-6][0-9]){1}:(0?[0-6]|[0-6][0-9]){1}\s+(am|pm|AM|PM){1}$|^(?:(?:(?:0?[13578]|1[02])(\/|-)31)|(?:(?:0?[1,3-9]|1[0-2])(\/|-)(?:29|30)))(\/|-)(?:[1-9]\d\d\d|\d[1-9]\d\d|\d\d[1-9]\d|\d\d\d[1-9])$|^((1[012]|0?[1-9]){1}\/(0?[1-9]|[12][0-9]|3[01]){1}\/\d{2,4}\s+(1[012]|0?[1-9]){1}:(0?[1-5]|[0-6][0-9]){1}:(0?[0-6]|[0-6][0-9]){1}\s+(am|pm|AM|PM){1})$/,
                    "alertText": "* Ugyldig dato eller datoformat",
                    "alertText2": "Forventet format: ",
                    "alertText3": "mm/dd/åååå tt:mm:ss AM|PM or ", 
                    "alertText4": "åååå-mm-dd tt:mm:ss AM|PM"
	            }
            };
            
        }
    };

    $.validationEngineLanguage.newLang();
    
})(jQuery);

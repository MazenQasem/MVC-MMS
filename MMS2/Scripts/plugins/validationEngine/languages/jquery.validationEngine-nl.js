(function($){
	$.fn.validationEngineLanguage = function(){
	};
	$.validationEngineLanguage = {
	    newLang: function(){
	        $.validationEngineLanguage.allRules = {
	            "required": {  	                "regex": "geen",
	                "alertText": "* Dit veld is verplicht",
	                "alertTextCheckboxMultiple": "* Selecteer a.u.b. een optie",
	                "alertTextCheckboxe": "* Dit selectievakje is verplicht"
	            },
	            "requiredInFunction": { 
                    "func": function(field, rules, i, options){
                        return (field.val() == "test") ? true : false;
                    },
                    "alertText": "* Field must equal test"
                	    },
	            "minSize": {
	                "regex": "none",
	                "alertText": "* Minimaal ",
	                "alertText2": " karakters toegestaan"
	            },
	            "maxSize": {
	                "regex": "none",
	                "alertText": "* Maximaal ",
	                "alertText2": " karakters toegestaan"
	            },
				"groupRequired": {
                    "regex": "none",
                    "alertText": "* You must fill one of the following fields"
                },
	            "min": {
	                "regex": "none",
	                "alertText": "* Minimale waarde is "
	            },
	            "max": {
	                "regex": "none",
	                "alertText": "* Maximale waarde is "
	            },
	            "past": {
	                "regex": "none",
	                "alertText": "* Datum voorafgaand aan "
	            },
	            "future": {
	                "regex": "none",
	                "alertText": "* Datum na "
	            },
	            "maxCheckbox": {
	                "regex": "none",
	                "alertText": "* Toegestane aantal vinkjes overschreden"
	            },
	            "minCheckbox": {
	                "regex": "none",
	                "alertText": "* Selecteer a.u.b. ",
	                "alertText2": " opties"
	            },
	            "equals": {
	                "regex": "none",
	                "alertText": "* Velden komen niet overeen"
	            },
                "creditCard": {
                    "regex": "none",
                    "alertText": "* Ongeldige credit card nummer"
                },
	            "phone": {
	                 	                "regex": /^([\+][0-9]{1,3}[ \.\-])?([\(]{1}[0-9]{2,6}[\)])?([0-9 \.\-\/]{3,20})((x|ext|extension)[ ]?[0-9]{1,4})?$/,
	                "alertText": "* Ongeldig telefoonnummer"
	            },
	            "email": {
	                 	                "regex": /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i,
	                "alertText": "* Ongeldig e-mailadres"
	            },
	            "integer": {
	                "regex": /^[\-\+]?\d+$/,
	                "alertText": "* Ongeldig geheel getal"
	            },
	            "number": {
	                 	                "regex": /^[\-\+]?((([0-9]{1,3})([,][0-9]{3})*)|([0-9]+))?([\.]([0-9]+))?$/,
	                "alertText": "* Ongeldig drijvende comma getal"
	            },
	            "date": {
	                "regex": /^\d{4}[\/\-](0?[1-9]|1[012])[\/\-](0?[1-9]|[12][0-9]|3[01])$/,
	                "alertText": "* Ongeldige datum, formaat moet JJJJ-MM-DD zijn"
	            },
	            "ipv4": {
	            	"regex": /^((([01]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))[.]){3}(([0-1]?[0-9]{1,2})|(2[0-4][0-9])|(25[0-5]))$/,
	                "alertText": "* Ongeldig IP-adres"
	            },
	            "url": {
                    "regex": /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i,
	                "alertText": "* Ongeldige URL"
	            },
	            "onlyNumberSp": {
	                "regex": /^[0-9\ ]+$/,
	                "alertText": "* Alleen cijfers"
	            },
	            "onlyLetterSp": {
	                "regex": /^[a-zA-Z\ \']+$/,
	                "alertText": "* Alleen leestekens"
	            },
	            "onlyLetterNumber": {
	                "regex": /^[0-9a-zA-Z]+$/,
	                "alertText": "* Geen vreemde tekens toegestaan"
	            },
	             	            "ajaxUserCall": {
	                "url": "ajaxValidateFieldUser",
	                 	                "extraData": "name=eric",
	                "alertText": "* Deze gebruiker bestaat al",
	                "alertTextLoad": "* Bezig met valideren, even geduld aub"
	            },
	            "ajaxNameCall": {
	                 	                "url": "ajaxValidateFieldName",
	                 	                "alertText": "* Deze naam bestaat al",
	                 	                "alertTextOk": "* Deze naam is beschikbaar",
	                 	                "alertTextLoad": "* Bezig met valideren, even geduld aub"
	            },
	            "validate2fields": {
	                "alertText": "* Voer aub HELLO in"
	            }
	        };

	    }
	};
	$.validationEngineLanguage.newLang();
})(jQuery);


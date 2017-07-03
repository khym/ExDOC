/*! jQuery Validation Plugin - v1.00.0 - 01/10/2013
* Copyleft (L) 2013 TNC NOK */

(function ($) {
    var decimalSeparator = '.';

    $.validate = function (options) {
        if (options != null && options.form != null) {
            var elems = $('#' + options.form).find("input, textarea, select");
        }
        else {
            var elems = $("input, textarea, select");
        }

        var isValid = true;
        var tmpStatus = true;

        elems.each(function () {
            var validator = $(this).attr("data-validate");
            var value = $(this).val() != '' ? $(this).val() : ($(this).prop("tagName") == "TEXTAREA" ? $(this).text() : $(this).val());

            if (validator != null) {
                // Check required
                if (tmpStatus && validator.indexOf("required") != -1) {
                    if (($(this).prop("tagName") == "INPUT" && value == '') ||
                        ((($(this).prop("tagName") == "SELECT" && $(this).attr("multiple") != "multiple" && (value == 0 || value == '')) ||
                            ($(this).prop("tagName") == "TEXTAREA") && (value == '')) ||
                            ($(this).prop("tagName") == "SELECT" && $(this).attr("multiple") == "multiple" && $(this).find("option").length == 0))) {
                        $(this).removeClass("valid");
                        $(this).parent().removeClass("has-success");
                        $(this).addClass("error");
                        $(this).parent().addClass("has-error");
                        isValid = false;
                        tmpStatus = false;
                    }
                    else {
                        $(this).parent().removeClass("has-error");
                        $(this).removeClass("error");
                        $(this).addClass("valid");
                        $(this).parent().addClass("has-success");

                    }
                }

            }

            tmpStatus = true;
        });

        return isValid;
    };


}(jQuery));